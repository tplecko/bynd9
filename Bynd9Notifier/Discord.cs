using System.Text;

namespace Bynd9Notifier
{
    public class Discord
    {
        public class Client
        {
            public static void Send(string url, string device, string oldIP, string newIP, string server)
            {
                if (url.Length > 0)
                {
                    using HttpClient client = new();

                    string requestBody =
                        $"{{\"embeds\": [" +
                            $"{{\"title\": \"Bynd9Client\", \"color\": \"14177041\", \"description\": \"Device `{device}` reporting in :wave:\", \"url\": \"https://github.com/tplecko/bynd9\"}}," +

                            $"{{\"fields\": [" +
                                $"{{\"name\": \"Timestamp\", \"value\": \"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}\"}}," +
                                $"{{\"name\": \"Old IP address\", \"value\": \"{oldIP}\", \"inline\": true}}," +
                                $"{{\"name\": \"New IP address\", \"value\": \"{newIP}\", \"inline\": true}}" +
                            $"]}}," +
                            $"{{\"footer\": {{\"text\": \"Using server {server}\"}}}}" +
                        $"]}}";

                    using var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    using HttpResponseMessage response = client.PostAsync(url, httpContent).Result;

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
            public static void Send(string url, string fqdn, string oldIP, string newIP)
            {
                if (url.Length > 0)
                {
                    using HttpClient client = new();

                    string requestBody =
                        $"{{\"embeds\": [" +
                            $"{{\"title\": \"Bynd9\", \"color\": \"9240320\", \"description\": \"Server reporting in :wave:\", \"url\": \"https://github.com/tplecko/bynd9\"}}," +

                            $"{{\"fields\": [" +
                                $"{{\"name\": \"Timestamp\", \"value\": \"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}\"}}," +
                                $"{{\"name\": \"Hostname\", \"value\": \"{fqdn}\", \"inline\": false}}," +
                                $"{{\"name\": \"Old IP address\", \"value\": \"{oldIP}\", \"inline\": true}}," +
                                $"{{\"name\": \"New IP address\", \"value\": \"{newIP}\", \"inline\": true}}" +
                            $"]}}" +
                        $"]}}";

                    using var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    using HttpResponseMessage response = client.PostAsync(url, httpContent).Result;

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
    }
}