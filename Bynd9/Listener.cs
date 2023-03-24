using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bynd9
{
    internal class Listener
    {

        internal Listener()
        {

            HttpListener httpListener;
            httpListener = new() { AuthenticationSchemes = AuthenticationSchemes.Anonymous };
            httpListener.Prefixes.Add($"http://*:{C.conf.Port}/");
            HttpListenerTimeoutManager timeoutManager = httpListener.TimeoutManager;
            timeoutManager.IdleConnection = new TimeSpan(0, 0, 5);
            try
            {
                httpListener.Start();
                ThreadPool.QueueUserWorkItem(WaitForIncomingConnection, httpListener);
            }
            catch
            {
                Environment.Exit(1);
            }
        }

        static void WaitForIncomingConnection(Object? obj)
        {
            HttpListener httpListener = (HttpListener)obj!;

            while (true)
            {
                HttpListenerContext context = httpListener.GetContext();

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

                                        bool returnValue = Bind.UpdateARecord(C.conf.ZoneFilePath, hostName, postData.IP);

                                        if (returnValue)
                                        {
                                            File.WriteAllText($"{hostName}.ip", postData.IP);

                                            C.RestartBIND9 = true;
                                            R.Status = 1; // Update succeeded
                                        }
                                        else
                                        {
                                            R.Status = -1; // Update failed
                                        }
                                        context.Response.Headers.Add("Content-Type", "application/json");
                                    }
                                    else
                                    {
                                        R.Status = 0; // Update not needed
                                    }
                                    context.Response.KeepAlive = true;
                                    context.Response.StatusCode = 200;
                                    context.Response.StatusDescription = "OK";
                                    context.Response.Headers.Add("Content-Type", "application/json");
                                    using StreamWriter writer = new(context.Response.OutputStream);
                                    writer.Write(JsonSerializer.Serialize(R));
                                }
                            }
                        }
                    }
                    catch
                    {
                        context.Response.Close();
                    }
                }
                context.Response.Close();
            }
        }
    }
}
