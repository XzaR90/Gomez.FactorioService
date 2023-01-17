namespace Gomez.Factorio.Services.Interfaces
{
    public interface IStatisticService
    {
        Task StartTransferAsync(CancellationToken ct);
    }
}