using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows.Controls;

namespace LocalMonoDebugger.Views
{
    public class HostAddressValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!(value is string addr))
                return new ValidationResult(false, "Address must be a string");

            if (!IPAddress.TryParse(addr, out var _))
            {
                var ipaddr = Dns.GetHostAddresses(addr)
                    .Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    .FirstOrDefault();
                if (ipaddr == null)
                    return new ValidationResult(false, "Could not find IPv4 address for hostname");
                else
                    return new ValidationResult(true, null);
            }
            else
                return new ValidationResult(true, null);
        }
    }
}
