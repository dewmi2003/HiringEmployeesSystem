using Recruitment.Application.DTOs.Dashboard;

namespace Recruitment.Application.Interfaces.Services
{
    public interface IHiringManagerDashboardService
    {
        Task<HiringManagerDashboardDto> GetDashboardAsync();
    }
}