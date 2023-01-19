using Gomez.Core;
using Gomez.Core.Exceptions;
using Gomez.Factorio.Models;
using Gomez.Factorio.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO.Compression;
using System.Text.Json;

namespace Gomez.Factorio.Services
{
    internal class ModService : IModService
    {
        private readonly ModdingOption _option;
        private readonly ILogger<ModService> _logger;
        private readonly IModHttpClient _httpClient;
        private readonly DebounceWrapper debouncedWrapper = new();

        private FileSystemWatcher? _fileSystemWatcher;
        private bool _disposedValue;

        public ModService(
            IOptions<ModdingOption> option,
            ILogger<ModService> logger,
            IModHttpClient httpClient)
        {
            _option = option.Value;
            _logger = logger;
            _httpClient = httpClient;
        }

        public Task StartAsync(CancellationToken ct)
        {
            ThrowIfModPathIsEmpty();

            if (_fileSystemWatcher is not null)
            {
                return Task.CompletedTask;
            }

            ct.Register(() =>
            {
                Dispose();
            });

            return Task.Factory.StartNew(
                () =>
                {
                    _fileSystemWatcher = new FileSystemWatcher(_option.ModFolder!)
                    {
                        NotifyFilter =
                            NotifyFilters.LastWrite |
                            NotifyFilters.FileName |
                            NotifyFilters.DirectoryName,
                        IncludeSubdirectories = true,
                        EnableRaisingEvents = true,
                        Filter = "changelog.txt",
                    };

                    _fileSystemWatcher.Changed += OnChangedAsync;
                    _fileSystemWatcher.Created += OnChangedAsync;
                    _fileSystemWatcher.Deleted += OnChangedAsync;
                    _fileSystemWatcher.Renamed += OnChangedAsync;
                },
                ct);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing && _fileSystemWatcher is not null)
                {
                    _fileSystemWatcher.Dispose();
                    _fileSystemWatcher = null;
                }

                _disposedValue = true;
            }
        }

        private async void OnChangedAsync(object sender, FileSystemEventArgs e)
        {
            await debouncedWrapper.DebounceAsync(ZipAndUploadAsync);
        }

        private async Task ZipAndUploadAsync()
        {
            ThrowIfModPathIsEmpty();

            _logger.LogInformation("Change made to mod script directory.");


            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            var modInfoPath = Path.Combine(_option.ModFolder!, "info.json");
            var info = JsonSerializer.Deserialize<ModInfo>(await File.ReadAllTextAsync(modInfoPath), options)!;
            info.Version = info.Versioning.Bump().ToString();

            await File.WriteAllTextAsync(modInfoPath, JsonSerializer.Serialize(info, options));

            var modName = $"{info.Name}_{info.Version}";
            var zipFileName = Path.Combine(FactorioPath.Mods, $"{modName}.zip");

            var zipStream = new FileStream(Path.GetFullPath(zipFileName), FileMode.Create, FileAccess.Write);
            using var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, false);

            foreach (var filePath in Directory.GetFiles(_option.ModFolder!, "*.*", SearchOption.AllDirectories))
            {
                var relativePath = filePath.Replace(_option.ModFolder!, info.Name);
                var unixRelativePath = relativePath.Replace('\\', '/');
                using Stream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                using Stream fileStreamInZip = archive.CreateEntry(unixRelativePath).Open();
                await fileStream.CopyToAsync(fileStreamInZip);
            }

            await _httpClient.PostInitAsync(info, zipFileName);
        }

        private void ThrowIfModPathIsEmpty()
        {
            if (string.IsNullOrEmpty(_option.ModFolder))
            {
                throw new FactorioServiceException($"The setting '{nameof(_option.ModFolder)}' is empty.");
            }
        }
    }
}
