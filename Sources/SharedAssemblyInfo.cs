using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;

#if DEBUG
#if PRIVATE_BUILD
[assembly: AssemblyConfiguration("Debug (private)")]
#else
[assembly: AssemblyConfiguration("Debug")]
#endif
#else
#if PRIVATE_BUILD
[assembly: AssemblyConfiguration("Release (private)")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
#endif

[assembly: AssemblyCompany("Rdev")]
[assembly: AssemblyProduct("Radev Licensing")]
[assembly: AssemblyCopyright("Copyright © Repetti Adriano 2015. All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: Localizable(false)]

[assembly: ComVisible(false)]

// Note that automatic build number with strongly signed
// assemblies will prevent you to mix different versions. It is (IMO)
// a little security improvement but it will make harder to release patches.
[assembly: AssemblyVersion("1.0.*")]

