using Gomez.Core;
using Gomez.Core.Exceptions;
using Gomez.Factorio.Models;
using Gomez.Factorio.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO.Compression;
using System.Text.Json;

namespace Gomez.Factorio.Services
{
    internal class ModService : IModService
    {
        private readonly ModdingOption _option;
        private readonly ILogger<ModService> _logger;
        private readonly IModHttpClient _httpClient;
        private readonly DebounceWrapper _debouncedWrapper = new();

        private FileSystemWatcher? _fileSystemWatcher;
        private bool _disposedValue;
        private CancellationToken _ct;

        public ModService(
            IOptions<ModdingOption> option,
            ILogger<ModService> logger,
            IModHttpClient httpClient)
        {
            _option = option.Value;
            _logger = logger;
            _httpClient = httpClient;
        }

        public event EventHandler? ModPublished;

        public Task StartAsync(CancellationToken ct)
        {
            ThrowIfModPathIsEmpty();

            if (_fileSystemWatcher is not null)
            {
                return Task.CompletedTask;
            }

            _ct = ct;
            _ct.Register(() =>
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

        protected virtual void OnModPublished(EventArgs e)
        {
            ModPublished?.Invoke(this, e);
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

        private static async Task<ModInfo> GetInfoAndBumpVersionAsync(JsonSerializerOptions options, string modInfoPath)
        {
            var info = JsonSerializer.Deserialize<ModInfo>(await File.ReadAllTextAsync(modInfoPath), options)!;
            info.Version = info.Versioning.Bump().ToString();
            return info;
        }

        private async void OnChangedAsync(object sender, FileSystemEventArgs e)
        {
            await _debouncedWrapper.DebounceAsync(ZipAndUploadAsync);
        }

        private async Task ZipAndUploadAsync()
        {
            ThrowIfModPathIsEmpty();
            if (_ct.IsCancellationRequested)
            {
                return;
            }

            _logger.LogInformation("Change made to mod script directory.");

            var info = await GetAndSaveChangedModInfoAsync();

            var modName = $"{info.Name}_{info.Version}";
            var zipFileName = Path.Combine(FactorioPath.Mods, $"{modName}.zip");

            await SaveZipFileAsync(info, zipFileName);

            if (_option.EnableUpload && !await _httpClient.PostInitAsync(info, zipFileName))
            {
                await RevertChangesAsync(info, zipFileName);
            }

            if (_ct.IsCancellationRequested)
            {
                return;
            }

            OnModPublished(new EventArgs());
        }

        private async Task RevertChangesAsync(ModInfo info, string zipFileName)
        {
            _logger.LogError("Failed to upload mod to the mod portal, reverting changes.");
            File.Delete(zipFileName);
            var newVersion = info.Version;
            info.Version = info.Versioning.Debump().ToString();
            GetInfoSaveInfo(out JsonSerializerOptions options, out string modInfoPath);
            await File.WriteAllTextAsync(modInfoPath, JsonSerializer.Serialize(info, options));
            _logger.LogError("Deleted mod version {NewVersion} from server and changed to {Version}.", newVersion, info.Version);
        }

        private async Task<ModInfo> GetAndSaveChangedModInfoAsync()
        {
            GetInfoSaveInfo(out JsonSerializerOptions options, out string modInfoPath);
            var info = await GetInfoAndBumpVersionAsync(options, modInfoPath);

            await File.WriteAllTextAsync(modInfoPath, JsonSerializer.Serialize(info, options));
            return info;
        }

        private void GetInfoSaveInfo(out JsonSerializerOptions options, out string modInfoPath)
        {
            options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            modInfoPath = Path.Combine(_option.ModFolder!, "info.json");
        }

        private async Task SaveZipFileAsync(ModInfo info, string zipFileName)
        {
            var zipStream = new FileStream(Path.GetFullPath(zipFileName), FileMode.Create, FileAccess.Write);
            using var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, false);

            foreach (var filePath in Directory.GetFiles(_option.ModFolder!, "*.*", SearchOption.AllDirectories))
            {
                var relativePath = filePath.Replace(_option.ModFolder!, info.Name);
                var unixRelativePath = relativePath.Replace('\\', '/');
                using Stream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using Stream fileStreamInZip = archive.CreateEntry(unixRelativePath).Open();
                await fileStream.CopyToAsync(fileStreamInZip);
            }
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
