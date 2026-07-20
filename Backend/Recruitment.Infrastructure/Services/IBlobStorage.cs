using System.IO;
using System.Threading.Tasks;

namespace Recruitment.Infrastructure.Services
{
    public interface IBlobStorage
    {
        Task<string> UploadAsync(Stream stream, string contentType, string fileName);
        Task<Stream?> DownloadAsync(string blobPath);
        Task DeleteAsync(string blobPath);
    }
}
