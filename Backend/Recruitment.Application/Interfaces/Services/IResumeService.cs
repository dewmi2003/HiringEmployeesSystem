using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Recruitment.Application.DTO.Resumes;

namespace Recruitment.Application.Interfaces.Services
{
    public interface IResumeService
    {
        Task<ResumeDto> UploadResumeAsync(Guid candidateId, Stream fileStream, string fileName, string contentType, long fileSize);
        Task<ResumeDto> UpdateResumeAsync(Guid resumeId, Stream fileStream, string fileName, string contentType, long fileSize);
        Task<(Stream FileStream, string ContentType, string FileName)> DownloadResumeAsync(Guid id);
        Task DeleteResumeAsync(Guid id);
        Task SoftDeleteResumeAsync(Guid id);
        Task<IEnumerable<ResumeDto>> GetVersionHistoryAsync(Guid candidateId);
        Task<IEnumerable<ResumeDto>> SearchResumesAsync(string searchTerm);
        Task<ResumeDto?> GetByIdAsync(Guid id);
    }
}
