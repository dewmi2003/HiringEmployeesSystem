using Recruitment.Application.DTOs.Dashboard;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;

namespace Recruitment.Application.Services
{
    public class HiringManagerDashboardService 
        : IHiringManagerDashboardService
    {

        private readonly IHiringManagerDashboardRepository _repository;


        public HiringManagerDashboardService(
            IHiringManagerDashboardRepository repository)
        {
            _repository = repository;
        }


        public async Task<HiringManagerDashboardDto> GetDashboardAsync()
        {
            return await _repository.GetDashboardAsync();
        }
    }
}