# .NET License Manager
This small project will help you to manage licensing in your .NET applications. Please note that it's still
a work in progress (inspired from Stack Overflow [Where can I store (and manage) Application license information?](http://stackoverflow.com/q/20676008/1207195)) then many things need to be completed (see issues) and code must be reviewed and simplified.

All documentation is still a TO DO, please refer to both source code and cited SO post.

Any help on this (also for documentation) is more than welcome!

## Glossary

**Client**: the machine where application you want to protect is deployed.

**Server**: the machine where you create licenses. It may be a public accessible web server or
simply your development machine but your customers must not have direct access to its content.

**Contact**: it's a text string generated at client side which contains some unique identifiers
used to uniquely tie one license to a specific machine.

**License**: it's a text string generated at server side and used client side which contains information about
the license (to whom you licensed your application, its expiration and which features are enabled). License
is generated from a contact and it's indissolubly tied to a specific machine.

## How It Works

TODO (or Wiki?)

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
    if (LicenseManager.IsLicenseValid == false)
      QuitApplicationBecauseInvalidLicense();

    if (LicenseManager.IsFeatureAvailable(Feature.AdvancedExpensiveFeature) {
      ExecuteAnAdvancedExpensiveFeature(); 
    }
```

* TODO
