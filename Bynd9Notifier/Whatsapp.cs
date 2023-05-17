using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bynd9Notifier
{
    public class Whatsapp
    {
        public class Client
        {
            public static void Send(string number, string key, string device, string oldIP, string newIP, string server)
            {
                if (number.Length > 0 && key.Length > 0)
                {
                    using HttpClient client = new();

                    using var httpContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                    using HttpResponseMessage response = client.PostAsync($"\"https://api.callmebot.com/whatsapp.php?phone={number}&text=Bynd9Client\nTimestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}\nDevice: {device}\nOld IP address: {oldIP}\nNew IP address: {newIP}\nServer: {server}&apikey={key}\"", httpContent).Result;

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
            public static void Send(string number, string key, string fqdn, string oldIP, string newIP)
            {
                if (number.Length > 0 && key.Length > 0)
                {
                    using HttpClient client = new();

                    using var httpContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                    using HttpResponseMessage response = client.PostAsync($"\"https://api.callmebot.com/whatsapp.php?phone={number}&text=Bynd9\nTimestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}\nFQDN: {fqdn}\nOld IP address: {oldIP}\nNew IP address: {newIP}&apikey={key}\"", httpContent).Result;

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
