using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Community.VisualStudio.Toolkit.DependencyInjection.Core
{
    /// <summary>
    /// Base class used for commands instantiated by the DI container.
    /// </summary>
    public abstract class BaseDICommand
    {
        private static readonly PropertyInfo _commandPropertyInfo = typeof(CommandWrapper<>).GetProperty(nameof(Command));

        /// <summary>
        /// The package.
        /// </summary>
        public DIToolkitPackage Package { get; }

        /// <summary>
        /// The command object associated with the command ID (GUID/ID).
        /// </summary>
        public OleMenuCommand Command { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="package"></param>
        public BaseDICommand(DIToolkitPackage package)
        {
            this.Package = package;
            var commandWrapperType = typeof(CommandWrapper<>).MakeGenericType(this.GetType());
            var commandWrapper = package.ServiceProvider.GetRequiredService(commandWrapperType);
            PropertyInfo commandPropertyInfo = commandWrapperType.GetProperty(nameof(Command));
            this.Command = (OleMenuCommand)commandPropertyInfo.GetValue(commandWrapper, null);
        }

        /// <summary>
        /// Execute the command logica
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected internal virtual void Execute(object sender, EventArgs e)
        {
            Package.JoinableTaskFactory.RunAsync(async delegate
            {
                try
                {
                    await ExecuteAsync((OleMenuCmdEventArgs)e);
                }
                catch (Exception ex)
                {
                    await ex.LogAsync();
                }
            }).FireAndForget();
        }

        /// <summary>Executes asynchronously when the command is invoked and <see cref="Execute(object, EventArgs)"/> isn't overridden.</summary>
        /// <remarks>Use this method instead of <see cref="Execute"/> if you're invoking any async tasks by using async/await patterns.</remarks>
        protected internal virtual Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// BeforeQueryStatus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected internal virtual void BeforeQueryStatus(object sender, EventArgs e)
        {
            BeforeQueryStatus(e);
        }

        /// <summary>Override this method to control the commands visibility and other properties.</summary>
        protected virtual void BeforeQueryStatus(EventArgs e)
        {
            // Leave empty
        }
    }
}
