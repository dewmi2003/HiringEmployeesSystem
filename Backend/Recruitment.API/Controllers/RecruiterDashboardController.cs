using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.Interfaces.Services;

namespace Recruitment.API.Controllers
{
    /// <summary>Phase 4 – Recruiter Dashboard APIs.</summary>
    [ApiController]
    [Route("api/recruiter-dashboard")]
    [Authorize(Roles = "Recruiter,Admin")]
    public class RecruiterDashboardController : ControllerBase
    {
        private readonly IRecruiterDashboardService _dashboardService;

        public RecruiterDashboardController(IRecruiterDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>Overall recruitment platform statistics.</summary>
        [HttpGet("statistics")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetStatistics()
        {
            var stats = await _dashboardService.GetStatisticsAsync();
            return Ok(stats);
        }

        /// <summary>
        /// Paginated list of all jobs with optional search, filter (status), and sorting.
        /// </summary>
        [HttpGet("jobs")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetJobs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool ascending = false)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var result = await _dashboardService.GetJobsAsync(page, pageSize, search, status, sortBy, ascending);
            return Ok(result);
        }

        /// <summary>Most recently submitted applications.</summary>
        [HttpGet("recent-applications")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetRecentApplications([FromQuery] int count = 10)
        {
            if (count < 1 || count > 100) count = 10;
            var results = await _dashboardService.GetRecentApplicationsAsync(count);
            return Ok(results);
        }

        /// <summary>Top candidates ranked by average evaluation score.</summary>
        [HttpGet("top-candidates")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetTopCandidates([FromQuery] int count = 10)
        {
            if (count < 1 || count > 100) count = 10;
            var results = await _dashboardService.GetTopCandidatesAsync(count);
            return Ok(results);
        }

        /// <summary>Monthly job posting statistics for the past 12 months.</summary>
        [HttpGet("monthly-stats")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetMonthlyStats([FromQuery] string type = "jobs")
        {
            var results = type.ToLower() == "applications"
                ? await _dashboardService.GetMonthlyApplicationStatsAsync()
                : await _dashboardService.GetMonthlyJobStatsAsync();

            return Ok(results);
        }
    }
}
