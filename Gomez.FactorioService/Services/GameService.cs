using Gomez.Core.Services;
using Gomez.FactorioService.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Gomez.FactorioService.Services
{
    public class GameService : ProcessBaseService, IGameService
    {
        private readonly GameOption _option;
        private readonly ILogger<GameService> _logger;

        private IGameProcess? _gameProcess;
        private bool _disposedValue;

        public GameService(
            IOptions<GameOption> option,
            ILogger<GameService> logger)
        {
            _option = option.Value;
            _logger = logger;
            ProcessName = _option.ProcessName;
        }

        public string ProcessName { get; set; }

        public override Task RunAsync(CancellationToken ct)
        {
            KillExistingProcesses();

            _gameProcess = new GameProcess(_option, _logger);
            return Task.Factory.StartNew(() => _gameProcess.StartAsync(ct)).Unwrap();
        }

        public Task WriteToChatAsync(string message)
        {
            return _gameProcess?.WriteToChatAsync(message) ?? Task.CompletedTask;
        }

        public void KillExistingProcesses()
        {
            foreach (var process in Process.GetProcessesByName(ProcessName))
            {
                _logger.LogWarning("Killed process ({ProcessName}) with ID: {id}", ProcessName, process.Id);
                process.Kill();
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
                if (disposing && _gameProcess is not null)
                {
                    _gameProcess.Dispose();
                    _gameProcess = null;
                }

                _disposedValue = true;
            }
        }
    }
}
