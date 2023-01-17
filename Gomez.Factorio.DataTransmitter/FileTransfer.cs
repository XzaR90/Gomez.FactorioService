using Azure.Storage.Blobs;

namespace Gomez.Factorio.DataTransmitter
{
    public class FileTransfer : IFileTransfer
    {
        private readonly BlobContainerClient _blobContainerClient;

        public FileTransfer(BlobContainerClient blobContainerClient)
        {
            _blobContainerClient = blobContainerClient;
        }

        public Task CreateIfNotExistsAsync(CancellationToken ct = default)
        {
            return _blobContainerClient.CreateIfNotExistsAsync(cancellationToken: ct);
        }

        public async Task UploadAsync(IEnumerable<string> filePaths, CancellationToken ct = default)
        {
            foreach (var filePath in filePaths)
            {
                await UploadSingleAsync(filePath, ct);
            }
        }

        private Task UploadSingleAsync(string filePath, CancellationToken ct = default)
        {
            string fileName = Path.GetFileName(filePath);
            BlobClient blobClient = _blobContainerClient.GetBlobClient(fileName);
            return blobClient.UploadAsync(filePath, true, ct);
        }
    }
}