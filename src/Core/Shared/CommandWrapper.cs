using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Community.VisualStudio.Toolkit.DependencyInjection.Core
{
    internal class CommandWrapper<T>
        where T : BaseDICommand
    {
        private static readonly MethodInfo _beforeQueryStatusMethod = typeof(BaseDICommand).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).First(x => x.Name == "BeforeQueryStatus" && x.GetParameters().Count() == 2);
        private static readonly MethodInfo _executeMethod = typeof(BaseDICommand).GetMethod("Execute", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo _executeAsyncMethod = typeof(BaseDICommand).GetMethod("ExecuteAsync", BindingFlags.Instance | BindingFlags.NonPublic);

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
            BaseDICommand instance = (BaseDICommand)scope.ServiceProvider.GetRequiredService(typeof(T));
            _beforeQueryStatusMethod.Invoke(instance, new object[] { sender, e });
        }

        protected void Execute(object sender, EventArgs e)
        {
            using var scope = this._serviceProvider.CreateScope();
            BaseDICommand instance = (BaseDICommand)scope.ServiceProvider.GetRequiredService(typeof(T));
            _executeMethod.Invoke(instance, new object[] { sender, e });
        }

        protected async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            using var scope = this._serviceProvider.CreateScope();
            BaseDICommand instance = (BaseDICommand)scope.ServiceProvider.GetRequiredService(typeof(T));
            Task executeAsyncTask = (Task)_executeAsyncMethod.Invoke(instance, new object[] { e });
            await executeAsyncTask;
        }
    }
}
