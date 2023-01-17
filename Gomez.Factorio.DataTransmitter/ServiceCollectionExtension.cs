using Azure.Identity;
using Azure.Storage.Blobs;
using Gomez.Factorio.DataTransmitter.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Gomez.Factorio.DataTransmitter
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddTransferConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<StorageOption>(configuration.GetSection(StorageOption.SectionName))
            .AddSingleton(sp =>
            {
                var option = sp.GetRequiredService<IOptions<StorageOption>>().Value;
                var connectionString = configuration.GetConnectionString(SettingsConstant.ConnectionString);
                if (!string.IsNullOrEmpty(connectionString))
                {
                    return new BlobContainerClient(connectionString, option.ContainerName);
                }

                var containerEndpoint = $"https://{option.AccountName}.blob.core.windows.net/{option.ContainerName}";
                return new BlobContainerClient(new Uri(containerEndpoint), new DefaultAzureCredential(true));
            })
            .AddSingleton<IFileTransfer, FileTransfer>();

            return services;
        }
    }
}
