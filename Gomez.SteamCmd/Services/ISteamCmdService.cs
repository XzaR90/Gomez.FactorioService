using Gomez.Core.Services;
using Gomez.SteamCmd.Models;

namespace Gomez.SteamCmd.Services
{
    public interface ISteamCmdService : IProcessBaseService
    {
        SteamCmdState State { get; }
    }
}