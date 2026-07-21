using Recruitment.Application.DTOs.Dashboard;

namespace Recruitment.Application.Interfaces.Services
{
    /// <summary>Phase 4 – Recruiter Dashboard API service contract.</summary>
    public interface IRecruiterDashboardService
    {
        /// <summary>High-level platform statistics.</summary>
        Task<DashboardStatisticsDto> GetStatisticsAsync();

        /// <summary>Paginated list of jobs with optional search/filter/sort.</summary>
        Task<PaginatedResult<JobDashboardDto>> GetJobsAsync(
            int page, int pageSize, string? search, string? status, string? sortBy, bool ascending);

        /// <summary>Most recent applications.</summary>
        Task<IEnumerable<RecentApplicationDto>> GetRecentApplicationsAsync(int count);

        /// <summary>Top candidates ranked by average evaluation score.</summary>
        Task<IEnumerable<TopCandidateDto>> GetTopCandidatesAsync(int count);

        /// <summary>Monthly job posting statistics for the past 12 months.</summary>
        Task<IEnumerable<MonthlyStatDto>> GetMonthlyJobStatsAsync();

        /// <summary>Monthly application statistics for the past 12 months.</summary>
        Task<IEnumerable<MonthlyStatDto>> GetMonthlyApplicationStatsAsync();
    }

    /// <summary>Job summary for the dashboard jobs list.</summary>
    public record JobDashboardDto(
        Guid Id,
        string Title,
        string Status,
        string Department,
        string Location,
        int ApplicationCount,
        DateTime PostedDate
    );
}
