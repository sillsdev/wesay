using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("WeSay.Data")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("WeSay")]
[assembly: AssemblyProduct("WeSay")]
[assembly: AssemblyCopyright("Copyright © WeSay 2006")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("331d99db-dead-481c-b42f-cbba11076646")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers
// by using the '*' as shown below:
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: System.CLSCompliant(true)]
[assembly: System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.RequestMinimum, Execution=true)]
[assembly: InternalsVisibleTo("WeSay.Data.Tests")]
[assembly: InternalsVisibleTo("WeSay.Project.Tests")]
[assembly: InternalsVisibleTo("LexicalModel.Tests")]
[assembly: InternalsVisibleTo("LexicalTools.Tests")]
[assembly: InternalsVisibleTo("WeSay.App.Tests")]
