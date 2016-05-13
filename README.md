# .NET License Manager
This small project will help you to manage licensing in your .NET applications. To check for license
may be as simple as:

```C#
if (!LicenseManagerIsLicenseValid)
  Environment.Exit(1);
```

Please note that it's still a work in progress (inspired from Stack Overflow [Where can I store (and manage) Application license information?](http://stackoverflow.com/q/20676008/1207195)) then many things need to be completed (see issues) and code must be reviewed and simplified. All documentation is still a TO DO, please refer to both source code and cited SO post. Any help on this (also for documentation) is more than welcome!

## Glossary

**Client**: the machine where application you want to protect is deployed.

**Server**: the machine where you create licenses. It may be a public accessible web server or
simply your development machine but your customers must not have direct access to its content.

**Contact**: it's a text string generated at client side which contains some unique identifiers
used to uniquely tie one license to a specific machine.

**License**: it's a text string generated at server side and used client side which contains information about
the license (to whom you licensed your application, its expiration and which features are enabled). License
is generated from a contact and it's indissolubly tied to a specific machine.

## How to Use It
This project cannot be used as-is, to be effective you must include full source code in your repository
and change it as described here, read carefully these notes. Project is separated in groups:

* Code that must deployed together with your application stays in `Radev.Licensing.dll`. This assembly also
contains code used by server components.
* Code that **must** be only on your private server (if you wish to have a private license server) stays in
`Radev.Licensing.Server.dll`. Your customers must never have an accessible copy of this assembly because
it contains your private key.
* Code that must be directly included in your application, possibly in each single assembly you will check
for license (for example if you have a modular architecture, where one assembly implements a set of features,
you should include one copy of this file in each assembly). A generic template is `LicenseManager.cs` but
it must be customized for your specific use.

To start using this project you have to carefully follow these steps:

* Create a new public/private key pair and save it as XML. Copy public key in `Radev.Licensing\Keys.cs` and
both public and private keys in `Radev.Licensing.Server\Keys.cs`.
* Include a copy of `LicenseManager.cs` in each assembly where you need to check for the license together with a reference
to `Radev.Licensing.dll`.
* Update `Feature` enum to include all features you want to protect with license. You may simply omit this file (and relative
code in `LicenseManager`) if you just need to know if there is a license but you don't protect single features.
* Create an utility (or _something_ automatic, if you have a public licensing web server) to generate a contact:

```C#
ContactWriter.ToFile("path_for_contact", ContactFactory.Create<Contact>());
```

If you need plain text (for example to send an e-mail) you can use `ContactWriter.ToString()`.
* Create an utility (or _something_ automatic, if you have a public licensing web server) to generate a license
from a contact file:

```C#
var contact = ContactReader.FromFile("path_for_contact");
var license = LicenseFactory.Create<License>(contact);

// Fill license with any information you need
// ...

LicenseWriter.ToFile("path_for_license", license);
```

* When you need to check for license (or for a specific feature) simply call `LicenseManager` methods:
 
```C#
    // Mere license presence enables a basic set of features...
    if (LicenseManager.IsLicenseValid == false)
      QuitApplicationBecauseInvalidLicense();

    // Here we check if an advanced feature is available too...
    if (LicenseManager.IsFeatureAvailable(Feature.AdvancedExpensiveFeature)) {
      ExecuteAnAdvancedExpensiveFeature(); 
    }
```

Please note you can assign a validity interval for your license (I strongly suggest to always
make it valid from the day it has been created) and also specify which version (again a range)
of your software is enabled by that license. For example:

```C#
// This license is valid from now to the far future...
license.Validity = new Interval(DateTime.Now, null);

// For any software versione in the 2.xx family
license.MinimumVersion = new Version(2, 0); 
license.MaximumVersion = new Version(2, 65535);
```

## How It Works

To have an adequate protection few assumptions must be satisfied:

* You want individually license each copy of your software and tie a license to the exact machine
to which it has been associated: licenses cannot be copied and reused between different machines.
* Licese file _may_ be hidden but its strength does not come from that nor from obfuscation: even if
it is accessible and modifiable your application must have a mechanism to realibly validate it.
* Code that check for the license is reasonably protected and it is not easy to _patch_ it to
circumvent the license.

### One License for Each Client

First of all we need to uniquely identify a specific machine. There are various IDs and
settings we may use but we must combine more than one of them because hardware configuration
may change and we do not want to invalidate our license for each small change.

