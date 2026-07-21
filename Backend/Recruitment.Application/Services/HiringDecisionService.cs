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
                ?? throw new Exception("Application not found");


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


            return await MapToDtoAsync(hiringDecision);
        }




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