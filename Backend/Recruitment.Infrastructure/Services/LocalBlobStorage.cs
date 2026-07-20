using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace Recruitment.Infrastructure.Services
{
    public class LocalBlobStorage : IBlobStorage
    {
        private readonly string _basePath = Path.Combine(Directory.GetCurrentDirectory(), "blobs");
        private readonly ILogger<LocalBlobStorage> _logger;

        public LocalBlobStorage(ILogger<LocalBlobStorage> logger)
        {
            _logger = logger;
            Directory.CreateDirectory(_basePath);
        }

        public async Task<string> UploadAsync(Stream stream, string contentType, string fileName)
        {
            var path = Path.Combine(_basePath, fileName);
            using var fileStream = File.Create(path);
            await stream.CopyToAsync(fileStream);
            _logger.LogInformation("Saved blob to {Path}", path);
            return path;
        }

        public Task<Stream?> DownloadAsync(string blobPath)
        {
            if (!File.Exists(blobPath)) return Task.FromResult<Stream?>(null);
            Stream s = File.OpenRead(blobPath);
            return Task.FromResult<Stream?>(s);
        }

        public Task DeleteAsync(string blobPath)
        {
            if (File.Exists(blobPath)) File.Delete(blobPath);
            return Task.CompletedTask;
        }
    }
}
