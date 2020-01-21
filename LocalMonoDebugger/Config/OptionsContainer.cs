using LocalMonoDebugger.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalMonoDebugger.Config
{
    public class OptionsContainer : BaseViewModel
    {
        public static OptionsContainer DeserializeFromJson(string j)
        {
            var obj = JsonConvert.DeserializeObject<OptionsContainer>(j);
            Validate(obj);
            return obj;
        }
        public string SerializeToJson()
            => JsonConvert.SerializeObject(this);
        
        public OptionsContainer()
        {
            Validate(this);
        }

        [JsonConstructor]
        public OptionsContainer(ObservableCollection<DebugOptions> profiles)
        {
            Profiles = profiles;
        }

        private string _selectedName;
        public string SelectedName
        {
            get => _selectedName;
            set
            {
                _selectedName = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Current));
            }
        }

        private ObservableCollection<DebugOptions> _profiles = new ObservableCollection<DebugOptions>();
        public ObservableCollection<DebugOptions> Profiles
        {
            get => _profiles;
            set
            {
                _profiles = value;
                Validate(this);
                NotifyPropertyChanged();
            }
        }


        private DebugOptions _current = null;

        [JsonIgnore]
        public DebugOptions Current
        {
            get
            {
                if (_current == null || _current.AppName != SelectedName)
                    _current = Profiles?.FirstOrDefault(x => x.AppName == SelectedName) ?? new DebugOptions(this);
                return _current;
            }
        }

        private static void Validate(OptionsContainer instance)
        {
            if (instance.Profiles == null)
                instance.Profiles = new ObservableCollection<DebugOptions>();
            if (instance.Profiles.Count < 1)
                instance.Profiles.Add(new DebugOptions(instance));
            if (string.IsNullOrWhiteSpace(instance.SelectedName) || !instance.Profiles.Any(x => x.AppName == instance.SelectedName))
                instance.SelectedName = instance.Profiles.First().AppName;
            foreach (var deviceConnection in instance.Profiles)
            {
                if (deviceConnection.DebugPort <= 0)
                    deviceConnection.DebugPort = DebugOptions.DefaultDebugPort;
            }
        }
    }
}
