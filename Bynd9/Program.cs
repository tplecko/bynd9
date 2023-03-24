using Bynd9;
using System.Net;
using System.Text.Json;


#region Setup ThreadPool
ThreadPool.GetMaxThreads(out int workerThreadsMax, out int completionPortThreadsMax);
ThreadPool.SetMinThreads(workerThreadsMax, completionPortThreadsMax);
#endregion

new Listener();

new System.Timers.Timer { AutoReset = true, Enabled = true, Interval = 15000 }.Elapsed += (sender, args) => {
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