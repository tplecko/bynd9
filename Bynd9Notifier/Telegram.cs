﻿using System.Text;

namespace Bynd9Notifier
{
    public class Telegram
    {
        public class Client
        {
            public static void Init(string user)
            {
                if (user.Length > 0)
                {
                    using HttpClient client = new();

                    using var httpContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                    using HttpResponseMessage response = client.PostAsync($"https://api.callmebot.com/text.php?user={user}&text=Bynd9Client\nClient startup: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}", httpContent).Result;
                }
            }
            public static void Send(string user, string device, string oldIP, string newIP, string server)
            {
                if (user.Length > 0)
                {
                    using HttpClient client = new();

                    using var httpContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                    using HttpResponseMessage response = client.PostAsync($"https://api.callmebot.com/text.php?user={user}&text=Bynd9Client\nTimestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}\nDevice: {device}\nOld IP address: {oldIP}\nNew IP address: {newIP}\nServer: {server}", httpContent).Result;

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
            public static void Init(string user)
            {
                if (user.Length > 0)
                {
                    using HttpClient client = new();

                    using var httpContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                    using HttpResponseMessage response = client.PostAsync($"https://api.callmebot.com/text.php?user={user}&text=Bynd9\nServer startup: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}", httpContent).Result;
                }
            }
            public static void Send(string user, string fqdn, string oldIP, string newIP)
            {
                if (user.Length > 0)
                {
                    using HttpClient client = new();

                    using var httpContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                    using HttpResponseMessage response = client.PostAsync($"https://api.callmebot.com/text.php?user={user}&text=Bynd9\nTimestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}\nFQDN: {fqdn}\nOld IP address: {oldIP}\nNew IP address: {newIP}", httpContent).Result;

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
            public static void SendError(string user, string err)
            {
                if (user.Length > 0)
                {
                    using HttpClient client = new();

                    using var httpContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                    using HttpResponseMessage response = client.PostAsync($"https://api.callmebot.com/text.php?user={user}&text=Bynd9\nTimestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}\nError: {err}", httpContent).Result;
                }
            }
        }
    }
}
