using Microsoft.EntityFrameworkCore;
using Recruitment.Application.DTOs.Analytics;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Persistence.Context;

namespace Recruitment.Persistence.Repositories
{
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly ApplicationDbContext _context;


        public AnalyticsRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<List<HiringTrendDto>> GetHiringTrendsAsync()
        {
            return await _context.HiringDecisions
                .Where(x => x.Decision == "Hired")
                .GroupBy(x => new
                {
                    x.DecidedAt.Year,
                    x.DecidedAt.Month
                })
                .Select(x => new HiringTrendDto
                {
                    Month =
                    $"{x.Key.Year}-{x.Key.Month}",

                    TotalHires =
                    x.Count()
                })
                .ToListAsync();
        }



        public async Task<List<SkillAnalyticsDto>> GetTopSkillsAsync()
        {
            return await _context.CandidateSkills
                .GroupBy(x => x.Skill == null ? "Unknown" : x.Skill.Name)
                .Select(x => new SkillAnalyticsDto
                {
                    SkillName = x.Key,

                    CandidateCount =
                    x.Count()
                })
                .OrderByDescending(x => x.CandidateCount)
                .Take(10)
                .ToListAsync();
        }



        public async Task<PerformanceAnalyticsDto> GetPerformanceAsync()
        {

            int applications =
                await _context.Applications.CountAsync();


            int hires =
                await _context.HiringDecisions
                .CountAsync(x => x.Decision == "Hired");



            return new PerformanceAnalyticsDto
            {
                TotalApplications = applications,

                TotalHires = hires,

                SuccessRate =
                applications == 0
                ? 0
                : ((double)hires / applications) * 100
            };
        }
    }
}
