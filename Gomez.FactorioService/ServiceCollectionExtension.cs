using Gomez.Core.BackgroundQueue;
using Gomez.FactorioService.Options;
using Gomez.FactorioService.Services;
using Gomez.SteamCmd;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gomez.FactorioService
{
    public static class ServiceCollectionExtension
    {
        private static IHostEnvironment _environment = default!;
        private static IConfiguration _configuration = default!;

        public static IServiceCollection InitializeAppServiceProvider(this IServiceCollection services, IHostEnvironment env)
        {
            SetEnv(env);
            BuildConfiguration();

            services.AddAppLogging();
            services.AddAppConfiguration();

            return services;
        }

        private static IServiceCollection AddAppConfiguration(this IServiceCollection services)
        {
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue>(sp =>
            {
                return new BackgroundTaskQueue(5);
            });

            services.AddSingleton(sp => _configuration);
            services.AddOptions();
            services.Configure<GameOption>(_configuration.GetSection(GameOption.SectionName));
            services.Configure<ApplicationOption>(_configuration);
            services.AddSteamConfiguration(_configuration);
            services.AddSingleton<IGameService, GameService>();
            services.AddScoped<IApplicationService, ApplicationService>();
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
                .Build();
        }
    }
}
