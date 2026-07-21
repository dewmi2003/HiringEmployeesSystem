using Microsoft.EntityFrameworkCore;
using Recruitment.Application.DTOs.Dashboard;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Persistence.Context;

namespace Recruitment.Persistence.Repositories
{
    public class HiringManagerDashboardRepository 
        : IHiringManagerDashboardRepository
    {
        private readonly ApplicationDbContext _context;


        public HiringManagerDashboardRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<HiringManagerDashboardDto> GetDashboardAsync()
        {
            var dashboard = new HiringManagerDashboardDto();


            dashboard.PendingEvaluations =
                await _context.Evaluations
                .CountAsync(e => e.OverallScore == 0);


            dashboard.TotalInterviews =
                await _context.Interviews
                .CountAsync();


            dashboard.PendingInterviews =
                await _context.Interviews
                .CountAsync(i => i.Status == "Pending");


            dashboard.TotalHiringDecisions =
                await _context.HiringDecisions
                .CountAsync();


            dashboard.TotalOffers =
                await _context.HiringDecisions
                .CountAsync(x => x.Decision == "Offer");


            dashboard.TotalHiredCandidates =
                await _context.HiringDecisions
                .CountAsync(x => x.Decision == "Hired");


            return dashboard;
        }
    }
}