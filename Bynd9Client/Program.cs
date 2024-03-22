using Bynd9Client;
using System.Net;
using System.Security.Cryptography.X509Certificates;

try
{
    Bynd9Notifier.Discord.Client.Init(C.conf.Discord);
    Bynd9Notifier.Telegram.Client.Init(C.conf.TelegramUser);
    Bynd9Notifier.Whatsapp.Client.Init(C.conf.WhatsappNumber, C.conf.WhatsappKey);
}
catch (Exception e)
{
    File.AppendAllText("client.log", $"{C.TS} => {e.Message}\n");
}
static bool GetNewIP(out string newIP)
{
    var url = $"http://{C.conf.Server}:{C.conf.Port}/ip";
    newIP = "0.0.0.0";

    HttpClient client = new();
    HttpContent content = new StringContent("");

    try
    {
        HttpResponseMessage response = client.PostAsync(url, content).Result;
        string currentIP = response.Content.ReadAsStringAsync().Result;

        string lastIP = File.Exists("ip.last") ? File.ReadAllText("ip.last") : "0.0.0.0";
        if (currentIP != lastIP)
        {
            newIP = currentIP;
            try
            {
                Bynd9Notifier.Discord.Client.Send(C.conf.Discord, C.conf.DeviceID, lastIP, newIP, C.conf.Server);
                Bynd9Notifier.Telegram.Client.Send(C.conf.TelegramUser, C.conf.DeviceID, lastIP, newIP, C.conf.Server);
                Bynd9Notifier.Whatsapp.Client.Send(C.conf.WhatsappNumber, C.conf.WhatsappKey, C.conf.DeviceID, lastIP, newIP, C.conf.Server);
            }
            catch (Exception ex)
            { 
                File.AppendAllText("client.log", $"{C.TS} => {ex.Message}\n");
            }
            newIP = currentIP;
            return true;
        }
    }
    catch (Exception e)
    {
        File.AppendAllText("client.log", $"{C.TS} => {e.Message}\n");
        return false;
    }
    return false;
}

static string CodeToText(int Code) => Code switch
{
    0 => "Update not needed",
    1 => "Update OK",
    -1 => "Update failed",
    _ => "Unknown code"
};

new System.Timers.Timer { AutoReset = true, Enabled = true, Interval = C.conf.Interval * 1000 }.Elapsed += (sender, args) => {
    if (!GetNewIP(out string newIP))
        return;

    File.WriteAllText("ip.last",newIP);
    File.AppendAllText($"ip.history", $"{C.TS} => {newIP}\n");

    var url = $"http://{C.conf.Server}:{C.conf.Port}{C.conf.Path}";

    using HttpClient client = new();
    client.DefaultRequestHeaders.Add(C.conf.KeyFieldName, C.conf.KeyFieldValue);

    string requestBody = $"{{\"DeviceID\": \"{C.conf.DeviceID}\", \"IP\": \"{newIP}\"}}";

    using var httpContent = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");
    using HttpResponseMessage response = client.PostAsync(url, httpContent).Result;

    File.AppendAllText("client.log", $"{C.TS} => {response.StatusCode}\n");
    if (response.IsSuccessStatusCode)
    {
        string responseBody = response.Content.ReadAsStringAsync().Result;
        if (int.TryParse(responseBody, out int x))
            File.AppendAllText("client.log", $"{C.TS} => {CodeToText(x)}\n");
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