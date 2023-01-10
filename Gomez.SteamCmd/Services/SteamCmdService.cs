using Gomez.Core.Services;
using Gomez.SteamCmd.Models;
using Gomez.SteamCmd.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Gomez.SteamCmd.Services
{
    public class SteamCmdService : ProcessBaseService, ISteamCmdService
    {
        private const string SteamCMD = "SteamCMD";

        private readonly SteamCmdOption _option;
        private readonly ILogger<SteamCmdService> _logger;

        public SteamCmdService(
            IOptions<SteamCmdOption> option,
            ILogger<SteamCmdService> logger,
            SteamCmdState state)
        {
            _option = option.Value;
            _logger = logger;
            State = state;
        }

        public SteamCmdState State { get; private set; }

        public override async Task RunAsync(CancellationToken ct)
        {
            _logger.LogInformation("{SteamCMD}: Try Downloading and Updating game with '{AppId}'.", SteamCMD, _option.AppId);
            var processToRunInfo = new ProcessStartInfo
            {
                Arguments = $"+force_install_dir \"{_option.AppPath}\" +login \"{_option.UserName}\" +app_update {_option.AppId} +exit",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = Path.GetDirectoryName(_option.CmdPath),
                FileName = _option.CmdPath,
            };

            using var p = new Process
            {
                StartInfo = processToRunInfo,
            };

            p.OutputDataReceived += OutputDataReceived;
            p.ErrorDataReceived += ErrorDataReceived;
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            try
            {
                await p.WaitForExitAsync(ct);
            }
            catch (OperationCanceledException)
            {
                // empty on purpose
            }
            finally
            {
                p.Close();
            }

            if (State.CurrentOutput is not null)
            {
                State = State with { HasErrors = true };
            }
        }

        private void ErrorDataReceived(object sender, DataReceivedEventArgs args)
        {
            if(args.Data is null)
            {
                return;
            }

            _logger.LogError("{SteamCMD}: {Data}", SteamCMD, args.Data);
            State = State with { HasErrors = true };
        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs args)
        {
            State = State with { CurrentOutput = args.Data };

            _logger.LogTrace("{SteamCMD}: {Data}", SteamCMD, args.Data);
            if (args.Data is null)
            {
                _logger.LogInformation("{SteamCMD}: Completed without errors.", SteamCMD);
            }
        }
    }
}