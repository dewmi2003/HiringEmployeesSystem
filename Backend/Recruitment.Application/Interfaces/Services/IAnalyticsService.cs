using Recruitment.Application.DTOs.Analytics;

namespace Recruitment.Application.Interfaces.Services
{
    public interface IAnalyticsService
    {
        Task<List<HiringTrendDto>> GetHiringTrendsAsync();

        Task<List<SkillAnalyticsDto>> GetTopSkillsAsync();

        Task<PerformanceAnalyticsDto> GetPerformanceAsync();
    }
}
