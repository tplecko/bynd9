using System.Text.Json;

namespace Bynd9Client
{
    internal class C
    {
        // Configuration
        internal readonly static Configuration conf = JsonSerializer.Deserialize<Configuration>(File.ReadAllText($@"config.json"))!;

        internal static string TS { get { return $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}"; } }

        public class Configuration
        {
            public string Server { get; set; } = "127.0.0.1";
            public int Port { get; set; } = 23432;
            public string Path { get; set; } = "/updater/post";
            public string KeyFieldName { get; set; } = "API-key";
            public string KeyFieldValue { get; set; } = "asdfasdfASDFASDF";
            public string DeviceID { get; set; } = "Gateway";
            public string Discord { get; set; } = string.Empty;
            public string TelegramUser { get; set; } = string.Empty;
            public int Interval { get; set; } = 30;
        }
    }
}
