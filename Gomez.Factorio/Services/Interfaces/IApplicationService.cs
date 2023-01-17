namespace Gomez.Factorio.Services.Interfaces
{
    public interface IApplicationService
    {
        Task RunAsync();

        Task WaitUntilProcessClosedAsync();
    }
}