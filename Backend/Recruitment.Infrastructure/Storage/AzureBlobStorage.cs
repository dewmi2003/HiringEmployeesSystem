using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Recruitment.Infrastructure.Services
{
    public class AzureBlobStorage : IBlobStorage
    {
        private readonly BlobContainerClient _container;
        private readonly ILogger<AzureBlobStorage> _logger;

        public AzureBlobStorage(IConfiguration config, ILogger<AzureBlobStorage> logger)
        {
            _logger = logger;
            var conn = config["Azure:Storage:ConnectionString"] ?? config["AzureStorageConnectionString"];
            var containerName = config["Azure:Storage:Container"] ?? "uploads";
            var client = new BlobServiceClient(conn);
            _container = client.GetBlobContainerClient(containerName);
            _container.CreateIfNotExists(PublicAccessType.None);
        }

        public async Task<string> UploadAsync(Stream stream, string contentType, string fileName)
        {
            var blob = _container.GetBlobClient(fileName);
            await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType });
            _logger.LogInformation("Uploaded blob {Name}", fileName);
            return blob.Uri.ToString();
        }

        public async Task<Stream?> DownloadAsync(string blobPath)
        {
            var blob = _container.GetBlobClient(blobPath);
            if (!await blob.ExistsAsync()) return null;
            var ms = new MemoryStream();
            await blob.DownloadToAsync(ms);
            ms.Position = 0;
            return ms;
        }

        public async Task DeleteAsync(string blobPath)
        {
            var blob = _container.GetBlobClient(blobPath);
            await blob.DeleteIfExistsAsync();
        }
    }
}
