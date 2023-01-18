using Gomez.Factorio.Models;

namespace Gomez.Factorio.Services
{
    public interface IModHttpClient
    {
        Task PostInitAsync(ModInfo info, string zipFilePath);
    }
}