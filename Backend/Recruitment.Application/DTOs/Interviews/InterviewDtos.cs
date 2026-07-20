namespace Recruitment.Application.DTOs.Interviews
{
    public record CreateInterviewDto(
        Guid ApplicationId,
        DateTime InterviewDate,
        string? Location
    );

    public record InterviewDto(
        Guid InterviewId,
        Guid ApplicationId,
        string CandidateFullName,
        string JobTitle,
        DateTime InterviewDate,
        string? Location,
        string InterviewStatus
    );

    public record CreateEvaluationDto(
        Guid InterviewId,
        Guid RecruiterId,
        int Rating,
        string? Comments
    );

    public record EvaluationDto(
        Guid EvaluationId,
        Guid InterviewId,
        string RecruiterName,
        int Rating,
        string? Comments,
        DateTime CreatedAt
    );
}
