using System;

namespace Recruitment.Application.DTOs.HiringDecisions
{
    /// <summary>Shared context sent with every Hiring Decision action.</summary>
    public record HiringDecisionActionDto(
        Guid ApplicationId,
        Guid DecidedByUserId,
        string? Comments
    );

    public record CreateHiringDecisionDto(
        Guid ApplicationId,
        Guid DecidedByUserId,
        /// <summary>Shortlisted | Rejected | OfferExtended | Hired</summary>
        string Decision,
        string? Comments
    );

    public record UpdateHiringDecisionDto(
        string Decision,
        string? Comments
    );

    public record HiringDecisionDto(
        Guid Id,
        Guid ApplicationId,
        string CandidateName,
        string JobTitle,
        Guid DecidedByUserId,
        string DecidedByUserName,
        string Decision,
        string? Comments,
        DateTime DecidedAt
    );

    public record ApplicationStatusHistoryDto(
        Guid Id,
        Guid ApplicationId,
        string OldStatus,
        string NewStatus,
        Guid ChangedByUserId,
        string ChangedByUserName,
        DateTime ChangedAt,
        string? Comments
    );
}
