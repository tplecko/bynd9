using System.Text.Json;

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
        internal readonly static Common.Configuration.Server conf = JsonSerializer.Deserialize<Common.Configuration.Server>(File.ReadAllText($@"config.json"))!;

        // Listener class
        internal readonly static Listener L = new();

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
    }
}
