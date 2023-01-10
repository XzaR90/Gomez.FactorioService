using Gomez.FactorioService.Options;
using Gomez.SteamCmd.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gomez.FactorioService.Services
{
    public class ApplicationService : IApplicationService, IDisposable
    {
        private readonly ISteamCmdService _steamCmdService;
        private readonly IGameService _factorioService;
        private readonly ILogger<ApplicationService> _logger;
        private readonly IHostApplicationLifetime _lifetime;

        private readonly ApplicationOption _option;

        private Timer? _timer = default!;
        private DateOnly? _lastRestart;
        private bool _disposedValue;
        private CancellationTokenSource _cts = new();

        public ApplicationService(
            ISteamCmdService steamCmdService,
            IGameService factorioService,
            ILogger<ApplicationService> logger,
            IHostApplicationLifetime lifetime,
            IOptions<ApplicationOption> option)
        {
            _steamCmdService = steamCmdService;
            _logger = logger;
            _factorioService = factorioService;
            _lifetime = lifetime;
            _option = option.Value;
        }


        public async Task RunAsync()
        {
            if (_lifetime.ApplicationStopping.IsCancellationRequested)
            {
                return;
            }

            SetLastRestartIfDue();

            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, _lifetime.ApplicationStopping);
            _timer = new Timer(new TimerCallback(CancelAfterTimeoutAsync), null, 0, 1000);

            await _steamCmdService.RunAsync(linkedCts.Token);
            if (!_steamCmdService.State.HasErrors && !linkedCts.Token.IsCancellationRequested)
            {
                await _factorioService.RunAsync(linkedCts.Token);
            }
        }

        private void SetLastRestartIfDue()
        {
            var currentDateTime = DateTime.Now;
            var currentTime = TimeOnly.FromDateTime(currentDateTime);
            if (_lastRestart == null && currentTime > _option.RestartAfter)
            {
                var currentDate = DateOnly.FromDateTime(currentDateTime);
                _lastRestart = currentDate;
            }
        }

        public async void CancelAfterTimeoutAsync(object? state)
        {
            var currentDateTime = DateTime.Now;
            var currentDate = DateOnly.FromDateTime(currentDateTime);
            if (currentDate == _lastRestart)
            {
                return;
            }

            var currentTime = TimeOnly.FromDateTime(currentDateTime);
            if (currentTime >= _option.RestartAfter)
            {
                _lastRestart = currentDate;

                _logger.LogInformation("Restart Application...");
                _cts.Cancel();
                _timer!.Dispose();
                _cts = new();
                await RunAsync();
            }
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
                    if (_timer is not null)
                    {
                        _timer.Dispose();
                        _timer = null;
                    }

                    if (_cts is not null)
                    {
                        _cts.Dispose();
                        _cts = null;
                    }
                }

                _disposedValue = true;
            }
        }
    }
}
