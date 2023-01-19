using Gomez.Factorio.Models;

namespace Gomez.Factorio.Services
{
    public interface IModHttpClient
    {
        Task<bool> PostInitAsync(ModInfo info, string zipFilePath);
    }
}