using ApplicationEntity = Recruitment.Domain.Entities.Application;
using Recruitment.Application.DTOs.Applications;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;

namespace Recruitment.Application.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IEmailService _emailService;


        public ApplicationService(
            IApplicationRepository applicationRepository,
            IJobRepository jobRepository,
            IEmailService emailService)
        {
            _applicationRepository = applicationRepository;
            _jobRepository = jobRepository;
            _emailService = emailService;
        }



        public async Task<IEnumerable<ApplicationEntity>> GetAllApplicationsAsync()
        {
            return await _applicationRepository.GetAllAsync();
        }



        public async Task<ApplicationEntity?> GetApplicationByIdAsync(Guid id)
        {
            return await _applicationRepository.GetByIdAsync(id);
        }



        public async Task<IEnumerable<ApplicationDetailDto>> GetApplicationsByJobAsync(Guid jobId)
        {
            var applications = await _applicationRepository.GetByJobIdAsync(jobId);

            return applications.Select(a => new ApplicationDetailDto(
                a.Id,
                a.CandidateId,
                a.Candidate == null
                    ? string.Empty
                    : $"{a.Candidate.FirstName} {a.Candidate.LastName}".Trim(),
                a.Candidate?.User?.Email ?? string.Empty,
                a.JobId,
                a.Job?.Title ?? string.Empty,
                a.Status,
                a.AppliedDate));
        }



        public async Task<IEnumerable<ApplicationListDto>> GetApplicationsByCandidateAsync(Guid candidateId)
        {
            var applications = await _applicationRepository.GetByCandidateIdAsync(candidateId);

            return applications.Select(a => new ApplicationListDto(
                a.Id,
                a.JobId,
                a.Job?.Title ?? string.Empty,
                a.Job?.Company?.Name ?? string.Empty,
                a.Status,
                a.AppliedDate));
        }



        public async Task<Guid> ApplyToJobAsync(Guid candidateId, CreateApplicationDto dto)
        {
            var job = await _jobRepository.GetByIdAsync(dto.JobId);
            if (job == null)
            {
                throw new ArgumentException("Job not found.");
            }

            var existingApplications = await _applicationRepository.GetByCandidateIdAsync(candidateId);
            if (existingApplications.Any(a => a.JobId == dto.JobId))
            {
                throw new InvalidOperationException("Candidate has already applied to this job.");
            }

            var application = new ApplicationEntity
            {
                Id = Guid.NewGuid(),
                CandidateId = candidateId,
                JobId = dto.JobId,
                AppliedDate = DateTime.UtcNow,
                Status = "Pending"
            };

            await _applicationRepository.AddAsync(application);

            return application.Id;
        }



        public async Task UpdateStatusAsync(Guid applicationId, UpdateApplicationStatusDto dto)
        {
            var allowedStatuses = new[] { "Pending", "Shortlisted", "Rejected", "OfferExtended", "Hired", "Withdrawn" };
            if (!allowedStatuses.Contains(dto.Status, StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Invalid application status.");
            }

            var application = await _applicationRepository.GetByIdAsync(applicationId);

            if (application == null)
                throw new KeyNotFoundException("Application not found.");


            application.Status = allowedStatuses.First(x => x.Equals(dto.Status, StringComparison.OrdinalIgnoreCase));

            await _applicationRepository.UpdateAsync(application);

            var candidateEmail = application.Candidate?.User?.Email;
            if (!string.IsNullOrWhiteSpace(candidateEmail))
            {
                var jobTitle = application.Job?.Title ?? "your job application";
                var body = $"""
                    <p>Hello {application.Candidate?.FirstName ?? "Candidate"},</p>
                    <p>Your application for <strong>{jobTitle}</strong> has been updated.</p>
                    <p>New status: <strong>{application.Status}</strong></p>
                    <p>Thank you,<br/>Recruitment Team</p>
                    """;

                await _emailService.SendEmailAsync(
                    candidateEmail,
                    $"Application status updated: {application.Status}",
                    body);
            }
        }



        public async Task UpdateApplicationAsync(ApplicationEntity application)
        {
            await _applicationRepository.UpdateAsync(application);
        }



        public async Task DeleteApplicationAsync(Guid id)
        {
            await _applicationRepository.DeleteAsync(id);
        }
    }
}
