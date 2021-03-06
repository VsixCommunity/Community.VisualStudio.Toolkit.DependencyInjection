// This will add the toolkit assembly to Visual Studio's probing path.
// Without it, Visual Studio is unable to find the assembly and the extension will fail to load.
using Microsoft.VisualStudio.Shell;
using System.Reflection;

[assembly: ProvideCodeBase(AssemblyName = "Community.VisualStudio.Toolkit.DependencyInjection.Core")]
[assembly: ProvideCodeBase(AssemblyName = "Community.VisualStudio.Toolkit.DependencyInjection.Microsoft")]
[assembly: ProvideCodeBase(AssemblyName = "Microsoft.Extensions.DependencyInjection")]
[assembly: ProvideCodeBase(AssemblyName = "Microsoft.Extensions.DependencyInjection.Abstractions")]