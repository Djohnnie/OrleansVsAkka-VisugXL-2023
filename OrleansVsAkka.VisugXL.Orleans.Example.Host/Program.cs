

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {

    })
    .UseOrleans((builderContext, siloBuilder) =>
    {
#if DEBUG
        siloBuilder.UseLocalhostClustering();
#else
        var azureStorageConnectionString = builderContext.Configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");

        siloBuilder.UseAzureStorageClustering(options =>
        {
            options.ConfigureTableServiceClient(azureStorageConnectionString);
        });
#endif
        siloBuilder.ConfigureLogging(loggingBuilder =>
        {
            loggingBuilder.AddConsole();
        });

        siloBuilder.UseDashboard();
    })
    .Build();

host.Run();