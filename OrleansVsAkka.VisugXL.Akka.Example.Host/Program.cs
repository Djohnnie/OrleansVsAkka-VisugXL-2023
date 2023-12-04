using System.Net;
using System.Reflection;
using Akka.Bootstrap.Docker;
using Akka.Cluster.Hosting;
using Akka.Configuration;
using Akka.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OrleansVsAkka.VisugXL.Akka.Example.Host;
using OrleansVsAkka.VisugXL.Akka.Example.Host.Actors;
using OrleansVsAkka.VisugXL.Akka.Example.Host.Messages;
using OrleansVsAkka.VisugXL.Akka.Example.Host.Sharding;
using Petabridge.Cmd.Cluster;
using Petabridge.Cmd.Host;
using Petabridge.Cmd.Remote;
using Phobos.Actor;
using Phobos.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var resource = ResourceBuilder.CreateDefault()
    .AddService(Assembly.GetEntryAssembly()!.GetName().Name!, serviceInstanceId: $"{Dns.GetHostName()}");

ConfigureOpenTelemetry(builder.Services);
ConfigureAkka(builder.Services);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.Run();


void ConfigureOpenTelemetry(IServiceCollection services)
{
    // enables OpenTelemetry for ASP.NET / .NET Core
    services.AddOpenTelemetry()
        .WithTracing(tracingBuilder =>
        {
            tracingBuilder
                .SetResourceBuilder(resource)
                .AddPhobosInstrumentation()
                .AddSource("OrleansVsAkka.VisugXL.Akka.Example.Host")
                .AddOtlpExporter();
        })
        .WithMetrics(metricsBuilder =>
        {
            metricsBuilder
                .SetResourceBuilder(resource)
                .AddPhobosInstrumentation()
                .AddPrometheusExporter(_ => { });
        });
}

void ConfigureAkka(IServiceCollection services)
{
    services.AddAkka("ClusterSys", (configurationBuilder, _) =>
    {
        // use our legacy app.conf file
        var config = ConfigurationFactory.ParseString(File.ReadAllText("app.conf"))
            .BootstrapFromDocker()
            .UseSerilog();

        configurationBuilder.AddHocon(config, HoconAddMode.Append)
            .WithClustering(new ClusterOptions { Roles = new[] { "console" }, })
            .WithPhobos(AkkaRunMode.AkkaCluster) // enable Phobos
            .StartActors((system, registry) =>
            {
                // start https://cmd.petabridge.com/ for diagnostics and profit
                var pbm = PetabridgeCmd.Get(system); // start Pbm
                pbm.RegisterCommandPalette(ClusterCommands.Instance);
                pbm.RegisterCommandPalette(new RemoteCommands());
                pbm.Start(); // begin listening for PBM management commands
            })
            .WithShardRegion<RegistryKey.RandomNumberRegion>(
                typeName: "random-numbers",
                entityPropsFactory: key => RandomNumberGeneratorActor.CreateProps(Guid.Parse(key)),
                messageExtractor: new GenerateRandomNumberExtractor(),
                shardOptions: new ShardOptions());
    });
}