using LocalMonoDebugger.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalMonoDebugger.Views
{
    public class OptionsModel : BaseViewModel
    {
        private OptionsContainer _container = OptionsManager.Instance.Load();
        public OptionsContainer Container
        {
            get => _container;
            set
            {
                _container = value;
                NotifyPropertyChanged();
            }
        }

        public void Save()
            => OptionsManager.Instance.Save(Container);

        public void LoadFrom(string path)
            => Container = OptionsManager.Instance.LoadFromPath(path);

        public void SaveTo(string path)
            => OptionsManager.Instance.SaveAs(Container, path);
    }
}
