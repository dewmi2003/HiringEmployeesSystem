using Recruitment.Application.DTOs.Reports;

namespace Recruitment.Application.Interfaces.Repositories
{
    public interface IReportRepository
    {
        Task<RecruitmentReportDto> GetRecruitmentReportAsync();

        Task<CandidateReportDto> GetCandidateReportAsync();

        Task<JobReportDto> GetJobReportAsync();

        Task<InterviewReportDto> GetInterviewReportAsync();
    }
}