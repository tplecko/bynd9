using System.Text.Json;

namespace Bynd9Client
{
    internal class C
    {
        // Configuration
        internal readonly static Common.Configuration.Client conf = JsonSerializer.Deserialize<Common.Configuration.Client>(File.ReadAllText($@"config.json"))!;

        internal static string TS { get { return $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}"; } }


    }
}
