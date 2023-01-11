using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gomez.Core.BackgroundQueue
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly ILogger<QueuedHostedService> _logger;

        public QueuedHostedService(
            IBackgroundTaskQueue taskQueue,
            ILogger<QueuedHostedService> logger)
        {
            TaskQueue = taskQueue;
            _logger = logger;
        }

        public IBackgroundTaskQueue TaskQueue { get; }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            return base.StopAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Queued Hosted Service is running");

            return BackgroundProcessingAsync(stoppingToken);
        }

        private async Task BackgroundProcessingAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem =
                    await TaskQueue.DequeueAsync(stoppingToken);

                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error occurred executing {WorkItem}.",
                        nameof(workItem));
                }
            }
        }


    }
}
