using Microsoft.EntityFrameworkCore;
using Recruitment.Application.DTOs.Jobs;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;
using Recruitment.Domain.Entities;

namespace Recruitment.Application.Services
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepo;

        public JobService(IJobRepository jobRepo) => _jobRepo = jobRepo;

        public async Task<List<JobListDto>> GetAllActiveJobsAsync(string? location = null, string? title = null)
        {
            var jobs = await _jobRepo.GetAllAsync();
            return jobs
                .Where(j => j.Status == "Active"
                    && (title    == null || j.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                    && (location == null || (j.Location != null && j.Location.Contains(location, StringComparison.OrdinalIgnoreCase))))
                .Select(j => new JobListDto(
                    j.Id,
                    j.Title,
                    j.Location,
                    j.Salary,
                    j.Status,
                    j.Company?.CompanyName ?? "",
                    j.CreatedDate))
                .ToList();
        }

        public async Task<JobDetailDto?> GetJobByIdAsync(Guid jobId)
        {
            var job = await _jobRepo.GetByIdAsync(jobId);
            if (job == null) return null;

            return new JobDetailDto(
                job.Id,
                job.Title,
                job.Description,
                job.Requirements,
                job.Salary,
                job.Location,
                job.Status,
                job.Company?.CompanyName ?? "",
                job.Company?.Website,
                job.CreatedDate);
        }

        public async Task<Guid> CreateJobAsync(Guid recruiterId, CreateJobDto dto)
        {
            var job = new Job
            {
                Id          = Guid.NewGuid(),
                RecruiterId = recruiterId,
                CompanyId   = dto.CompanyId,
                Title       = dto.Title,
                Description = dto.Description,
                Requirements= dto.Requirements,
                Salary      = dto.Salary,
                Location    = dto.Location,
                Status      = "Active",
                CreatedDate = DateTime.UtcNow
            };

            await _jobRepo.AddAsync(job);
            return job.Id;
        }

        public async Task UpdateJobAsync(Guid jobId, UpdateJobDto dto)
        {
            var job = await _jobRepo.GetByIdAsync(jobId)
                      ?? throw new KeyNotFoundException("Job not found.");

            if (dto.Title       != null) job.Title        = dto.Title;
            if (dto.Description != null) job.Description  = dto.Description;
            if (dto.Requirements!= null) job.Requirements = dto.Requirements;
            if (dto.Salary      != null) job.Salary       = dto.Salary;
            if (dto.Location    != null) job.Location     = dto.Location;
            if (dto.Status      != null) job.Status       = dto.Status;

            await _jobRepo.UpdateAsync(job);
        }

        public async Task DeleteJobAsync(Guid jobId)
        {
            var job = await _jobRepo.GetByIdAsync(jobId)
                      ?? throw new KeyNotFoundException("Job not found.");
            await _jobRepo.DeleteAsync(job);
        }
    }
}
