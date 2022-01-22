# Dependency Injection for the [Community.VisualStudio.Toolkit](https://github.com/VsixCommunity/Community.VisualStudio.Toolkit)

[![Build status](https://ci.appveyor.com/api/projects/status/a77wgqcu1c9qb64a?svg=true)](https://ci.appveyor.com/project/madskristensen/community-visualstudio-toolkit-dependencyinjection)
[![NuGet](https://img.shields.io/nuget/vpre/Community.VisualStudio.Toolkit.DependencyInjection.Core.17)](https://nuget.org/packages/Community.VisualStudio.Toolkit.DependencyInjection.Core.17/)
[![NuGet](https://img.shields.io/nuget/vpre/Community.VisualStudio.Toolkit.DependencyInjection.Microsoft.17)](https://nuget.org/packages/Community.VisualStudio.Toolkit.DependencyInjection.Microsoft.17/)

## Usage

Currently this package comes with a single implementation DI container, namely the `Microsoft.Extensions.DependencyInjection.ServiceCollection`.

To use this DI provider, your Visual Studio extension package will need to inherit from the `Community.VisualStudio.Toolkit.DependencyInjection.Microsoft.MicrosoftDIToolkitPackage`.
Doing so provides the `ServiceProvider` property along with the `virtual` method `InitializeServices`.

To register your services in the DI container you will need to override the `InitializeServices` method. 
Once the services have been registered in the collection, they can be retrieved by using the `ServiceProvider` property. 
An example implementation could be:

```csharp
[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[Guid(PackageGuids.DITestExtensionString)]
[ProvideMenuResource("Menus.ctmenu", 1)]
public sealed class TestExtensionPackage : MicrosoftDIToolkitPackage<TestExtensionPackage>
{
    protected override void InitializeServices(IServiceCollection services)
    {
        // Register your services here
        services.AddSingleton<IYourService, YourService>();
        ...
    }

    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
        // Ensure that you first call the base.InitializeAsync method.
        await base.InitializeAsync(cancellationToken, progress);

        // Your initialization code here
        var yourService = this.ServiceProvider.GetRequiredService<IYourService>();
        ...
    }
}

```

## Retrieving the `IServiceProvider` from the main VS Service Provider

As part of the initialization process, the DI container is registered into Visual Studio's container as well. 
You can retrieve your custom container using the following:

```csharp
// Retrieve the IServiceProvider
var serviceProvider = await VS.GetServiceAsync<SToolkitServiceProvider<TestExtensionPackage>, IToolkitServiceProvider<TestExtensionPackage>>();

// Retrieve your service from the IServiceProvider
var yourService = serviceProvider.GetRequiredService<IYourService>();
``` 

## Retrieving your package from the DI container

You can retrieve the instance of your package from the DI container using one of the following:

```csharp
// Retrieve it as your package type
var testExtensionPackage = serviceProvider.GetRequiredService<TestExtensionPackage>();

// Retrieve it as the base class type 'AsyncPackage'
var asyncPackage = serviceProvider.GetRequiredService<AsyncPackage>();
```

## Additional DI providers
The package has been written to allow additional containers if the need arrises. 
Look at the `MicrosoftDIToolkitPackage` implementation for how to incorporate your own DI container of choice.