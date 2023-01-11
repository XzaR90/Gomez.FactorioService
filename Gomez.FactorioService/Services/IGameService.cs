using Gomez.Core.Services;

namespace Gomez.FactorioService.Services
{
    public interface IGameService : IProcessBaseService, IDisposable
    {
        string ProcessName { get; set; }

        void KillExistingProcesses();
        Task WriteToChatAsync(string message);
    }
}