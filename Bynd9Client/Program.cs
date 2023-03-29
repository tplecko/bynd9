using Bynd9Client;
using System.Net;
using System.Security.Cryptography.X509Certificates;

static bool GetNewIP(out string NewIP)
{
    var url = $"http://{C.conf.Server}:{C.conf.Port}/ip";
    NewIP = "0.0.0.0";

    HttpClient client = new();
    HttpContent content = new StringContent("");

    try
    {
        HttpResponseMessage response = client.PostAsync(url, content).Result;
        string currentIP = response.Content.ReadAsStringAsync().Result;

        string lastIP = File.Exists("ip.last") ? File.ReadAllText("ip.last") : "0.0.0.0";
        if (currentIP != lastIP)
        {
            NewIP = currentIP;
            Bynd9Notifier.Discord.Client.Send(C.conf.Discord, C.conf.DeviceID, lastIP, NewIP, C.conf.Server);
            Bynd9Notifier.Telegram.Client.Send(C.conf.TelegramUser, C.conf.DeviceID, lastIP, NewIP, C.conf.Server);
            NewIP = currentIP;
            return true;
        }
    }
    catch
    {
        return false;
    }
    return false;
}

static string y(int x) => x switch
{
    0 => "Update not needed",
    1 => "Update OK",
    -1 => "Update failed",
    _ => "Unknown code"
};

new System.Timers.Timer { AutoReset = true, Enabled = true, Interval = 3000 }.Elapsed += (sender, args) => {


    if (GetNewIP(out string NewIP))
    {
        File.WriteAllText("ip.last",NewIP);
        File.AppendAllText($"ip.history", $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} => {NewIP}\n");

        var url = $"http://{C.conf.Server}:{C.conf.Port}{C.conf.Path}";

        using HttpClient client = new();
        client.DefaultRequestHeaders.Add(C.conf.KeyFieldName, C.conf.KeyFieldValue);

        string requestBody = $"{{\"DeviceID\": \"{C.conf.DeviceID}\", \"IP\": \"{NewIP}\"}}";


        using var httpContent = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");
        using HttpResponseMessage response = client.PostAsync(url, httpContent).Result;

        if (response.IsSuccessStatusCode)
        {
            string responseBody = response.Content.ReadAsStringAsync().Result;
            if (int.TryParse(responseBody, out int x))
            {
                File.AppendAllText("client.log", $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} => {y(x)}\n");
            }
        }
        else
        {
            File.AppendAllText("client.log", $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} => {response.StatusCode}\n");
        }
    }
};


IHost host = Host.CreateDefaultBuilder(args)
    .UseSystemd()
    //.UseWindowsService()
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();