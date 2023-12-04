

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {

    })
    .UseOrleans((siloBuilder) =>
    {
        siloBuilder.UseLocalhostClustering(11111, 30001);

        siloBuilder.ConfigureLogging(loggingBuilder =>
        {
            loggingBuilder.AddConsole();
        });

        siloBuilder.UseDashboard();
    })
    .Build();

host.Run();