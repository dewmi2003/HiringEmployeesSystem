using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Recruitment.Infrastructure.Storage;

namespace Recruitment.Infrastructure.Services
{
    public class AzureBlobStorage : IBlobStorage
    {
        private readonly BlobContainerClient _container;
        private readonly ILogger<AzureBlobStorage> _logger;

        public AzureBlobStorage(
            IOptions<StorageSettings> settings,
            ILogger<AzureBlobStorage> logger)
        {
            _logger = logger;

            var client = new BlobServiceClient(settings.Value.ConnectionString);
            _container = client.GetBlobContainerClient(settings.Value.ContainerName);
            _container.CreateIfNotExists(PublicAccessType.None);
        }

        public async Task<string> UploadAsync(
            Stream stream,
            string contentType,
            string fileName)
        {
            var blobName = $"{Guid.NewGuid()}_{fileName}";
            var blob = _container.GetBlobClient(blobName);

            await blob.UploadAsync(
                stream,
                new BlobHttpHeaders
                {
                    ContentType = contentType
                });

            _logger.LogInformation("Uploaded blob {BlobName}", blobName);

            return blob.Name;
        }

        public async Task<Stream?> DownloadAsync(string blobPath)
        {
            var blob = _container.GetBlobClient(blobPath);

            if (!await blob.ExistsAsync())
            {
                return null;
            }

            var memory = new MemoryStream();
            await blob.DownloadToAsync(memory);
            memory.Position = 0;

            return memory;
        }

        public async Task DeleteAsync(string blobPath)
        {
            var blob = _container.GetBlobClient(blobPath);
            await blob.DeleteIfExistsAsync();

            _logger.LogInformation("Deleted blob {BlobPath}", blobPath);
        }
    }
}
