using Bynd9;

Bynd9Notifier.Discord.Server.Init(C.conf.Discord);
Bynd9Notifier.Telegram.Server.Init(C.conf.TelegramUser);
Bynd9Notifier.Whatsapp.Server.Init(C.conf.WhatsappNumber,C.conf.WhatsappKey);

#region Setup ThreadPool
ThreadPool.GetMaxThreads(out int workerThreadsMax, out int completionPortThreadsMax);
ThreadPool.SetMinThreads(workerThreadsMax, completionPortThreadsMax);
#endregion

new Listener();

new System.Timers.Timer { AutoReset = true, Enabled = true, Interval = C.conf.Interval * 1000 }.Elapsed += (sender, args) => {
    if (C.RestartBIND9)
    {
        C.RestartBIND9 = false;
        Bind.Restart();
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