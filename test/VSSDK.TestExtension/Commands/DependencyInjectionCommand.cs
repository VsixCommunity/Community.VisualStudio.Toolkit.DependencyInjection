﻿using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
using Community.VisualStudio.Toolkit.DependencyInjection;
using Community.VisualStudio.Toolkit.DependencyInjection.Core;
using Microsoft.VisualStudio.Shell;
using VSSDK.TestExtension;

namespace TestExtension.Commands
{
    [Command(PackageIds.TestDependencyInjection)]
    internal sealed class DependencyInjectionCommand : BaseDICommand
    {
        private readonly SomeSingletonObject _singletonObject;
        public DependencyInjectionCommand(DIToolkitPackage package, SomeSingletonObject singletonObject)
            : base(package)
        {
            this._singletonObject = singletonObject;
        }

        protected async override Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            IToolkitServiceProvider<TestExtensionPackage> serviceProvider = await VS.GetServiceAsync<SToolkitServiceProvider<TestExtensionPackage>, IToolkitServiceProvider<TestExtensionPackage>>();

            if (serviceProvider == null)
                await VS.MessageBox.ShowErrorAsync($"There was an error trying to retrieve the {nameof(IToolkitServiceProvider<TestExtensionPackage>)}.");
            else
                await VS.MessageBox.ShowAsync($"The {nameof(IToolkitServiceProvider<TestExtensionPackage>)} was retrieved from the service collection succuessfully!");
        }
    }
}
