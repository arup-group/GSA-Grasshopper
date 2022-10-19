using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using GsaGH;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle(GsaGHInfo.PluginName)]
[assembly: AssemblyDescription(GsaGHInfo.Company + " " + GsaGHInfo.ProductName + " Grasshopper plugin")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(GsaGHInfo.Company)]
[assembly: AssemblyProduct(GsaGHInfo.ProductName)]
[assembly: AssemblyCopyright(GsaGHInfo.Copyright)]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("362f5ed1-ba80-4f21-881d-6bd8784612e4")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion(GsaGHInfo.Vers)]
[assembly: AssemblyFileVersion(GsaGHInfo.Vers + ".0")]

[assembly: InternalsVisibleTo("GsaGHTests")]
