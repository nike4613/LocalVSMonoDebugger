using LocalMonoDebugger.Config;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalMonoDebugger
{
    internal sealed partial class Commands
    {
        private void InstallMenu(OleMenuCommandService commandService)
        {
            /*AddMenuItem(commandService, CommandIds.cmdDeployAndDebugOverSSH, SetMenuTextAndVisibility, DeployAndDebugOverSSHClicked);
            AddMenuItem(commandService, CommandIds.cmdDeployOverSSH, SetMenuTextAndVisibility, DeployOverSSHClicked);
            AddMenuItem(commandService, CommandIds.cmdDebugOverSSH, SetMenuTextAndVisibility, DebugOverSSHClicked);
            AddMenuItem(commandService, CommandIds.cmdAttachToMonoDebuggerWithoutSSH, SetMenuTextAndVisibility, AttachToMonoDebuggerWithoutSSHClicked);
            AddMenuItem(commandService, CommandIds.cmdBuildProjectWithMDBFiles, CheckStartupProjects, BuildProjectWithMDBFilesClicked);

            AddMenuItem(commandService, CommandIds.cmdOpenLogFile, CheckOpenLogFile, OpenLogFile);
            AddMenuItem(commandService, CommandIds.cmdOpenDebugSettings, null, OpenSSHDebugConfigDlg);*/
            AddMenuItem(commandService, Ids.CmdOpenOptions, null, OpenOptionsClicked);
        }

        private void OpenOptionsClicked(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
            string title = "OpenDebugOption";

            // Show a message box to prove we were here
            VsShellUtilities.ShowMessageBox(
                package,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        private void SetCommandText(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var menuCommand = sender as OleMenuCommand;
            if (menuCommand != null)
            {
                var allDeviceSettings = OptionsManager.Instance.Load();
                var settings = allDeviceSettings.Current;
                /*if (menuCommand.CommandID.ID == Ids.CmdOpenOptions)
                {
                    menuCommand.Text = $"{GetMenuText(menuCommand.CommandID.ID)} (TCP {settings.SSHHostIP})";
                }
                else if (settings.DeployAndDebugOnLocalWindowsSystem)
                {
                    menuCommand.Text = $"{GetMenuText(menuCommand.CommandID.ID)} (Local {settings.SSHHostIP})";
                }
                else
                {
                    menuCommand.Text = $"{GetMenuText(menuCommand.CommandID.ID)} (SSH {settings.SSHHostIP})";
                }*/
            }
        }
    }
}
