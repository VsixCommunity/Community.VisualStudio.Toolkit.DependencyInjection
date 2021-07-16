using System;
using System.Runtime.InteropServices;
using System.Threading;
using Community.VisualStudio.Toolkit.DependencyInjection.Microsoft;
using Microsoft.VisualStudio.Shell;
using TestExtension;
using TestExtension.Commands;
using Task = System.Threading.Tasks.Task;

namespace VSSDK.TestExtension
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuids.DITestExtensionString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class TestExtensionPackage : MicrosoftDIToolkitPackage<TestExtensionPackage>
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // Commands
            await DependencyInjectionCommand.InitializeAsync(this);
        }
    }
}
