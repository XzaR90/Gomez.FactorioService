using Gomez.Core.Services;

namespace Gomez.FactorioService.Services
{
    public interface IGameService : IProcessBaseService
    {
        string ProcessName { get; set; }

        void KillExistingProcesses();
    }
}