using System;

namespace Recruitment.Application.DTOs.Evaluations
{
    /// <summary>Create a new candidate evaluation.</summary>
    public record CreateEvaluationDto(
        Guid ApplicationId,
        Guid CandidateId,
        Guid InterviewId,
        Guid InterviewerId,
        Guid? HiringManagerId,
        /// <summary>0–100</summary>
        int TechnicalScore,
        /// <summary>0–100</summary>
        int CommunicationScore,
        /// <summary>0–100</summary>
        int ExperienceScore,
        /// <summary>0–100</summary>
        int CultureFitScore,
        string Notes
    );

    /// <summary>Update an existing evaluation's scores / notes.</summary>
    public record UpdateEvaluationDto(
        int TechnicalScore,
        int CommunicationScore,
        int ExperienceScore,
        int CultureFitScore,
        string Notes,
        Guid? HiringManagerId
    );

    /// <summary>Full evaluation response.</summary>
    public record EvaluationDto(
        Guid Id,
        Guid ApplicationId,
        Guid CandidateId,
        Guid InterviewId,
        Guid InterviewerId,
        string InterviewerName,
        Guid? HiringManagerId,
        string? HiringManagerName,
        int TechnicalScore,
        int CommunicationScore,
        int ExperienceScore,
        int CultureFitScore,
        double OverallScore,
        string Recommendation,
        string Notes,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
}
