using Recruitment.Application.DTOs.HiringDecisions;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;

namespace Recruitment.Application.Services
{
    public class ApplicationStatusHistoryService : IApplicationStatusHistoryService
    {
        private readonly IApplicationStatusHistoryRepository _historyRepo;
        private readonly IUserRepository _userRepo;

        public ApplicationStatusHistoryService(
            IApplicationStatusHistoryRepository historyRepo,
            IUserRepository userRepo)
        {
            _historyRepo = historyRepo;
            _userRepo = userRepo;
        }

        public async Task<IEnumerable<ApplicationStatusHistoryDto>> GetByApplicationIdAsync(Guid applicationId)
        {
            var items = await _historyRepo.GetByApplicationIdAsync(applicationId);
            var dtos = new List<ApplicationStatusHistoryDto>();

            foreach (var h in items)
            {
                var user = h.ChangedByUser ?? await _userRepo.GetByIdAsync(h.ChangedByUserId);
                dtos.Add(new ApplicationStatusHistoryDto(
                    h.Id,
                    h.ApplicationId,
                    h.OldStatus,
                    h.NewStatus,
                    h.ChangedByUserId,
                    user?.FullName ?? string.Empty,
                    h.ChangedAt,
                    h.Comments
                ));
            }

            return dtos;
        }
    }
}
