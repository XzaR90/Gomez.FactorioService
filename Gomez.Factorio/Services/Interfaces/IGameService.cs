using Gomez.Core.Services;

namespace Gomez.Factorio.Services.Interfaces
{
    public interface IGameService : IProcessBaseService, IDisposable
    {
        string ProcessName { get; set; }

        void KillExistingProcesses();

        Task WriteToChatAsync(string message);
    }
}