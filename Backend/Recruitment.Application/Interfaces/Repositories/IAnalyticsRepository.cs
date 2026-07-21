using Recruitment.Application.DTOs.Analytics;

namespace Recruitment.Application.Interfaces.Repositories
{
    public interface IAnalyticsRepository
    {
        Task<List<HiringTrendDto>> GetHiringTrendsAsync();

        Task<List<SkillAnalyticsDto>> GetTopSkillsAsync();

        Task<PerformanceAnalyticsDto> GetPerformanceAsync();
    }
}
