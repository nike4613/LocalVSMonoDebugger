using LocalMonoDebugger.Config;
using LocalMonoDebugger.Services;
using LocalMonoDebugger.Views;
using Microsoft;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            AddMenuItem(commandService, Ids.CmdAttachDebugger, SetCommandText, AttachDebugger);
            AddMenuItem(commandService, Ids.CmdOpenOptions, null, OpenOptionsClicked);
        }

        private void AttachDebugger(object sender, EventArgs args)
        {
            _ = AttachDebuggerAsync();
        }
        private async System.Threading.Tasks.Task AttachDebuggerAsync()
        {
            // TODO error handling
            // TODO show ssh output stream
            // TODO stop monoRemoteSshDebugTask properly
            try
            {
                NLogService.Logger.Info($"===== {nameof(AttachDebuggerAsync)} =====");

                if (!monoExtension.IsStartupProjectAvailable())
                {
                    NLogService.Logger.Info($"No startup project/solution loaded yet.");
                    return;
                }

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var allDeviceSettings = OptionsManager.Instance.Load();
                var options = allDeviceSettings.Current;


                monoExtension.AttachDebuggerToRunningProcess(options);

                return;
            }
            catch (Exception ex)
            {
                var outputWindow = (IVsOutputWindow)Package.GetGlobalService(typeof(SVsOutputWindow));
                if (outputWindow == null)
                    return;

                Guid guidDebugOutputPane = VSConstants.GUID_OutWindowDebugPane;
                var hr = outputWindow.GetPane(ref guidDebugOutputPane, out var pane);
                if (hr < 0)
                    return;

                hr = pane.OutputString(ex.Message);
                pane.Activate(); // Brings this pane into view

                NLogService.Logger.Error(ex);
                MessageBox.Show(ex.Message, "MonoDebugger", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void OpenOptionsClicked(object sender, EventArgs e)
        {
            _ = OpenOptionsClickedAsync();
        }
        private async System.Threading.Tasks.Task OpenOptionsClickedAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            // https://docs.microsoft.com/en-us/visualstudio/extensibility/creating-and-managing-modal-dialog-boxes?view=vs-2019
            var vsUIShell = await package.GetServiceAsync(typeof(SVsUIShell)) as IVsUIShell;
            Assumes.Present(vsUIShell);

            var dlg = new OptionsWindow();
            vsUIShell.GetDialogOwnerHwnd(out IntPtr vsParentHwnd);
            vsUIShell.EnableModeless(0);
            try
            {
                WindowHelper.ShowModal(dlg, vsParentHwnd);
            }
            finally
            {
                vsUIShell.EnableModeless(1);
            }
        }

        private void SetCommandText(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (sender is OleMenuCommand menuCommand)
            {
                var allDeviceSettings = OptionsManager.Instance.Load();
                var settings = allDeviceSettings.Current;
                if (menuCommand.CommandID.ID == Ids.CmdAttachDebugger)
                {
                    menuCommand.Text = $"Start debugger ({settings.HostAddress}:{settings.DebugPort})";
                }

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
