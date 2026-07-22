using Recruitment.Application.DTOs.AI;

namespace Recruitment.Application.Interfaces.Services
{
    public interface IAiFeatureService
    {
        Task<ResumeAnalysisDto> AnalyzeResumeAsync(ResumeAnalysisRequestDto request);

        Task<ResumeAnalysisDto> AnalyzeStoredResumeAsync(Guid resumeId, Guid? jobId);

        Task<JobMatchDto> MatchJobAsync(JobMatchRequestDto request);

        Task<CandidateRankingResponseDto> RankCandidatesAsync(CandidateRankingRequestDto request);

        Task<CandidateRankingResponseDto> RankJobApplicantsAsync(Guid jobId);

        Task<InterviewQuestionsResponseDto> GenerateInterviewQuestionsAsync(InterviewQuestionsRequestDto request);
    }
}
