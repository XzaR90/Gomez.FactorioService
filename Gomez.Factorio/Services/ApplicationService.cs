using Gomez.Factorio.Options;
using Gomez.Factorio.Services.Interfaces;
using Gomez.Factorio.Utils;
using Gomez.SteamCmd.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Gomez.Factorio.Services
{
    public class ApplicationService : IApplicationService, IDisposable
    {
        private readonly ISteamCmdService _steamCmdService;
        private readonly IGameService _gameService;
        private readonly IStatisticService _statisticService;
        private readonly IModService _modService;
        private readonly ILogger<ApplicationService> _logger;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly ApplicationOption _option;

        private CancellationTokenSource? _cts = new();
        private DailyScheduler? _scheduler = null;
        private bool _disposedValue;

        public ApplicationService(
            ISteamCmdService steamCmdService,
            IGameService gameService,
            ILogger<ApplicationService> logger,
            IHostApplicationLifetime lifetime,
            IOptions<ApplicationOption> option,
            IStatisticService statisticService,
            IModService modService)
        {
            _steamCmdService = steamCmdService;
            _logger = logger;
            _gameService = gameService;
            _lifetime = lifetime;
            _option = option.Value;
            _statisticService = statisticService;
            _modService = modService;
        }

        public async Task RunAsync()
        {
            if (_lifetime.ApplicationStopping.IsCancellationRequested)
            {
                return;
            }

            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_cts!.Token, _lifetime.ApplicationStopping);
            _scheduler = new DailyScheduler(_option, _lifetime.ApplicationStopping);
            await _scheduler.StartAsync();
            _scheduler.Invoked += SchedulerInvokedAsync;

            await _statisticService.StartTransferAsync(linkedCts.Token);
            await _modService.StartAsync(linkedCts.Token);
            await StartAsync(linkedCts);
        }

        public async Task WaitUntilProcessClosedAsync()
        {
            var retries = 60;
            while (Process.GetProcessesByName(_gameService.ProcessName).Length > 0 && retries > 0)
            {
                retries--;
                await Task.Delay(1000);
                _logger.LogWarning("Waiting until processes with name ({ProcessName}) are closed...", _gameService.ProcessName);
            }

            _gameService.KillExistingProcesses();
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
                if (disposing)
                {
                    if (_cts is not null)
                    {
                        _cts.Dispose();
                        _cts = null;
                    }

                    if (_scheduler is not null)
                    {
                        _scheduler.Dispose();
                        _scheduler = null;
                    }
                }

                _disposedValue = true;
            }
        }

        private async void SchedulerInvokedAsync(object? sender, EventArgs e)
        {
            _scheduler?.Dispose();
            _cts?.Cancel();
            await WaitUntilProcessClosedAsync();

            _cts = new();
            await RunAsync();
        }

        private async Task StartAsync(CancellationTokenSource linkedCts)
        {
            await _steamCmdService.RunAsync(linkedCts.Token);
            if (!_steamCmdService.State.HasErrors && !linkedCts.Token.IsCancellationRequested)
            {
                await _gameService.RunAsync(linkedCts.Token);
            }
        }
    }
}
