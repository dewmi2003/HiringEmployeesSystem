using System.IO;

namespace Recruitment.Application.Interfaces.Services
{
    public interface IStorageService
    {
        Task<string> UploadAsync(
            Stream stream,
            string fileName,
            string contentType);


        Task<Stream?> DownloadAsync(
            string fileName);


        Task DeleteAsync(
            string fileName);
    }
}