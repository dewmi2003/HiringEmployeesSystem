using ApplicationEntity = Recruitment.Domain.Entities.Application;
using Recruitment.Application.DTOs;
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
        private readonly INotificationService _notificationService;


        public ApplicationService(
            IApplicationRepository applicationRepository,
            IJobRepository jobRepository,
            IEmailService emailService,
            INotificationService notificationService)
        {
            _applicationRepository = applicationRepository;
            _jobRepository = jobRepository;
            _emailService = emailService;
            _notificationService = notificationService;
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

            var savedApplication = await _applicationRepository.GetByIdAsync(application.Id);
            var candidateEmail = savedApplication?.Candidate?.User?.Email;

            if (!string.IsNullOrWhiteSpace(candidateEmail))
            {
                await _emailService.SendEmailAsync(
                    candidateEmail,
                    $"Application received - {job.Title}",
                    EmailTemplateBuilder.ApplicationSubmitted(
                        BuildCandidateName(savedApplication),
                        job.Title,
                        savedApplication?.Job?.Company?.Name,
                        application.AppliedDate));
            }

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

            var candidateUserId = application.Candidate?.UserId;
            var statusJobTitle = application.Job?.Title ?? "your job application";
            if (candidateUserId.HasValue && candidateUserId.Value != Guid.Empty)
            {
                await _notificationService.CreateAsync(new NotificationDto
                {
                    UserId = candidateUserId.Value,
                    Title = "Application status updated",
                    Message = $"Your application for {statusJobTitle} is now {application.Status}.",
                    Type = "Application"
                });
            }

            var candidateEmail = application.Candidate?.User?.Email;
            if (!string.IsNullOrWhiteSpace(candidateEmail))
            {
                var candidateName = BuildCandidateName(application);
                var isOffer = string.Equals(
                    application.Status,
                    "OfferExtended",
                    StringComparison.OrdinalIgnoreCase);

                await _emailService.SendEmailAsync(
                    candidateEmail,
                    isOffer
                        ? $"Job offer - {statusJobTitle}"
                        : $"Application status updated - {application.Status}",
                    isOffer
                        ? EmailTemplateBuilder.OfferExtended(
                            candidateName,
                            statusJobTitle,
                            application.Job?.Company?.Name,
                            null)
                        : EmailTemplateBuilder.ApplicationStatusUpdated(
                            candidateName,
                            statusJobTitle,
                            application.Status));
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

        private static string BuildCandidateName(ApplicationEntity? application)
        {
            if (application?.Candidate == null)
            {
                return "Candidate";
            }

            var fullName =
                $"{application.Candidate.FirstName} {application.Candidate.LastName}"
                .Trim();

            return string.IsNullOrWhiteSpace(fullName)
                ? "Candidate"
                : fullName;
        }
    }
}
