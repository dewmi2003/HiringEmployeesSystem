using System;
using System.Collections.Generic;

namespace Recruitment.Application.DTOs.Dashboard
{
    public record DashboardStatisticsDto(
        int TotalJobs,
        int OpenJobs,
        int ClosedJobs,
        int DraftJobs,
        int TotalApplications,
        int ShortlistedCandidates,
        int RejectedCandidates,
        int HiredCandidates,
        int PendingInterviews
    );

    public record RecentApplicationDto(
        Guid ApplicationId,
        Guid CandidateId,
        string CandidateName,
        string CandidateEmail,
        Guid JobId,
        string JobTitle,
        string Status,
        DateTime AppliedDate
    );

    public record TopCandidateDto(
        Guid CandidateId,
        string FullName,
        string Email,
        double AverageEvaluationScore,
        string Recommendation,
        int ApplicationCount
    );

    public record MonthlyStatDto(
        int Year,
        int Month,
        string MonthName,
        int Count
    );

    public record RecruiterDashboardDto(
        DashboardStatisticsDto Statistics,
        IEnumerable<RecentApplicationDto> RecentApplications,
        IEnumerable<TopCandidateDto> TopCandidates
    );

    public record PaginatedResult<T>(
        IEnumerable<T> Items,
        int TotalCount,
        int Page,
        int PageSize,
        int TotalPages
    );
}
