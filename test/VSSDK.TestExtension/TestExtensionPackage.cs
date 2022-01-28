using System;
using System.Runtime.InteropServices;
using Community.VisualStudio.Toolkit;
using Community.VisualStudio.Toolkit.DependencyInjection.Microsoft;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Shell;
using TestExtension;

namespace VSSDK.TestExtension
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuids.DITestExtensionString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class TestExtensionPackage : MicrosoftDIToolkitPackage<TestExtensionPackage>
    {
        protected override void InitializeServices(IServiceCollection services)
        {
            base.InitializeServices(services);

            // Register any commands into the DI container
            services.RegisterCommands(ServiceLifetime.Singleton);

            // Register anything else
            services.AddSingleton<SomeSingletonObject>();
        }
    }

    internal class SomeSingletonObject
    {

    }
}
