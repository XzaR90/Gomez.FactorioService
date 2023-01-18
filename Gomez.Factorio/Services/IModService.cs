namespace Gomez.Factorio.Services
{
    public interface IModService : IDisposable
    {
        Task StartAsync(CancellationToken ct);
    }
}