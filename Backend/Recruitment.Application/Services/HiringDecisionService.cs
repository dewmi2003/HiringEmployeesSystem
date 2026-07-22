using Recruitment.Application.DTOs;
using Recruitment.Application.DTOs.HiringDecisions;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;
using Recruitment.Domain.Entities;

namespace Recruitment.Application.Services
{
    public class HiringDecisionService : IHiringDecisionService
    {
        private readonly IHiringDecisionRepository _decisionRepo;
        private readonly IApplicationRepository _appRepo;
        private readonly IApplicationStatusHistoryRepository _historyRepo;
        private readonly IUserRepository _userRepo;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;


        public HiringDecisionService(
            IHiringDecisionRepository decisionRepo,
            IApplicationRepository appRepo,
            IApplicationStatusHistoryRepository historyRepo,
            IUserRepository userRepo,
            IEmailService emailService,
            INotificationService notificationService)
        {
            _decisionRepo = decisionRepo;
            _appRepo = appRepo;
            _historyRepo = historyRepo;
            _userRepo = userRepo;
            _emailService = emailService;
            _notificationService = notificationService;
        }



        public Task<HiringDecisionDto> ShortlistAsync(
            HiringDecisionActionDto dto)
            => MakeDecisionAsync(dto, "Shortlisted");



        public Task<HiringDecisionDto> RejectAsync(
            HiringDecisionActionDto dto)
            => MakeDecisionAsync(dto, "Rejected");



        public Task<HiringDecisionDto> OfferAsync(
            HiringDecisionActionDto dto)
            => MakeDecisionAsync(dto, "OfferExtended");



        public Task<HiringDecisionDto> HireAsync(
            HiringDecisionActionDto dto)
            => MakeDecisionAsync(dto, "Hired");



        public async Task<IEnumerable<HiringDecisionDto>> GetAllAsync()
        {
            var decisions = await _decisionRepo.GetAllAsync();

            var result = new List<HiringDecisionDto>();

            foreach(var item in decisions)
            {
                result.Add(await MapToDtoAsync(item));
            }

            return result;
        }



        public async Task<HiringDecisionDto?> GetByIdAsync(Guid id)
        {
            var decision =
                await _decisionRepo.GetByIdAsync(id);


            if(decision == null)
                return null;


            return await MapToDtoAsync(decision);
        }




        public async Task<IEnumerable<HiringDecisionDto>> 
            GetByApplicationIdAsync(Guid applicationId)
        {
            var decisions =
                await _decisionRepo
                .GetByApplicationIdAsync(applicationId);


            var result = new List<HiringDecisionDto>();


            foreach(var item in decisions)
            {
                result.Add(await MapToDtoAsync(item));
            }


            return result;
        }




        private async Task<HiringDecisionDto> MakeDecisionAsync(
            HiringDecisionActionDto dto,
            string decision)
        {

            var application =
                await _appRepo.GetByIdAsync(dto.ApplicationId)
                ?? throw new KeyNotFoundException("Application not found");


            string oldStatus = application.Status;


            application.Status = decision;


            await _appRepo.UpdateAsync(application);



            var history = new ApplicationStatusHistory
            {
                Id = Guid.NewGuid(),
                ApplicationId = dto.ApplicationId,
                OldStatus = oldStatus,
                NewStatus = decision,
                ChangedByUserId = dto.DecidedByUserId,
                ChangedAt = DateTime.UtcNow,
                Comments = dto.Comments
            };


            await _historyRepo.AddAsync(history);



            var hiringDecision = new HiringDecision
            {
                Id = Guid.NewGuid(),
                ApplicationId = dto.ApplicationId,
                DecidedByUserId = dto.DecidedByUserId,
                Decision = decision,
                Comments = dto.Comments,
                DecidedAt = DateTime.UtcNow
            };


            await _decisionRepo.AddAsync(hiringDecision);

            await NotifyCandidateAsync(
                application,
                decision,
                dto.Comments);

            return await MapToDtoAsync(hiringDecision);
        }




        private async Task NotifyCandidateAsync(
            Recruitment.Domain.Entities.Application application,
            string decision,
            string? comments)
        {
            var jobTitle = application.Job?.Title ?? "your job application";
            var candidateName = BuildCandidateName(application);
            var candidateUserId = application.Candidate?.UserId;

            if (candidateUserId.HasValue
                && candidateUserId.Value != Guid.Empty)
            {
                await _notificationService.CreateAsync(new NotificationDto
                {
                    UserId = candidateUserId.Value,
                    Title = BuildNotificationTitle(decision),
                    Message = $"Your application for {jobTitle} is now {decision}.",
                    Type = "Application"
                });
            }

            var candidateEmail = application.Candidate?.User?.Email;
            if (string.IsNullOrWhiteSpace(candidateEmail))
            {
                return;
            }

            var isOffer = string.Equals(
                decision,
                "OfferExtended",
                StringComparison.OrdinalIgnoreCase);

            await _emailService.SendEmailAsync(
                candidateEmail,
                isOffer
                    ? $"Job offer - {jobTitle}"
                    : $"Application status updated - {decision}",
                isOffer
                    ? EmailTemplateBuilder.OfferExtended(
                        candidateName,
                        jobTitle,
                        application.Job?.Company?.Name,
                        comments)
                    : EmailTemplateBuilder.ApplicationStatusUpdated(
                        candidateName,
                        jobTitle,
                        decision));
        }



        private static string BuildCandidateName(
            Recruitment.Domain.Entities.Application application)
        {
            if (application.Candidate == null)
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



        private static string BuildNotificationTitle(string decision)
            => decision switch
            {
                "OfferExtended" => "Job offer received",
                "Shortlisted" => "Application shortlisted",
                "Rejected" => "Application rejected",
                "Hired" => "Application approved",
                _ => "Application status updated"
            };




        private async Task<HiringDecisionDto> MapToDtoAsync(
            HiringDecision d)
        {

            var application = d.Application 
                ?? await _appRepo.GetByIdAsync(d.ApplicationId);


            var user =
                d.DecidedByUser
                ?? await _userRepo.GetByIdAsync(d.DecidedByUserId);



            return new HiringDecisionDto(
                d.Id,
                d.ApplicationId,
                application?.Candidate != null
                ? $"{application.Candidate.FirstName} {application.Candidate.LastName}"
                : "",
                application?.Job?.Title ?? "",
                d.DecidedByUserId,
                user?.FullName ?? "",
                d.Decision,
                d.Comments,
                d.DecidedAt
            );
        }
    }
}
