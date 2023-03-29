using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bynd9Client
{
    internal class C
    {
        // Configuration
        internal readonly static Configuration conf = JsonSerializer.Deserialize<Configuration>(File.ReadAllText($@"config.json"))!;

        public class Configuration
        {
            public string Server { get; set; } = "127.0.0.1";
            public int Port { get; set; } = 23432;
            public string Path { get; set; } = "/updater/post";
            public string KeyFieldName { get; set; } = "API-key";
            public string KeyFieldValue { get; set; } = "asdfasdfASDFASDF";
            public string DeviceID { get; set; } = "Gateway";
            public string Discord { get; set; } = string.Empty;
        }
    }
}
