using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Bynd9
{
    public class Worker : BackgroundService
    {
        public readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("{time} => Waiting for tasks...", DateTimeOffset.Now);

                var task1 = HandleHttpListenerAsync(stoppingToken);
                var task2 = HandleHttpsListenerAsync(stoppingToken);

                await Task.WhenAll(task1, task2);
                _logger.LogInformation("{time} => Tasks complete!", DateTimeOffset.Now);

            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{time} => Stoppint worker...", DateTimeOffset.Now);
            if (!C.L.StopListener(out string err))
            {
                _logger.LogInformation("{time} => Error stopping worker...", DateTimeOffset.Now);
                Bynd9Notifier.Discord.Server.SendError(C.conf.Discord, err);
                Bynd9Notifier.Telegram.Server.SendError(C.conf.TelegramUser, err);
                Bynd9Notifier.Whatsapp.Server.SendError(C.conf.WhatsappNumber, C.conf.WhatsappKey, err);
            }
            _logger.LogInformation("{time} => Worker stopped...", DateTimeOffset.Now);

            await base.StopAsync(cancellationToken);
        }


        async Task HandleHttpListenerAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("{time} => Waiting for http context", DateTimeOffset.Now);
                    var context = await C.L.httpListener!.GetContextAsync();
                    _ = HandleHttpRequestAsync(context, cancellationToken);
                }
            }
            catch (HttpListenerException ex) when (ex.ErrorCode == 995) // Ignoring the "The I/O operation has been aborted because of either a thread exit or an application request" error.
            {
                // Ignore this exception since it's thrown when stopping the listener.
            }
            catch (OperationCanceledException)
            {
                // Ignore this exception since it's thrown when cancellationToken is canceled.
            }
            finally
            {
                C.L.httpListener!.Close();
                _logger.LogInformation("{time} => Http listener closed", DateTimeOffset.Now);
            }
        }

        async Task HandleHttpsListenerAsync(CancellationToken cancellationToken)
        {
            if (C.L.httpsListener is not null)
            {
                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        _logger.LogInformation("{time} => Waiting for https context", DateTimeOffset.Now);
                        var context = await C.L.httpsListener.GetContextAsync();
                        _ = HandleHttpsRequestAsync(context, cancellationToken);
                    }
                }
                catch (HttpListenerException ex) when (ex.ErrorCode == 995) // Ignoring the "The I/O operation has been aborted because of either a thread exit or an application request" error.
                {
                    // Ignore this exception since it's thrown when stopping the listener.
                }
                catch (OperationCanceledException)
                {
                    // Ignore this exception since it's thrown when cancellationToken is canceled.
                }
                finally
                {
                    C.L.httpsListener.Close();
                    _logger.LogInformation("{time} => Https listener closed", DateTimeOffset.Now);
                }
            }
        }

        async Task HandleHttpRequestAsync(HttpListenerContext context, CancellationToken cancellationToken)
        {
            try
            {
                if (context.Request.Url!.AbsolutePath == "/ip")
                {
                    _logger.LogInformation("{time} => Reporting Http IP", DateTimeOffset.Now);
                    context.Response.KeepAlive = true;
                    context.Response.StatusCode = 200;
                    context.Response.StatusDescription = "OK";
                    context.Response.Headers.Add("Content-Type", "text/html");
                    using StreamWriter writer = new(context.Response.OutputStream);
                    var sb = new StringBuilder(context.Request.RemoteEndPoint.Address.ToString());
                    await writer.WriteAsync(sb,cancellationToken);
                }
                else
                {
                    await ProcessRequest(context, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Ignore this exception since it's thrown when cancellationToken is canceled.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling request: {ex.Message}");
            }
            finally
            {
                context.Response.Close();
            }
        }

        async Task HandleHttpsRequestAsync(HttpListenerContext context, CancellationToken cancellationToken)
        {
            try
            {
                X509Certificate2 __certificate = new(C.conf.CertificateFilePath, C.conf.CertificatePassword);

                if (context.Request.Url!.AbsolutePath == "/ip")
                {
                    _logger.LogInformation("{time} => Reporting Https IP", DateTimeOffset.Now);
                    context.Response.KeepAlive = true;
                    context.Response.StatusCode = 200;
                    context.Response.StatusDescription = "OK";
                    context.Response.Headers.Add("Content-Type", "text/html");
                    using StreamWriter writer = new(context.Response.OutputStream);
                    var sb = new StringBuilder(context.Request.RemoteEndPoint.Address.ToString());
                    await writer.WriteAsync(sb, cancellationToken);
                }
                else
                {
                    SslStream sslStream = new(context.Request.InputStream, false);
                    sslStream.AuthenticateAsServer(__certificate, false, SslProtocols.Tls13, true);
                    await ProcessRequest(context, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Ignore this exception since it's thrown when cancellationToken is canceled.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling request: {ex.Message}");
            }
            finally
            {
                context.Response.Close();
            }
        }



        async Task ProcessRequest(HttpListenerContext context, CancellationToken cancellationToken)
        {
            if (context.Request.Url!.AbsolutePath == C.conf.Path)
            {
                _logger.LogInformation("{time} => Processing context", DateTimeOffset.Now);
                try
                {
                    string? hostNameAPIKey = context.Request.Headers.GetValues(C.conf.KeyFieldName)!.FirstOrDefault();
                    if (!string.IsNullOrEmpty(hostNameAPIKey))
                    {
                        if (C.hosts.TryGetValue(hostNameAPIKey, out string? hostName))
                        {
                            using StreamReader reader = new(context.Request.InputStream, context.Request.ContentEncoding);
                            C.PostData postData = JsonSerializer.Deserialize<C.PostData>(reader.ReadToEnd())!;

                            if (C.devices.Contains(postData.DeviceID))
                            {
                                C.ResponseData responseCode = new() { Status = 0 /* Update not needed */ };
                                string responseMessage = "Update not needed";

                                if (postData.IP == "0.0.0.0")
                                    postData.IP = context.Request.RemoteEndPoint.Address.ToString();

                                string currentValue = File.Exists($"{hostName}.ip") ? File.ReadAllText($"{hostName}.ip") : "0.0.0.0";

                                if (currentValue != postData.IP) // --- This is currently not thread safe ---
                                {
                                    responseCode.Status = -1; // Update failed
                                    responseMessage = "Update failed";

                                    File.AppendAllText($"server.log", $"{C.TS} => Updating zone file: {C.conf.ZoneFilePath}\n");

                                    if (Bind.UpdateARecord(C.conf.ZoneFilePath, hostName, postData.IP) && Bind.IncreaseSerial(C.conf.ZoneFilePath))
                                    {
                                        responseCode.Status = 1; // Update succeeded
                                        responseMessage = postData.IP;
                                        C.RestartBIND9 = true;

                                        File.WriteAllText($"{hostName}.ip", postData.IP);

                                        Bynd9Notifier.Discord.Server.Send(C.conf.Discord, $"{hostName}.{C.conf.FQDNSuffix}", currentValue, postData.IP);
                                        Bynd9Notifier.Telegram.Server.Send(C.conf.TelegramUser, $"{hostName}.{C.conf.FQDNSuffix}", currentValue, postData.IP);
                                        Bynd9Notifier.Whatsapp.Server.Send(C.conf.WhatsappNumber, C.conf.WhatsappKey, $"{hostName}.{C.conf.FQDNSuffix}", currentValue, postData.IP);
                                    }
                                }

                                File.AppendAllText($"{hostName}.history", $"{C.TS} => {responseMessage}\n");

                                context.Response.KeepAlive = true;
                                context.Response.StatusCode = 200;
                                context.Response.StatusDescription = "OK";
                                context.Response.Headers.Add("Content-Type", "text/html");
                                using StreamWriter writer = new(context.Response.OutputStream);
                                var sb = new StringBuilder(responseCode.Status.ToString());
                                await writer.WriteAsync(sb, cancellationToken);
                            }
                        }
                        else
                        {
                            string msg = "Invalid DeviceID";
                            _logger.LogError("{time} => " + msg, DateTimeOffset.Now);
                            Bynd9Notifier.Discord.Server.SendError(C.conf.Discord, msg);
                            Bynd9Notifier.Telegram.Server.SendError(C.conf.TelegramUser, msg);
                            Bynd9Notifier.Whatsapp.Server.SendError(C.conf.WhatsappNumber, C.conf.WhatsappKey, msg);
                            File.AppendAllText($"server.log", $"{C.TS} => {msg}\n");
                        }
                    }
                    else
                    {
                        string msg = "Invalid API key";
                        _logger.LogError("{time} => Invalid API key", DateTimeOffset.Now);
                        Bynd9Notifier.Discord.Server.SendError(C.conf.Discord, msg);
                        Bynd9Notifier.Telegram.Server.SendError(C.conf.TelegramUser, msg);
                        Bynd9Notifier.Whatsapp.Server.SendError(C.conf.WhatsappNumber, C.conf.WhatsappKey, msg);
                        File.AppendAllText($"server.log", $"{C.TS} => {msg}\n");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("{time} => Exception " + ex.Message, DateTimeOffset.Now);
                    Bynd9Notifier.Discord.Server.SendError(C.conf.Discord, ex.Message);
                    Bynd9Notifier.Telegram.Server.SendError(C.conf.TelegramUser, ex.Message);
                    Bynd9Notifier.Whatsapp.Server.SendError(C.conf.WhatsappNumber, C.conf.WhatsappKey, ex.Message);
                    File.AppendAllText($"server.log", $"{C.TS} => {ex.Message}\n");
                }
            }
            else
            {
                string msg = "Invalid AbsolutePOath";
                _logger.LogError("{time} => " + msg, DateTimeOffset.Now);
                Bynd9Notifier.Discord.Server.SendError(C.conf.Discord, msg);
                Bynd9Notifier.Telegram.Server.SendError(C.conf.TelegramUser, msg);
                Bynd9Notifier.Whatsapp.Server.SendError(C.conf.WhatsappNumber, C.conf.WhatsappKey, msg);
                File.AppendAllText($"server.log", $"{C.TS} => {msg}\n");
            }
            context.Response.Close();
        }


    }
}