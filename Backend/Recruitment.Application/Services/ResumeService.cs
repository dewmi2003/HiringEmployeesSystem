using Recruitment.Application.DTO.Resumes;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;
using Recruitment.Domain.Entities;

namespace Recruitment.Application.Services
{
    public class ResumeService : IResumeService
    {
        private readonly IResumeRepository _resumeRepository;
        private readonly IStorageService _storage;

        public ResumeService(
            IResumeRepository resumeRepository,
            IStorageService storage)
        {
            _resumeRepository = resumeRepository;
            _storage = storage;
        }

        public async Task<ResumeDto> UploadResumeAsync(
            Guid candidateId,
            Stream fileStream,
            string fileName,
            string contentType,
            long fileSize)
        {
            var existingResumes = await _resumeRepository.GetAllAsync();
            var candidateResumes = existingResumes
                .Where(x => x.CandidateId == candidateId)
                .ToList();

            foreach (var oldResume in candidateResumes.Where(x => x.IsActive))
            {
                oldResume.IsActive = false;
                await _resumeRepository.UpdateAsync(oldResume);
            }

            var version = candidateResumes.Count == 0
                ? 1
                : candidateResumes.Max(x => x.Version) + 1;

            var uniqueName = $"{Guid.NewGuid()}_{fileName}";
            var filePath = await _storage.UploadAsync(
                fileStream,
                uniqueName,
                contentType);

            var now = DateTime.UtcNow;
            var resume = new Resume
            {
                Id = Guid.NewGuid(),
                CandidateId = candidateId,
                FileName = fileName,
                FilePath = filePath,
                FileType = contentType,
                FileSize = fileSize,
                Version = version,
                IsActive = true,
                IsDeleted = false,
                UploadedDate = now,
                CreatedAt = now
            };

            await _resumeRepository.AddAsync(resume);

            return MapToDto(resume);
        }

        public async Task<ResumeDto> UpdateResumeAsync(
            Guid resumeId,
            Stream fileStream,
            string fileName,
            string contentType,
            long fileSize)
        {
            var existing = await _resumeRepository.GetByIdAsync(resumeId)
                ?? throw new ArgumentException("Resume not found.");

            return await UploadResumeAsync(
                existing.CandidateId,
                fileStream,
                fileName,
                contentType,
                fileSize);
        }

        public async Task<(Stream FileStream, string ContentType, string FileName)> DownloadResumeAsync(
            Guid id)
        {
            var resume = await _resumeRepository.GetByIdAsync(id)
                ?? throw new ArgumentException("Resume not found.");

            var fileStream = await _storage.DownloadAsync(resume.FilePath)
                ?? throw new FileNotFoundException("Resume file not found in storage.");

            return (fileStream, GetContentType(resume.FileType), resume.FileName);
        }

        public async Task DeleteResumeAsync(Guid id)
        {
            var resume = await _resumeRepository.GetByIdAsync(id)
                ?? throw new ArgumentException("Resume not found.");

            await _storage.DeleteAsync(resume.FilePath);
            await _resumeRepository.DeleteAsync(resume);
        }

        public async Task SoftDeleteResumeAsync(Guid id)
        {
            var resume = await _resumeRepository.GetByIdAsync(id)
                ?? throw new ArgumentException("Resume not found.");

            resume.IsDeleted = true;
            resume.IsActive = false;
            await _resumeRepository.UpdateAsync(resume);

            var latestResume = (await _resumeRepository.GetAllAsync())
                .Where(x => x.CandidateId == resume.CandidateId && x.Id != id && !x.IsDeleted)
                .OrderByDescending(x => x.Version)
                .FirstOrDefault();

            if (latestResume != null)
            {
                latestResume.IsActive = true;
                await _resumeRepository.UpdateAsync(latestResume);
            }
        }

        public async Task<IEnumerable<ResumeDto>> GetVersionHistoryAsync(Guid candidateId)
        {
            return (await _resumeRepository.GetAllAsync())
                .Where(x => x.CandidateId == candidateId)
                .OrderByDescending(x => x.Version)
                .Select(MapToDto);
        }

        public async Task<IEnumerable<ResumeDto>> SearchResumesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Enumerable.Empty<ResumeDto>();
            }

            return (await _resumeRepository.GetAllAsync())
                .Where(x =>
                    x.FileName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    x.ParsedText.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .Select(MapToDto);
        }

        public async Task<ResumeDto?> GetByIdAsync(Guid id)
        {
            var resume = await _resumeRepository.GetByIdAsync(id);

            return resume == null
                ? null
                : MapToDto(resume);
        }

        private static string GetContentType(string fileType)
        {
            if (fileType.Contains('/'))
            {
                return fileType;
            }

            return fileType.ToLowerInvariant() switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }

        private static ResumeDto MapToDto(Resume resume)
        {
            return new ResumeDto
            {
                Id = resume.Id,
                CandidateId = resume.CandidateId,
                FileName = resume.FileName,
                FilePath = resume.FilePath,
                FileType = resume.FileType,
                FileSize = resume.FileSize,
                UploadedDate = resume.UploadedDate == default
                    ? resume.CreatedAt
                    : resume.UploadedDate,
                Version = resume.Version,
                IsActive = resume.IsActive,
                IsDeleted = resume.IsDeleted
            };
        }
    }
}
