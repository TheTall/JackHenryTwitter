using DataLayer;
using TwitterWorker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<TwitterStats>();
        services.AddHostedService<Worker>();
        services.AddHttpClient();
    })
    .Build();

await host.RunAsync();
