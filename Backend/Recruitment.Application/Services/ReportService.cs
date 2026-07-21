using Recruitment.Application.DTOs.Reports;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;

namespace Recruitment.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repository;


        public ReportService(
            IReportRepository repository)
        {
            _repository = repository;
        }



        public async Task<RecruitmentReportDto> GetRecruitmentReportAsync()
        {
            return await _repository.GetRecruitmentReportAsync();
        }



        public async Task<CandidateReportDto> GetCandidateReportAsync()
        {
            return await _repository.GetCandidateReportAsync();
        }



        public async Task<JobReportDto> GetJobReportAsync()
        {
            return await _repository.GetJobReportAsync();
        }



        public async Task<InterviewReportDto> GetInterviewReportAsync()
        {
            return await _repository.GetInterviewReportAsync();
        }
    }
}