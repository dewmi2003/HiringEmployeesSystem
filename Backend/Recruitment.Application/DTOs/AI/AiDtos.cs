namespace Recruitment.Application.DTOs.AI
{
    public record ResumeAnalysisRequestDto(
        string ResumeText,
        string? JobDescription,
        string? CandidateName
    );

    public record ResumeAnalysisDto(
        string CandidateName,
        int AtsScore,
        int MatchScore,
        string Summary,
        IReadOnlyList<string> MatchedSkills,
        IReadOnlyList<string> MissingSkills,
        IReadOnlyList<string> Strengths,
        IReadOnlyList<string> ImprovementSuggestions,
        IReadOnlyList<string> AtsIssues,
        IReadOnlyList<string> Keywords,
        bool UsedAiProvider,
        string AiProviderResult,
        DateTime GeneratedAt
    );

    public record JobMatchRequestDto(
        string ResumeText,
        string JobDescription,
        string? JobTitle
    );

    public record JobMatchDto(
        string JobTitle,
        int MatchPercentage,
        IReadOnlyList<string> MatchedSkills,
        IReadOnlyList<string> MissingSkills,
        string ExperienceFit,
        string EducationFit,
        string Recommendation,
        string Rationale,
        IReadOnlyList<string> SuggestedInterviewFocus,
        bool UsedAiProvider,
        string AiProviderResult,
        DateTime GeneratedAt
    );

    public record CandidateSummaryDto(
        Guid? CandidateId,
        string CandidateName,
        string Summary
    );

    public record CandidateRankingRequestDto(
        string JobDescription,
        IReadOnlyList<CandidateSummaryDto> Candidates,
        string? JobTitle
    );

    public record CandidateRankDto(
        int Rank,
        Guid? CandidateId,
        string CandidateName,
        int Score,
        IReadOnlyList<string> MatchedSkills,
        IReadOnlyList<string> MissingSkills,
        string Rationale,
        string Recommendation
    );

    public record CandidateRankingResponseDto(
        Guid? JobId,
        string JobTitle,
        IReadOnlyList<CandidateRankDto> Candidates,
        bool UsedAiProvider,
        string AiProviderResult,
        DateTime GeneratedAt
    );

    public record InterviewQuestionsRequestDto(
        string JobDescription,
        string CandidateSummary,
        int Count
    );

    public record InterviewQuestionsResponseDto(
        IReadOnlyList<string> Questions,
        IReadOnlyList<string> FocusAreas,
        bool UsedAiProvider,
        string AiProviderResult,
        DateTime GeneratedAt
    );
}
