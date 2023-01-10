using Gomez.Core.Services;
using Gomez.FactorioService.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Gomez.FactorioService.Services
{
    public class GameService : ProcessBaseService, IGameService
    {
        private const string GameName = "Factorio";

        private readonly GameOption _option;
        private readonly ILogger<GameService> _logger;

        public GameService(
            IOptions<GameOption> option,
            ILogger<GameService> logger)
        {
            _option = option.Value;
            _logger = logger;
        }

        public override async Task RunAsync(CancellationToken ct)
        {
            var processToRunInfo = new ProcessStartInfo
            {
                Arguments = $"--start-server \"{_option.SavePath}\" --server-settings \"{_option.SettingsPath}\"",
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                WorkingDirectory = Path.GetDirectoryName(_option.ExePath),
                FileName = _option.ExePath,
            };

            var p = new Process
            {
                StartInfo = processToRunInfo,
                EnableRaisingEvents = true,
            };

            p.OutputDataReceived += OutputDataReceived;
            p.ErrorDataReceived += ErrorDataReceived;
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            ct.Register(() =>
            {
                p.Kill(true);
            });
        }

        private void ErrorDataReceived(object sender, DataReceivedEventArgs args)
        {
            if (args.Data is null)
            {
                return;
            }

            _logger.LogError("{GameName}: {Data}", GameName, args.Data);
        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs args)
        {
            if (args.Data is null)
            {
                return;
            }

            _logger.LogInformation("{GameName}: {Data}", GameName, args.Data);
        }
    }
}
