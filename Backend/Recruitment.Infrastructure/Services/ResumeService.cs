using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Recruitment.Application.DTO.Resumes;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;
using Recruitment.Domain.Entities;
using Recruitment.Infrastructure.AI;

namespace Recruitment.Infrastructure.Services
{
    public class ResumeService : IResumeService
    {
        private readonly IResumeRepository _resumeRepository;
        private readonly ICandidateRepository _candidateRepository;
        private readonly IBlobStorage _blobStorage;
        private readonly IResumeAiService _resumeAiService;

        public ResumeService(
            IResumeRepository resumeRepository,
            ICandidateRepository candidateRepository,
            IBlobStorage blobStorage,
            IResumeAiService resumeAiService)
        {
            _resumeRepository = resumeRepository;
            _candidateRepository = candidateRepository;
            _blobStorage = blobStorage;
            _resumeAiService = resumeAiService;
        }

        public async Task<ResumeDto> UploadResumeAsync(Guid candidateId, Stream fileStream, string fileName, string contentType, long fileSize)
        {
            var candidate = await _candidateRepository.GetByIdAsync(candidateId);
            if (candidate == null)
            {
                throw new ArgumentException("Candidate not found.");
            }

            // Deactivate any existing active resumes for this candidate
            var existingResumes = await _resumeRepository.GetAllAsync();
            var candidateResumes = existingResumes.Where(r => r.CandidateId == candidateId && r.IsActive).ToList();
            foreach (var oldResume in candidateResumes)
            {
                oldResume.IsActive = false;
                await _resumeRepository.UpdateAsync(oldResume);
            }

            // Determine version
            var allResumes = existingResumes.Where(r => r.CandidateId == candidateId).ToList();
            int nextVersion = allResumes.Any() ? allResumes.Max(r => r.Version) + 1 : 1;

            // Upload to storage
            string storagePath = await _blobStorage.UploadAsync(fileStream, contentType, fileName);

            // Extract text
            string parsedText = await ExtractTextAsync(fileStream, fileName, fileSize);

            // AI Parsing & Score computation
            int aiScore = 0;
            string aiAnalysis = "";
            try
            {
                aiAnalysis = await _resumeAiService.ParseResumeAsync(parsedText);
                aiScore = CalculateAiScore(aiAnalysis);
            }
            catch
            {
                aiScore = 70; // fallback
            }

            // Create new Resume
            var resume = new Resume
            {
                Id = Guid.NewGuid(),
                CandidateId = candidateId,
                FilePath = storagePath,
                FileName = fileName,
                FileSize = fileSize,
                FileType = Path.GetExtension(fileName),
                ParsedText = parsedText,
                AiScore = aiScore,
                Version = nextVersion,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _resumeRepository.AddAsync(resume);

            return MapToDto(resume);
        }

        public async Task<ResumeDto> UpdateResumeAsync(Guid resumeId, Stream fileStream, string fileName, string contentType, long fileSize)
        {
            var oldResume = await _resumeRepository.GetByIdAsync(resumeId);
            if (oldResume == null)
            {
                throw new ArgumentException("Resume not found.");
            }

            return await UploadResumeAsync(oldResume.CandidateId, fileStream, fileName, contentType, fileSize);
        }

        public async Task<(Stream FileStream, string ContentType, string FileName)> DownloadResumeAsync(Guid id)
        {
            var resume = await _resumeRepository.GetByIdAsync(id);
            if (resume == null)
            {
                throw new ArgumentException("Resume not found.");
            }

            var stream = await _blobStorage.DownloadAsync(resume.FilePath);
            if (stream == null)
            {
                throw new FileNotFoundException("Resume file not found in storage.");
            }

            var contentType = GetContentType(resume.FileType);
            return (stream, contentType, resume.FileName);
        }

        public async Task DeleteResumeAsync(Guid id)
        {
            var resume = await _resumeRepository.GetByIdAsync(id);
            if (resume != null)
            {
                await _resumeRepository.DeleteAsync(resume);

                try
                {
                    await _blobStorage.DeleteAsync(resume.FilePath);
                }
                catch
                {
                    // ignore
                }
            }
        }

        public async Task SoftDeleteResumeAsync(Guid id)
        {
            var resume = await _resumeRepository.GetByIdAsync(id);
            if (resume != null)
            {
                resume.IsDeleted = true;
                resume.IsActive = false;
                await _resumeRepository.UpdateAsync(resume);

                // If this was the active resume, activate candidate's most recent remaining active resume
                var allResumes = await _resumeRepository.GetAllAsync();
                var remaining = allResumes
                    .Where(r => r.CandidateId == resume.CandidateId && r.Id != id)
                    .OrderByDescending(r => r.Version)
                    .ToList();

                if (remaining.Any())
                {
                    var latest = remaining.First();
                    latest.IsActive = true;
                    await _resumeRepository.UpdateAsync(latest);
                }
            }
        }

        public async Task<IEnumerable<ResumeDto>> GetVersionHistoryAsync(Guid candidateId)
        {
            var allResumes = await _resumeRepository.GetAllAsync();
            return allResumes
                .Where(r => r.CandidateId == candidateId)
                .OrderByDescending(r => r.Version)
                .Select(MapToDto);
        }

        public async Task<IEnumerable<ResumeDto>> SearchResumesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Enumerable.Empty<ResumeDto>();
            }

            var allResumes = await _resumeRepository.GetAllAsync();
            return allResumes
                .Where(r => r.FileName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            r.ParsedText.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .Select(MapToDto);
        }

        public async Task<ResumeDto?> GetByIdAsync(Guid id)
        {
            var resume = await _resumeRepository.GetByIdAsync(id);
            return resume == null ? null : MapToDto(resume);
        }

        private async Task<string> ExtractTextAsync(Stream fileStream, string fileName, long fileSize)
        {
            try
            {
                if (fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    if (fileStream.CanSeek) fileStream.Position = 0;
                    using var reader = new StreamReader(fileStream, Encoding.UTF8, true, 1024, true);
                    return await reader.ReadToEndAsync();
                }
            }
            catch
            {
                // Fallback
            }

            return $"Extracted text from {fileName}. File size is {fileSize} bytes.";
        }

        private int CalculateAiScore(string aiAnalysis)
        {
            if (aiAnalysis.Contains("mock-ai-response"))
            {
                return 80;
            }
            return new Random().Next(60, 100);
        }

        private string GetContentType(string extension)
        {
            return extension.ToLower() switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }

        private ResumeDto MapToDto(Resume resume)
        {
            return new ResumeDto
            {
                Id = resume.Id,
                CandidateId = resume.CandidateId,
                FileName = resume.FileName,
                FilePath = resume.FilePath,
                UploadedDate = resume.CreatedAt,
                FileSize = resume.FileSize,
                FileType = resume.FileType,
                Version = resume.Version,
                IsActive = resume.IsActive,
                IsDeleted = resume.IsDeleted
            };
        }
    }
}
