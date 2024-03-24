using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace Bynd9Notifier
{
    public class Discord
    {
        public class Client
        {
            public static void Init(Common.Configuration.Client C)
            {
                if (C.Discord.Length > 0)
                {
                    using HttpClient client = new();

                    DiscordMessage M = new()
                    {
                        username = "Bynd9 Client",
                        avatar_url = C.DiscordAvatarURL,
                        embeds =
                        [
                            new Embeds() {
                                author = new() {
                                    name = "Bynd9 Client",
                                    icon_url = C.DiscordIconURL
                                },
                                title = "source",
                                url = "https://github.com/tplecko/bynd9",
                                description = "Client checking in :wave:",
                                color = "9240320",
                                fields =
                                [
                                    new() {
                                        name = "Client startup",
                                        value = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                                        inline = false}
                                ]
                            }
                        ]
                    };

                    using var httpContent = new StringContent(JsonSerializer.Serialize(M), Encoding.UTF8, "application/json");
                    using HttpResponseMessage response = client.PostAsync(C.Discord, httpContent).Result;
                }
            }
            public static void Send(Common.Configuration.Client C, string device, string oldIP, string newIP, string server)
            {
                if (C.Discord.Length > 0)
                {
                    using HttpClient client = new();

                    DiscordMessage M = new()
                    {
                        username = "Bynd9 Client",
                        avatar_url = C.DiscordAvatarURL,
                        embeds =
                        [
                            new Embeds() {
                                author = new() {
                                    name = "Bynd9 Client",
                                    icon_url = C.DiscordIconURL
                                },
                                title = "source",
                                url = "https://github.com/tplecko/bynd9",
                                description = $"Client `{device}` sending update :wave:",
                                color = "9240320",
                                fields =
                                [
                                    new() {
                                        name = "Timestamp",
                                        value = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                                        inline = false},
                                    new() {
                                        name = "Old IP address",
                                        value = oldIP,
                                        inline = true},
                                    new() {
                                        name = "New IP address",
                                        value = newIP,
                                        inline = true}
                                ],
                                footer = new() {
                                    text = $"Using server {server}"
                                }
                            }
                        ]
                    };

                    using var httpContent = new StringContent(JsonSerializer.Serialize(M), Encoding.UTF8, "application/json");
                    using HttpResponseMessage response = client.PostAsync(C.Discord, httpContent).Result;

                    //if (response.IsSuccessStatusCode)
                    //{
                    //    string responseBody = response.Content.ReadAsStringAsync().Result;
                    //    Console.WriteLine(responseBody);
                    //}
                    //else
                    //{
                    //    //Console.WriteLine("err");
                    //}
                }
            }
        }
        public class Server
        {
            public static void Init(Common.Configuration.Server C)
            {
                if (C.Discord.Length > 0)
                {
                    using HttpClient client = new();

                    DiscordMessage M = new()
                    {
                        username = "Bynd9 Server",
                        avatar_url = C.DiscordAvatarURL,
                        embeds =
                        [
                            new Embeds() {
                                author = new() {
                                    name = "Bynd9 Server",
                                    icon_url = C.DiscordIconURL
                                },
                                title = "source",
                                url = "https://github.com/tplecko/bynd9",
                                description = "Server checking in :wave:",
                                color = "9240320",
                                fields =
                                [
                                    new() {
                                        name = "Server startup",
                                        value = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                                        inline = false}
                                ]
                            }
                        ]
                    };

                    using var httpContent = new StringContent(JsonSerializer.Serialize(M), Encoding.UTF8, "application/json");
                    using HttpResponseMessage response = client.PostAsync(C.Discord, httpContent).Result;
                }
            }
            public static void Send(Common.Configuration.Server C, string fqdn, string oldIP, string newIP)
            {
                if (C.Discord.Length > 0)
                {
                    using HttpClient client = new();

                    DiscordMessage M = new()
                    {
                        username = "Bynd9 Server",
                        avatar_url = C.DiscordAvatarURL,
                        embeds =
                        [
                            new Embeds() {
                                author = new() {
                                    name = "Bynd9 Server",
                                    icon_url = C.DiscordIconURL
                                },
                                title = "source",
                                url = "https://github.com/tplecko/bynd9",
                                description = "Server sending update :wave:",
                                color = "9240320",
                                fields =
                                [
                                    new() {
                                        name = "Timestamp",
                                        value = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                                        inline = false},
                                    new() {
                                        name = "Hostname",
                                        value = fqdn,
                                        inline = false},
                                    new() {
                                        name = "Old IP address",
                                        value = oldIP,
                                        inline = true},
                                    new() {
                                        name = "New IP address",
                                        value = newIP,
                                        inline = true}
                                ]
                            }
                        ]
                    };

                    using var httpContent = new StringContent(JsonSerializer.Serialize(M), Encoding.UTF8, "application/json");
                    using HttpResponseMessage response = client.PostAsync(C.Discord, httpContent).Result;
                }
            }
            
            public static void SendError(Common.Configuration.Server C, string err)
            {
                if (C.Discord.Length > 0)
                {
                    using HttpClient client = new();

                    DiscordMessage M = new()
                    {
                        username = "Bynd9 Server",
                        avatar_url = C.DiscordAvatarURL,
                        embeds =
                        [
                            new Embeds() {
                                author = new() {
                                    name = "Bynd9 Server",
                                    icon_url = C.DiscordIconURL
                                },
                                title = "source",
                                url = "https://github.com/tplecko/bynd9",
                                description = "Server reporting an error :bangbang:",
                                color = "16711680",
                                fields =
                                [
                                    new() {
                                        name = "Timestamp",
                                        value = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                                        inline = false},
                                    new() {
                                        name = "Error",
                                        value = err,
                                        inline = false}
                                ]
                            }
                        ]
                    };

                    using var httpContent = new StringContent(JsonSerializer.Serialize(M), Encoding.UTF8, "application/json");
                    using HttpResponseMessage response = client.PostAsync(C.Discord, httpContent).Result;
                }
            }
        }

#pragma warning disable IDE1006 // Naming Styles
        public class DiscordMessage
        {
            public string username { get; set; } = string.Empty; // Username instead of webhook name
            public string avatar_url { get; set; } = string.Empty; // Avatar instead of webhook avatar
            public string content { get; set; } = string.Empty; // Text up to 2000 characters
            public Embeds[]? embeds { get; set; }
        }

        public class Embeds
        {
            public Author? author { get; set; }
            public string title { get; set; } = string.Empty;
            public string url { get; set; } = string.Empty; // If title is provided, it becomes a hyperlink
            public string description { get; set; } = string.Empty; // Text message. You can use Markdown here. *Italic* **bold** __underline__ ~~strikeout~~ [hyperlink](https://google.com) `code
            public string color { get; set; } = string.Empty; // Decimal value
            public Fields[]? fields { get; set; }
            public Thumbnail? thumbnail { get; set; }
            public Image? image { get; set; }
            public Footer? footer { get; set; }
        }

        public class Author
        {
            public string name { get; set; } = string.Empty;
            public string url { get; set; } = string.Empty;
            public string icon_url { get; set; } = string.Empty;
        }

        public class Fields
        {
            public string name { get; set; } = string.Empty;
            public string value { get; set; } = string.Empty;
            public bool inline { get; set; } = false;
        }
        public class Thumbnail
        {
            public string url {  get; set; } = string.Empty;
        }
        public class Image
        {
            public string url { get; set; } = string.Empty;

        }
        public class Footer
        {
            public string text { get; set; } = string.Empty;
            public string icon_url { get; set; } = string.Empty;

        }
 #pragma warning restore IDE1006 // Naming Styles

    }
}