using Gomez.Factorio.Options;
using Gomez.Factorio.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Gomez.Factorio.Services
{
    public class GameProcess : IGameProcess
    {
        private readonly GameOption _option;
        private readonly ILogger<GameService> _logger;

        private Process? _process;
        private bool _safeClosed;
        private bool _disposedValue;

        public GameProcess(
            GameOption option,
            ILogger<GameService> logger)
        {
            _option = option;
            _logger = logger;
            ProcessName = _option.ProcessName;
        }

        public bool SafeClosed
        {
            get => _safeClosed; private set
            {
                _safeClosed = value;
                if (_safeClosed)
                {
                    var sw = _process?.StandardInput;
                    if (sw?.BaseStream == null)
                    {
                        return;
                    }

                    sw.Dispose();
                }
            }
        }

        public string ProcessName { get; set; }

        public Task StartAsync(CancellationToken ct)
        {
            SafeClosed = false;
            var processToRunInfo = new ProcessStartInfo
            {
                Arguments = $"{_option.ExePath} --start-server {_option.SavePath} --server-settings {_option.SettingsPath}",
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                FileName = "powershell",
            };

            _process = new Process
            {
                StartInfo = processToRunInfo,
                EnableRaisingEvents = true,
            };

            _process.OutputDataReceived += OutputDataReceived;
            _process.ErrorDataReceived += ErrorDataReceived;
            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            ct.Register(() =>
            {
                TryQuit();
            });

            return _process.WaitForExitAsync(CancellationToken.None);
        }

        public async Task WriteToChatAsync(string message)
        {
            if (_process?.HasExited != false || _process.StandardInput?.BaseStream is null)
            {
                return;
            }

            await _process.StandardInput.WriteLineAsync(message);
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
                if (disposing && _process is not null)
                {
                    _process.Dispose();
                    _process = null;
                }

                _disposedValue = true;
            }
        }

        private void ErrorDataReceived(object sender, DataReceivedEventArgs args)
        {
            if (args.Data is null)
            {
                return;
            }

            _logger.LogError("{ProcessName}: {Data}", ProcessName, args.Data);
        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs args)
        {
            if (args.Data is null)
            {
                return;
            }

            var quitMessages = new string[] { " changing state from(Disconnected) to(Closed)", "Goodbye" };
            if (quitMessages.Any(x => args.Data.EndsWith(x)) && !args.Data.Contains("[CHAT]"))
            {
                SafeClosed = true;
            }

            _logger.LogInformation("{ProcessName}: {Data}", ProcessName, args.Data);
        }

        private void TryQuit()
        {
            var sw = _process?.StandardInput;
            if (sw?.BaseStream == null)
            {
                return;
            }

            _logger.LogInformation("{ProcessName}: Server is restarting or closing.", ProcessName);
            sw.WriteLine("GameService: server is restarting or closing.");
            sw.Flush();
            Thread.Sleep(5000);
            sw.WriteLine("/quit");
            sw.Flush();
        }
    }
}
