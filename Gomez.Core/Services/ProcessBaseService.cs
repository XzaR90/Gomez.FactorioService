namespace Gomez.Core.Services
{
    public abstract class ProcessBaseService : IProcessBaseService
    {
        public abstract Task RunAsync(CancellationToken ct);
    }
}