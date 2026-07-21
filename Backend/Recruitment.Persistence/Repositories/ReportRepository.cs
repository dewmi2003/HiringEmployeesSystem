using Microsoft.EntityFrameworkCore;
using Recruitment.Application.DTOs.Reports;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Persistence.Context;

namespace Recruitment.Persistence.Repositories
{
    public class ReportRepository : IReportRepository
    {

        private readonly ApplicationDbContext _context;


        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<RecruitmentReportDto> GetRecruitmentReportAsync()
        {

            var hired =
                await _context.HiringDecisions
                .CountAsync(x => x.Decision=="Hired");


            var applications =
                await _context.Applications.CountAsync();


            return new RecruitmentReportDto
            {
                TotalCandidates =
                    await _context.Candidates.CountAsync(),

                TotalJobs =
                    await _context.Jobs.CountAsync(),

                TotalApplications =
                    applications,

                TotalInterviews =
                    await _context.Interviews.CountAsync(),

                TotalHired = hired,

                HiringRate =
                    applications == 0 
                    ? 0 
                    : (double)hired / applications * 100
            };
        }




        public async Task<CandidateReportDto> GetCandidateReportAsync()
        {
            return new CandidateReportDto
            {
                TotalCandidates =
                await _context.Candidates.CountAsync(),

                ActiveCandidates =
                await _context.Candidates.CountAsync(),

                CandidatesApplied =
                await _context.Applications
                .Select(x=>x.CandidateId)
                .Distinct()
                .CountAsync()
            };
        }




        public async Task<JobReportDto> GetJobReportAsync()
        {
            return new JobReportDto
            {
                TotalJobs =
                await _context.Jobs.CountAsync(),

                OpenJobs =
                await _context.Jobs.CountAsync(),

                ClosedJobs = 0
            };
        }





        public async Task<InterviewReportDto> GetInterviewReportAsync()
        {
            return new InterviewReportDto
            {
                TotalInterviews =
                await _context.Interviews.CountAsync(),

                Completed = 0,

                Pending =
                await _context.Interviews.CountAsync()
            };
        }
    }
}