using System;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Community.VisualStudio.Toolkit.DependencyInjection.Core
{
    internal class CommandWrapper<T>
        where T : BaseDICommand
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandWrapper(IServiceProvider serviceProvider, AsyncPackage package)
        {
            this._serviceProvider = serviceProvider;

            CommandAttribute? attr = (CommandAttribute)typeof(T).GetCustomAttributes(typeof(CommandAttribute), true).FirstOrDefault();

            if (attr is null)
            {
                throw new InvalidOperationException($"No [Command(GUID, ID)] attribute was added to {typeof(T).Name}");
            }

            // Use package GUID if no command set GUID has been specified
            Guid cmdGuid = attr.Guid == Guid.Empty ? package.GetType().GUID : attr.Guid;
            CommandID cmd = new(cmdGuid, attr.Id);

            this.Command = new OleMenuCommand(this.Execute, changeHandler: null, this.BeforeQueryStatus, cmd);

            ThreadHelper.ThrowIfNotOnUIThread();

            IMenuCommandService commandService = serviceProvider.GetRequiredService<IMenuCommandService>();
            commandService.AddCommand(this.Command);  // Requires main/UI thread
        }

        /// <summary>
        /// The command object associated with the command ID (GUID/ID).
        /// </summary>
        public OleMenuCommand Command { get; }

        protected void BeforeQueryStatus(object sender, EventArgs e)
        {
            using var scope = this._serviceProvider.CreateScope();
            var instance = (BaseDICommand)scope.ServiceProvider.GetRequiredService(typeof(T));
            instance.BeforeQueryStatus(sender, e);
        }

        protected void Execute(object sender, EventArgs e)
        {
            using var scope = this._serviceProvider.CreateScope();
            var instance = (BaseDICommand)scope.ServiceProvider.GetRequiredService(typeof(T));
            instance.Execute(sender, e);
        }

        protected async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            using var scope = this._serviceProvider.CreateScope();
            var instance = (BaseDICommand)scope.ServiceProvider.GetRequiredService(typeof(T));
            await instance.ExecuteAsync(e);
        }
    }
}