I decided to use WMI queries to read configuration values, because they're easy to use and to change,
and by default (but it can be changed in `Licensing\Token.cs`) I read only IDs, because they're less
subject to change compared to - for example - memory size. Note that this choice is not mandatory
and you may want to change it implementing your own `Licensing\Client\HardwareAnalyzer.cs` class
and changing `Licensing\Token.cs` accordingly.

Application will compare hardware configuration stored in its license with current live configuration
and if they differ more than a specified amount then it will consider license as invalid. For details
check `License.IsAssociatedWithThisMachine()` and `License.MaximumNumberOfHardwareChanges`. Currently
license will check these values:

* Computer manufacturer and model. These values should not ever change over time, unless you also
change motherboard, but they're not widely available and they may contain dummy text.
* Motherboard manufacturer, model and serial number. These values should not change over time but
sometimes (especially on old systems) serial number is _fake_. 
* BIOS serial number. Also this value may be a _fake_ on very old motherboards.
* CPU ID. This value is not a _true_ unique ID of your CPU but it model and stepping, combined with
other IDs, will help you to uniquely identify a specific system.
* OS serial number and installation date. Here you rely on Operating System license protection...
if they use your application on two different machines then you assume they also need to use
two different OS copies (unless they also want to crack OS). Installation date (absolutely optional)
lets you detect false claims like _I had to reinstall OS because of broken hard disk_.

Note that I didn't use any disk serial number (hard to reliably read) nor MAC addresses (NICs
may be turned on and off at run-time).

Note that you may use motherboard and computer manufacturer name to _detect_ virtual machines
(Microsoft, Oracle, VMWare and similar). It's not a reliable method but it has the advantage to
be extremly simple. For a more reliable check you have to individually check for virtual machines
you know, for example:

```C
// Code to detect VMWare virtual machines
bool IsRunningInsideVmWare()
{
	bool flag = true;

	__try
	{
		__asm
		{
			push	edx
			push	ecx
			push	ebx

			mov		eax, 'VMXh'
			mov		ebx, 0
			mov		ecx, 10
			mov		edx, 'VX'

			in		eax, dx
			cmp		ebx, 'VMXh'
			setz	[flag]

			pop		ebx
			pop		ecx
			pop		edx
		}
	}
	__except(EXCEPTION_EXECUTE_HANDLER)
	{
		flag = false;
	}

	return flag;
}

// Code to detect Virtual PC virtual machines
DWORD __forceinline VpcExceptionFilter(LPEXCEPTION_POINTERS ep)
{
	PCONTEXT ctx = ep->ContextRecord;

	ctx->Ebx = -1;
	ctx->Eip += 4;

	return EXCEPTION_CONTINUE_EXECUTION;
}

bool IsRunningInsideVpc()
{
	bool flag = false;

	__try
	{
		_asm
		{
			push	ebx

			mov		eax, 1
			mov		ebx, 0 

			__emit	0Fh
			__emit	3Fh
			__emit	07h
			__emit	0Bh

			test	ebx, ebx
			setz	[flag]

			pop		ebx
		}
	}
	__except(VpcExceptionFilter(GetExceptionInformation()))
	{
	}

	return flag;
}
```

### Secure License File

After we collected all required information into a _contact_ then we encrypt that data with our public
key and then encode it as base64 (to make it easy to transfer, for example, via e-mail). Note that
here encryption isn't strictly necessary (because everything a cracker needs is available and visible
in application code).

Server code will then decode and decrypt contact information, fill license as required and then
sign result with our private key. Everything is then encoded again as base64. From this moment
our **application can detect any change to license content** because signature (verified with public key)
won't match.

### Protect Application Assemblies

Of course you also need to protect application assemblies otherwise they may be tampered. This topic is
vast (especially because a cracker has a big _surface attack_) but you should at least start signing
all your assemblies and then - eventually - embed `Radev.Licensing.dll` into your application assembly as
a resource (to make things slightly more complicate for _casual_ crackers), you will load it inside
`AppDomain.CurrentDomain.AssemblyResolve` event handler.

There are more things to consider:

* If application can load untrusted/unsigned plugins then cracker may simply use Reflection to
bypass/modify already validated in-memory license data.
* Code may be patched at run-time bypassing license checks (the (in)famous `NOP` instead of `CALL`).
* WMI information may be _faked_.
* An entire system may be cloned using virtual machines.
* Application may be _shared_ using Web Access, Terminal Server or similar technology.

More about this later...
