using Recruitment.Application.DTOs.Analytics;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;

namespace Recruitment.Application.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IAnalyticsRepository _repository;


        public AnalyticsService(
            IAnalyticsRepository repository)
        {
            _repository = repository;
        }



        public async Task<List<HiringTrendDto>> GetHiringTrendsAsync()
        {
            return await _repository.GetHiringTrendsAsync();
        }



        public async Task<List<SkillAnalyticsDto>> GetTopSkillsAsync()
        {
            return await _repository.GetTopSkillsAsync();
        }



        public async Task<PerformanceAnalyticsDto> GetPerformanceAsync()
        {
            return await _repository.GetPerformanceAsync();
        }
    }
}