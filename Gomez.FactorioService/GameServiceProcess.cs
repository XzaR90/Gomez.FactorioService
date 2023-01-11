﻿using Gomez.FactorioService.Options;
using Gomez.FactorioService.Services;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Gomez.FactorioService
{
    public class GameServiceProcess
    {
        private readonly GameOption _option;
        private readonly ILogger<GameService> _logger;

        private Process? _process;
        private bool _safeClosed;

        public GameServiceProcess(
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

            return _process.WaitForExitAsync();
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

            sw.WriteLine("GameService: server is restarting or closing.");
            sw.Flush();
            Thread.Sleep(5000);
            sw.WriteLine("/quit");
            sw.Flush();
        }
    }
}