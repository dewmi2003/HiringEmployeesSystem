using Recruitment.Application.DTOs.Dashboard;

namespace Recruitment.Application.Interfaces.Repositories
{
    public interface IHiringManagerDashboardRepository
    {
        Task<HiringManagerDashboardDto> GetDashboardAsync();
    }
}