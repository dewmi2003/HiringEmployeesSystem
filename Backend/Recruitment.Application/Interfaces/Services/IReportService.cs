using Recruitment.Application.DTOs.Reports;

namespace Recruitment.Application.Interfaces.Services
{
    public interface IReportService
    {
        Task<RecruitmentReportDto> GetRecruitmentReportAsync();

        Task<CandidateReportDto> GetCandidateReportAsync();

        Task<JobReportDto> GetJobReportAsync();

        Task<InterviewReportDto> GetInterviewReportAsync();
    }
}