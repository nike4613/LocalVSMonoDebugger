using LocalMonoDebugger.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LocalMonoDebugger.Config
{
    public class DebugOptions : BaseViewModel
    {
        public static DebugOptions DeserializeFromJson(string j)
            => JsonConvert.DeserializeObject<DebugOptions>(j);
        public string SerializeToJson()
            => JsonConvert.SerializeObject(this);

        private string _appName = "Mono Application";
        public string AppName
        {
            get => _appName;
            set 
            {
                _appName = value;
                NotifyPropertyChanged();
            }
        }

        private string _hostAddress = "localhost";
        public string HostAddress
        {
            get => _hostAddress;
            set
            {
                _hostIpAddress = null;
                _hostAddress = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(HostIPAddress));
            }
        }


        private IPAddress _hostIpAddress = null;
        [JsonIgnore]
        public IPAddress HostIPAddress
            => _hostIpAddress ?? (_hostIpAddress = GetHostIpAddress());
        private IPAddress GetHostIpAddress()
        {
            if (!IPAddress.TryParse(HostAddress, out var hostIp))
            {
                return Dns.GetHostAddresses(HostAddress)
                    .Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    .FirstOrDefault() ?? IPAddress.Loopback;
            }
            else
                return hostIp;
        }

        public const int DefaultDebugPort = 5000;
        private int _debugPort = DefaultDebugPort;
        public int DebugPort 
        {
            get => _debugPort;
            set
            {
                _debugPort = value;
                NotifyPropertyChanged();
            }
        }
        private bool _runAsDebugServer = false;
        public bool RunAsDebugServer
        {
            get => _runAsDebugServer;
            set
            {
                _runAsDebugServer = value;
                NotifyPropertyChanged();
            }
        }

        private int _timeBetwConnAttemptsMs = 500;
        public int TimeBetweenConnectionAttemptsMs
        {
            get => _timeBetwConnAttemptsMs;
            set
            {
                _timeBetwConnAttemptsMs = value;
                NotifyPropertyChanged();
            }
        }
        private int _maxConnAttempts = 3;
        public int MaxConnectionAttempts
        {
            get => _maxConnAttempts;
            set
            {
                _maxConnAttempts = value;
                NotifyPropertyChanged();
            }
        }
    }
}
