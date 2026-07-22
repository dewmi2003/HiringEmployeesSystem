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

    public record ScheduledInterviewDto(
        Guid InterviewId,
        Guid ApplicationId,
        string CandidateFullName,
        string CandidateEmail,
        string JobTitle,
        DateTime InterviewDate,
        DateTime InterviewEndDate,
        string? Location,
        string InterviewStatus,
        string CalendarResult
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
