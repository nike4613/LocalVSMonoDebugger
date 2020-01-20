using LocalMonoDebugger.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;

namespace LocalMonoDebugger.Views
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsWindow()
        {
            InitializeComponent();

            DataContext = Model;
        }

        public OptionsModel Model { get; set; } = new OptionsModel();

        private void Clone(object source, EventArgs args)
        {
            var container = Model.Container;

            var current = container.Current;
            var cloned = new DebugOptions(container)
            {
                HostAddress = current.HostAddress,
                DebugPort = current.DebugPort,
                RunAsDebugServer = current.RunAsDebugServer,
                MaxConnectionAttempts = current.MaxConnectionAttempts,
                TimeBetweenConnectionAttemptsMs = current.TimeBetweenConnectionAttemptsMs
            };

            var idx = 1;
            var name = current.AppName + " Clone  ";
            while (container.Profiles.Any(p => p.AppName == name.Trim()))
                name = $"{name.Substring(0, name.Length-(idx.ToString().Length)).Trim()} {idx++}";

            cloned.AppName = name.Trim();
            container.Profiles.Add(cloned);
            container.SelectedName = cloned.AppName;
        }
        private void Delete(object source, EventArgs args)
        {
            var container = Model.Container;

            container.Profiles.Remove(container.Current);
            container.Profiles = container.Profiles; // re-verify
        }

        private void Save(object source, EventArgs args)
        {
            Model.Save();
            DialogResult = true;
        }
        private void Cancel(object source, EventArgs args)
        {
            DialogResult = false;
        }

        private const string DialogFilter 
            = "Settings.MonoDebugger json files (*.MonoDebugger.json)|*.MonoDebugger.json|JSON files (*.json)|*.json|All files (*.*)|*.*";
        private void SaveAs(object source, EventArgs args)
        {
            var dialog = new SaveFileDialog
            {
                FileName = "Settings.MonoDebugger.json",
                Filter = DialogFilter
            };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Model.SaveTo(dialog.FileName);
        }
        private void LoadFrom(object source, EventArgs args)
        {
            var dialog = new OpenFileDialog
            {
                Filter = DialogFilter
            };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Model.LoadFrom(dialog.FileName);
        }
    }
}
