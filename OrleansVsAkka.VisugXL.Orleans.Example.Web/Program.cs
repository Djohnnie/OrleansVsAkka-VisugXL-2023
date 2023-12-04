using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans.Hosting;
using OrleansVsAkka.VisugXL.Orleans.Example.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Host.UseOrleansClient((builderContext, clientBuilder) =>
{
#if DEBUG
    clientBuilder.UseLocalhostClustering();
#else
    var azureStorageConnectionString = builderContext.Configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");

    clientBuilder.UseAzureStorageClustering(options =>
    {
        options.ConfigureTableServiceClient(azureStorageConnectionString);
    });
#endif
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
