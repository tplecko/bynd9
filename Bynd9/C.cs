using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bynd9
{
    internal class C
    {
        // Bind restart flag
        internal static bool RestartBIND9 = false;

        // Hosts dictionary
        internal readonly static Dictionary<string, string> hosts = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText($@"hosts.json"))!;

        // Devices dictionary
        internal readonly static List<string> devices = JsonSerializer.Deserialize<List<string>>(File.ReadAllText($@"devices.json"))!;

        // Configuration
        internal readonly static Configuration conf = JsonSerializer.Deserialize<Configuration>(File.ReadAllText($@"config.json"))!;

        internal static string TS { get { return $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}"; } }

        public class PostData
        {
            public string IP { get; set; } = string.Empty;
            public string DeviceID { get; set; } = string.Empty;
        };
        public class ResponseData
        {
            public int Status { get; set; }
        };

        public class Configuration
        {
            public int Interval { get; set; } = 60;
            public int HttpPort { get; set; } = 23432;
            public int HttpsPort { get; set; } = 23433;
            public string Path { get; set; } = "/updater/post";
            public string KeyFieldName { get; set; } = "API-key";
            public string ZoneFilePath { get; set; } = "/etc/bind/example.com.zone";
            public string ActiveString { get; set; } = "(running)";
            public string InactiveString { get; set; } = "(dead)";
            public int FieldIndex { get; set; } = 2;
            public string Bind9ServiceName { get; set; } = "bind9";
            public string CertificateFilePath { get; set; } = "server.crt";
            public string CertificatePassword { get; set; } = "password";
            public string Discord { get; set; } = string.Empty;
            public string TelegramUser { get; set; } = string.Empty;
            public string FQDNSuffix { get; set; } = "example.com";
        }
    }
}
