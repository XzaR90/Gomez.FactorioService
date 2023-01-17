namespace Gomez.Factorio.DataTransmitter
{
    public interface IFileTransfer
    {
        Task CreateIfNotExistsAsync(CancellationToken ct = default);

        Task UploadAsync(IEnumerable<string> filePaths, CancellationToken ct = default);
    }
}