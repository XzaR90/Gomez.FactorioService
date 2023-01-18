using Gomez.Core.BackgroundQueue;
using Gomez.Factorio.DataTransmitter;
using Gomez.Factorio.Options;
using Gomez.Factorio.Services;
using Gomez.Factorio.Services.Interfaces;
using Gomez.SteamCmd;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gomez.Factorio
{
    public static class ServiceCollectionExtension
    {
        private static IHostEnvironment _environment = default!;
        private static IConfiguration _configuration = default!;

        public static IServiceCollection InitializeAppServiceProvider(this IServiceCollection services, IHostEnvironment env)
        {
            SetEnv(env);
            BuildConfiguration();

            services.AddAppLogging()
            .AddAppConfiguration();

            return services;
        }

        private static IServiceCollection AddAppConfiguration(this IServiceCollection services)
        {
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue>(sp =>
            {
                return new BackgroundTaskQueue(5);
            });



            services
            .AddSingleton(sp => _configuration)
            .AddOptions()
            .Configure<ModdingOption>(_configuration.GetSection(ModdingOption.SectionName))
            .Configure<GameOption>(_configuration.GetSection(GameOption.SectionName))
            .Configure<ApplicationOption>(_configuration)
            .AddSteamConfiguration(_configuration)
            .AddTransferConfiguration(_configuration)
            .AddSingleton<IGameService, GameService>()
            .AddSingleton<IModService, ModService>()
            .AddSingleton<IStatisticService, StatisticService>()
            .AddScoped<IApplicationService, ApplicationService>();

            services.AddHttpClient<IModHttpClient, ModHttpClient>();

            return services;
        }

        private static IServiceCollection AddAppLogging(this IServiceCollection services)
        {
            services.AddLogging(sp => { sp.AddConsole(); });
            return services;
        }

        private static void SetEnv(IHostEnvironment env)
        {
            _environment = env;
        }

        private static void BuildConfiguration()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{_environment.EnvironmentName}.json", true)
                .AddUserSecrets<Program>()
                .Build();
        }
    }
}
