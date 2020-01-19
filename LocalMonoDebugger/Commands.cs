using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalMonoDebugger
{
    internal sealed partial class Commands
    {
        #region IDs from LocalMonoDebuggerPackage.vsct
        public static class Ids
        {
            public const int DebugMenuGroup = 0x1020;
            public const int DebugMenu = 0x1030;
            public const int CmdOpenOptions = 0x0100;
        }
        #endregion

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("146714cc-ce59-4946-9931-facafde0fee5");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="VSMonoDebuggerCommands"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="asyncServiceProvider">Owner package, not null.</param>
        public Commands(AsyncPackage asyncServiceProvider, OleMenuCommandService menuCommandService)
        {
            package = asyncServiceProvider ?? throw new ArgumentNullException(nameof(asyncServiceProvider));

            InstallMenu(menuCommandService);
        }

        private OleMenuCommand AddMenuItem(OleMenuCommandService mcs, int cmdCode, EventHandler check, EventHandler action)
        {
            var commandID = new CommandID(CommandSet, cmdCode);
            var menuCommand = new OleMenuCommand(action, commandID);
            if (check != null)
            {
                menuCommand.BeforeQueryStatus += check;
            }
            mcs.AddCommand(menuCommand);

            return menuCommand;
        }
    }
}
