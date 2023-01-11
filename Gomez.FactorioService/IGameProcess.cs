namespace Gomez.FactorioService
{
    public interface IGameProcess : IDisposable
    {
        string ProcessName { get; set; }

        bool SafeClosed { get; }

        Task StartAsync(CancellationToken ct);
        Task WriteToChatAsync(string message);
    }
}