using Gomez.SteamCmd.Models;
using Gomez.SteamCmd.Options;
using Gomez.SteamCmd.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gomez.SteamCmd
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSteamConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SteamCmdOption>(configuration.GetSection(SteamCmdOption.SectionName));
            services.AddTransient<SteamCmdState>();
            services.AddScoped<ISteamCmdService, SteamCmdService>();
            return services;
        }
    }
}
