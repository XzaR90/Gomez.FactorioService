namespace Gomez.Factorio.Services
{
    public interface IModService : IDisposable
    {
        event EventHandler? ModPublished;

        Task StartAsync(CancellationToken ct);
    }
}