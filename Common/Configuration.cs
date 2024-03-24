namespace Common
{
    public class Configuration
    {
        public class Server
        {
            public int Interval { get; set; } = 60;
            public int HttpPort { get; set; } = 23432;
            public string ServerIP { get; set; } = string.Empty;
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
            public string DiscordAvatarURL { get; set; } = "/discord/avatar.jpg";
            public string DiscordIconURL { get; set; } = string.Empty;
            public string TelegramUser { get; set; } = string.Empty;
            public string WhatsappNumber { get; set; } = string.Empty;
            public string WhatsappKey { get; set; } = string.Empty;
            public string FQDNSuffix { get; set; } = "example.com";

        }

        public class Client
        {
            public string Server { get; set; } = "127.0.0.1";
            public int Port { get; set; } = 23432;
            public string Path { get; set; } = "/updater/post";
            public string KeyFieldName { get; set; } = "API-key";
            public string KeyFieldValue { get; set; } = "asdfasdfASDFASDF";
            public string DeviceID { get; set; } = "Gateway";
            public string Discord { get; set; } = string.Empty;
            public string DiscordAvatarURL { get; set; } = string.Empty;
            public string DiscordIconURL { get; set; } = string.Empty;
            public string TelegramUser { get; set; } = string.Empty;
            public string WhatsappNumber { get; set; } = string.Empty;
            public string WhatsappKey { get; set; } = string.Empty;
            public int Interval { get; set; } = 30;
        }
    }
}
