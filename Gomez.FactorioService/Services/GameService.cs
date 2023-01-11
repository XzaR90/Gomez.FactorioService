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

            using var gameProcess = new GameProcess(_option, _logger);
            return Task.Factory.StartNew(() => gameProcess.StartAsync(ct)).Unwrap();
        }

        public void KillExistingProcesses()
        {
            foreach (var process in Process.GetProcessesByName(ProcessName))
            {
                _logger.LogWarning("Killed process ({ProcessName}) with ID: {id}", ProcessName, process.Id);
                process.Kill();
            }
        }
    }
}
