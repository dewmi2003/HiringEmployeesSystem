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

        // Allowed status transitions
        private static readonly Dictionary<string, string> ActionToStatus = new()
        {
            { "Shortlisted",    "Shortlisted" },
            { "Rejected",       "Rejected" },
            { "OfferExtended",  "OfferExtended" },
            { "Hired",          "Hired" }
        };

        public HiringDecisionService(
            IHiringDecisionRepository decisionRepo,
            IApplicationRepository appRepo,
            IApplicationStatusHistoryRepository historyRepo,
            IUserRepository userRepo)
        {
            _decisionRepo = decisionRepo;
            _appRepo = appRepo;
            _historyRepo = historyRepo;
            _userRepo = userRepo;
        }

        public Task<HiringDecisionDto> ShortlistAsync(HiringDecisionActionDto dto)
            => MakeDecisionAsync(dto, "Shortlisted");

        public Task<HiringDecisionDto> RejectAsync(HiringDecisionActionDto dto)
            => MakeDecisionAsync(dto, "Rejected");

        public Task<HiringDecisionDto> OfferAsync(HiringDecisionActionDto dto)
            => MakeDecisionAsync(dto, "OfferExtended");

        public Task<HiringDecisionDto> HireAsync(HiringDecisionActionDto dto)
            => MakeDecisionAsync(dto, "Hired");

        public async Task<IEnumerable<HiringDecisionDto>> GetAllAsync()
        {
            var all = await _decisionRepo.GetAllAsync();
            var dtos = new List<HiringDecisionDto>();
            foreach (var d in all)
                dtos.Add(await MapToDtoAsync(d));
            return dtos;
        }

        public async Task<HiringDecisionDto?> GetByIdAsync(Guid id)
        {
            var d = await _decisionRepo.GetByIdAsync(id);
            return d == null ? null : await MapToDtoAsync(d);
        }

        public async Task<IEnumerable<HiringDecisionDto>> GetByApplicationIdAsync(Guid applicationId)
        {
            var items = await _decisionRepo.GetByApplicationIdAsync(applicationId);
            var dtos = new List<HiringDecisionDto>();
            foreach (var d in items)
                dtos.Add(await MapToDtoAsync(d));
            return dtos;
        }

        // ─── Core private logic ──────────────────────────────────────────────

        private async Task<HiringDecisionDto> MakeDecisionAsync(HiringDecisionActionDto dto, string decision)
        {
            var application = await _appRepo.GetByIdAsync(dto.ApplicationId)
                ?? throw new KeyNotFoundException($"Application {dto.ApplicationId} not found.");

            var user = await _userRepo.GetByIdAsync(dto.DecidedByUserId)
                ?? throw new KeyNotFoundException($"User {dto.DecidedByUserId} not found.");

            string oldStatus = application.Status;
            application.Status = decision;
            await _appRepo.UpdateAsync(application);

            // Record history
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

            // Persist hiring decision
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

            return await MapToDtoAsync(hiringDecision);
        }

        private async Task<HiringDecisionDto> MapToDtoAsync(HiringDecision d)
        {
            var app = d.Application ?? await _appRepo.GetByIdAsync(d.ApplicationId);
            var user = d.DecidedByUser ?? await _userRepo.GetByIdAsync(d.DecidedByUserId);
            string candidateName = app?.Candidate != null
                ? $"{app.Candidate.FirstName} {app.Candidate.LastName}"
                : string.Empty;
            string jobTitle = app?.Job?.Title ?? string.Empty;

            return new HiringDecisionDto(
                d.Id,
                d.ApplicationId,
                candidateName,
                jobTitle,
                d.DecidedByUserId,
                user?.FullName ?? string.Empty,
                d.Decision,
                d.Comments,
                d.DecidedAt
            );
        }
    }
}
