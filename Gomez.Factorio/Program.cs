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

try
{
    using var scope = host.Services.CreateScope();
    var application = scope.ServiceProvider.GetRequiredService<IApplicationService>();
    await application.RunAsync();
    await application.WaitUntilProcessClosedAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine("Press any key to continue");
    Console.ReadLine();
    throw;
}

await host.WaitForShutdownAsync();