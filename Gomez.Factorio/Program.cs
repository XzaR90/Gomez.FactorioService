using Gomez.Factorio;
using Gomez.Factorio.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = Host.CreateDefaultBuilder(args);
hostBuilder.UseConsoleLifetime();
hostBuilder.ConfigureServices((ctx, services) =>
{
    services.InitializeAppServiceProvider(ctx.HostingEnvironment);
});

using var host = hostBuilder.Build();
await host.StartAsync();

using var scope = host.Services.CreateScope();
var application = scope.ServiceProvider.GetRequiredService<IApplicationService>();
await application.RunAsync();
await application.WaitUntilProcessClosedAsync();
await host.WaitForShutdownAsync();