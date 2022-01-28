using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Shell;

namespace Community.VisualStudio.Toolkit.DependencyInjection.Core
{
    /// <summary>
    /// Base class used for commands instantiated by the DI container.
    /// </summary>
    public abstract class BaseDICommand : BaseCommand
    {
        /// <summary>
        /// Constructor for the BaseDICommand
        /// </summary>
        /// <param name="package"></param>
        public BaseDICommand(DIToolkitPackage package)
        {
            this.Package = package;
            var commandWrapperType = typeof(CommandWrapper<>).MakeGenericType(this.GetType());
            var commandWrapper = package.ServiceProvider.GetRequiredService(commandWrapperType);
            var commandPropertyInfo = commandWrapperType.GetProperty(nameof(BaseCommand.Command));
            this.Command = (OleMenuCommand)commandPropertyInfo.GetValue(commandWrapper, null);
        }
    }
}
