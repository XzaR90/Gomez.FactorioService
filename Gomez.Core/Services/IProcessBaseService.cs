namespace Gomez.Core.Services
{
    public interface IProcessBaseService
    {
        Task RunAsync(CancellationToken ct);
    }
}