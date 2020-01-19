using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

            Model = new OptionsModel();
            DataContext = Model;
        }

        public OptionsModel Model { get; set; }

        private void Clone(object source, EventArgs args)
        {
            throw new NotImplementedException();
        }
        private void Delete(object source, EventArgs args)
        {
            throw new NotImplementedException();
        }

        private void Save(object source, EventArgs args)
        {
            throw new NotImplementedException();
        }
        private void Cancel(object source, EventArgs args)
        {
            throw new NotImplementedException();
        }
        private void SaveAs(object source, EventArgs args)
        {
            throw new NotImplementedException();
        }
        private void LoadFrom(object source, EventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
