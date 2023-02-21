using Gomez.Core.BackgroundQueue;
using Gomez.Factorio.DataTransmitter;
using Gomez.Factorio.Models;
using Gomez.Factorio.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Gomez.Factorio.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly IFileTransfer _fileTransfer;
        private readonly ILogger<StatisticService> _logger;

        public StatisticService(
            IBackgroundTaskQueue backgroundTaskQueue,
            IFileTransfer fileTransfer,
            ILogger<StatisticService> logger)
        {
            StatisticsPath = Path.Combine(FactorioPath.ScriptOutput, "statistic");
            _backgroundTaskQueue = backgroundTaskQueue;
            _fileTransfer = fileTransfer;
            _logger = logger;
        }

        public string StatisticsPath { get; private set; }

        public Task StartTransferAsync(CancellationToken ct)
        {
            return Task.WhenAll(
                _fileTransfer.CreateIfNotExistsAsync(ct),
                Task.Factory.StartNew(
                    async () =>
                    {
                        while (!ct.IsCancellationRequested)
                        {
                            await Task.Delay(60_000);
                            await TransferAsync(ct);
                            _logger.LogInformation("Transfered factorio files");
                        }
                    },
                    ct));
        }

        private ValueTask TransferAsync(CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested)
            {
                return ValueTask.FromCanceled(ct);
            }

            var filePaths = Directory.EnumerateFiles(StatisticsPath, "*", SearchOption.AllDirectories);
            return _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async (ct) =>
            {
                await _fileTransfer.UploadAsync(filePaths, ct);
            });
        }
    }
}
