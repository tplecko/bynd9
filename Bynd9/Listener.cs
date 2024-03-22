using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace Bynd9
{
    internal class Listener
    {
        internal readonly HttpListener? httpListener;
        internal readonly HttpListener? httpsListener;

        internal Listener()
        {
            // HTTP
            httpListener = new() { AuthenticationSchemes = AuthenticationSchemes.Anonymous };
            httpListener.Prefixes.Add($"http://*:{C.conf.HttpPort}/");
            HttpListenerTimeoutManager timeoutManager = httpListener.TimeoutManager;
            timeoutManager.IdleConnection = new TimeSpan(0, 0, 5);

            //HTTPS
            if (File.Exists(C.conf.CertificateFilePath))
            {
                httpsListener = new() { AuthenticationSchemes = AuthenticationSchemes.Anonymous };
                httpsListener.Prefixes.Add($"https://*:{C.conf.HttpsPort}/");
                HttpListenerTimeoutManager timeoutManager1 = httpsListener.TimeoutManager;
                timeoutManager1.IdleConnection = new TimeSpan(0, 0, 5);
            }
        }

        internal bool StartListener(out string Err)
        {
            bool __success = false;

            Err = string.Empty;

            if (httpListener is not null)
            {
                try
                {
                    httpListener.Start();
                    File.AppendAllText($"server.log", $"{C.TS} => HTTP started\n");
                    __success = true;
                }
                catch (Exception ex)
                {
                    Err = ex.Message;
                }
            }
            if (httpsListener is not null)
            {
                try
                {
                    httpsListener.Start();
                    File.AppendAllText($"server.log", $"{C.TS} => HTTPS started\n");
                    __success = true;
                }
                catch (Exception ex)
                {
                    Err = ex.Message;
                }
            }
            return __success;
        }

        internal bool StopListener(out string Err)
        {
            bool __success = false;
            
            Err = string.Empty;

            if (httpListener is not null)
            {
                try
                {
                    httpListener.Stop();
                }
                catch (Exception ex)
                {
                    Err = ex.Message;
                }
            }
            if (httpsListener is not null)
            {
                try
                {
                    httpsListener.Stop();
                }
                catch (Exception ex)
                {
                    Err = ex.Message;
                }
            }
            return __success;
        }

    }
}
