using Microsoft.Extensions.Logging;
using Recruitment.Application.Interfaces.Services;

namespace Recruitment.Infrastructure.Storage
{
    public class LocalStorageService : IStorageService
    {
        private readonly string _basePath;
        private readonly ILogger<LocalStorageService> _logger;

        public LocalStorageService(ILogger<LocalStorageService> logger)
        {
            _logger = logger;
            _basePath = Path.Combine(Directory.GetCurrentDirectory(), "blobs");
            Directory.CreateDirectory(_basePath);
        }

        public async Task<string> UploadAsync(Stream stream, string fileName, string contentType)
        {
            var path = GetPath(fileName);

            await using var fileStream = File.Create(path);
            await stream.CopyToAsync(fileStream);

            _logger.LogInformation("Saved resume file to {Path}", path);
            return path;
        }

        public Task<Stream?> DownloadAsync(string fileName)
        {
            var path = GetPath(fileName);
            if (!File.Exists(path))
            {
                return Task.FromResult<Stream?>(null);
            }

            return Task.FromResult<Stream?>(File.OpenRead(path));
        }

        public Task DeleteAsync(string fileName)
        {
            var path = GetPath(fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            return Task.CompletedTask;
        }

        private string GetPath(string fileName)
        {
            return Path.IsPathRooted(fileName)
                ? fileName
                : Path.Combine(_basePath, Path.GetFileName(fileName));
        }
    }
}
