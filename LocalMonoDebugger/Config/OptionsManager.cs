using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalMonoDebugger.Config
{
    public class OptionsManager
    {
        public readonly static string SETTINGS_STORE_NAME = "VSMonoDebugger";

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private WritableSettingsStore _settingsStore;

        private OptionsManager()
        {
        }

        public OptionsContainer Load()
        {
            var result = new OptionsContainer();

            if (_settingsStore.CollectionExists(SETTINGS_STORE_NAME))
            {
                try
                {
                    var content = _settingsStore.GetString(SETTINGS_STORE_NAME, "Settings");
                    result = OptionsContainer.DeserializeFromJson(content);
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            }

            return result;
        }

        public void Save(OptionsContainer settings)
        {
            var json = settings.SerializeToJson();
            if (!_settingsStore.CollectionExists(SETTINGS_STORE_NAME))
            {
                _settingsStore.CreateCollection(SETTINGS_STORE_NAME);
            }
            _settingsStore.SetString(SETTINGS_STORE_NAME, "Settings", json);
        }

        public void SaveAs(OptionsContainer settings, string path)
        {
            var json = settings.SerializeToJson();
            File.WriteAllText(path, json);
        }

        public OptionsContainer LoadFromPath(string path)
        {
            var result = new OptionsContainer();

            try
            {
                var content = File.ReadAllText(path);
                result = OptionsContainer.DeserializeFromJson(content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return result;
        }

        public static OptionsManager Instance { get; } = new OptionsManager();

        public static void Initialize(Package package)
        {
            if (package != null)
            {
                var settingsManager = new ShellSettingsManager(package);
                var configurationSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
                Instance._settingsStore = configurationSettingsStore;
            }
            else
            {
                throw new ArgumentNullException("package argument was null");
            }
        }
    }
}
