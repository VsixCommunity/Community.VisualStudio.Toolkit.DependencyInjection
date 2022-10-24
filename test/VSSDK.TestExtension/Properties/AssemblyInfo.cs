using Microsoft.VisualStudio.Shell;

[assembly: ProvideCodeBase(AssemblyName = "Community.VisualStudio.Toolkit")]

[assembly: ProvideBindingRedirection(AssemblyName = "Community.VisualStudio.Toolkit",
    NewVersion = "17.0.475.0", OldVersionLowerBound = "17.0.430.0", OldVersionUpperBound = "17.0.451.0")]
