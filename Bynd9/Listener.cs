using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bynd9
{
    internal class Listener
    {

        internal Listener()
        {
            // HTTP
            HttpListener httpListener;
            httpListener = new() { AuthenticationSchemes = AuthenticationSchemes.Anonymous };
            httpListener.Prefixes.Add($"http://*:{C.conf.HttpPort}/");
            HttpListenerTimeoutManager timeoutManager = httpListener.TimeoutManager;
            timeoutManager.IdleConnection = new TimeSpan(0, 0, 5);
            try
            {
                httpListener.Start();
                ThreadPool.QueueUserWorkItem(HttpResponse, httpListener);
            }
            catch
            {
                Environment.Exit(1);
            }

            //HTTPS
            if (File.Exists(C.conf.CertificateFilePath))
            {
                HttpListener httpsListener;
                httpsListener = new() { AuthenticationSchemes = AuthenticationSchemes.Anonymous };
                httpsListener.Prefixes.Add($"https://*:{C.conf.HttpsPort}/");
                HttpListenerTimeoutManager timeoutManager1 = httpsListener.TimeoutManager;
                timeoutManager1.IdleConnection = new TimeSpan(0, 0, 5);
                try
                {
                    httpsListener.Start();
                    ThreadPool.QueueUserWorkItem(HttpsResponse, httpsListener);
                }
                catch
                {
                    Environment.Exit(1);
                }
            }
        }

        static void HttpResponse(object? obj)
        {
            HttpListener httpListener = (HttpListener)obj!;

            while (true)
            {
                HttpListenerContext context = httpListener.GetContext();
                if (context.Request.Url!.AbsolutePath == "/ip")
                {
                    context.Response.KeepAlive = true;
                    context.Response.StatusCode = 200;
                    context.Response.StatusDescription = "OK";
                    context.Response.Headers.Add("Content-Type", "text/html");
                    using StreamWriter writer = new(context.Response.OutputStream);
                    writer.Write(context.Request.RemoteEndPoint.Address.ToString());
                }
                else
                {

                }
                ThreadPool.QueueUserWorkItem(ProcessRequest, context);
            }
        }

        static void HttpsResponse(object? obj)
        {
            HttpListener httpListener = (HttpListener)obj!;
            X509Certificate2 certificate = new(C.conf.CertificateFilePath, C.conf.CertificatePassword);

            while (true)
            { // Untested -------------------
                HttpListenerContext context = httpListener.GetContext();
                SslStream sslStream = new(context.Request.InputStream, false);
                sslStream.AuthenticateAsServer(certificate, false, SslProtocols.Tls13, true);
                ThreadPool.QueueUserWorkItem(ProcessRequest, context);
            }
        }

        static void ProcessRequest(object? obj)
        {
            HttpListenerContext context = (HttpListenerContext)obj!;

            if (context.Request.Url!.AbsolutePath == C.conf.Path)
            {
                try
                {
                    string? hostNameAPIKey = context.Request.Headers.GetValues(C.conf.KeyFieldName).FirstOrDefault();
                    if (hostNameAPIKey != null)
                    {
                        if (C.hosts.TryGetValue(hostNameAPIKey, out string? hostName))
                        {
                            using StreamReader reader = new(context.Request.InputStream, context.Request.ContentEncoding);
                            C.PostData postData = JsonSerializer.Deserialize<C.PostData>(reader.ReadToEnd())!;

                            if (C.devices.Contains(postData.DeviceID))
                            {
                                if (postData.IP == "0.0.0.0")
                                    postData.IP = context.Request.RemoteEndPoint.Address.ToString();

                                string CurrentValue = File.Exists($"{hostName}.ip") ? File.ReadAllText($"{hostName}.ip") : "0.0.0.0";
                                C.ResponseData R = new();
                                if (CurrentValue != postData.IP)
                                {
                                    // --- This is currently not thread safe ---

                                    File.AppendAllText($"server.log", $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} => Updating zone file: {C.conf.ZoneFilePath}\n");
                                    bool returnValue = Bind.UpdateARecord(C.conf.ZoneFilePath, hostName, postData.IP) && Bind.IncreaseSerial(C.conf.ZoneFilePath);

                                    if (returnValue)
                                    {
                                        File.WriteAllText($"{hostName}.ip", postData.IP);

                                        C.RestartBIND9 = true;
                                        R.Status = 1; // Update succeeded
                                        File.AppendAllText($"{hostName}.history", $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} => {postData.IP}\n");
                                        Bynd9Notifier.Discord.Server.Send(C.conf.Discord, $"{hostName}.{C.conf.FQDNSuffix}", CurrentValue, postData.IP);
                                    }
                                    else
                                    {
                                        R.Status = -1; // Update failed
                                        File.AppendAllText($"{hostName}.history", $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} => Update failed\n");
                                    }
                                }
                                else
                                {
                                    R.Status = 0; // Update not needed
                                    File.AppendAllText($"{hostName}.history", $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} => Update not needed\n");
                                }
                                context.Response.KeepAlive = true;
                                context.Response.StatusCode = 200;
                                context.Response.StatusDescription = "OK";
                                context.Response.Headers.Add("Content-Type", "text/html");
                                using StreamWriter writer = new(context.Response.OutputStream);
                                writer.Write(R.Status.ToString());
                            }
                        }
                        else
                        {
                            File.AppendAllText($"server.log", $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} => Invalid DeviceID\n");
                        }
                    }
                    else
                    {
                        File.AppendAllText($"server.log", $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} => Invalid API key\n");
                    }
                }
                catch ( Exception ex ) 
                {
                    File.AppendAllText($"server.log", $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} => {ex.Message}\n");
                    context.Response.Close();
                }
            }
            context.Response.Close();
        }
    }
}
