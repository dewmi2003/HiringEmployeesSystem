using Recruitment.Application.DTOs.Dashboard;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;

namespace Recruitment.Application.Services
{
    public class RecruiterDashboardService : IRecruiterDashboardService
    {
        private readonly IJobRepository _jobRepo;
        private readonly IApplicationRepository _appRepo;
        private readonly IEvaluationRepository _evalRepo;
        private readonly ICandidateRepository _candidateRepo;
        private readonly IInterviewRepository _interviewRepo;

        public RecruiterDashboardService(
            IJobRepository jobRepo,
            IApplicationRepository appRepo,
            IEvaluationRepository evalRepo,
            ICandidateRepository candidateRepo,
            IInterviewRepository interviewRepo)
        {
            _jobRepo = jobRepo;
            _appRepo = appRepo;
            _evalRepo = evalRepo;
            _candidateRepo = candidateRepo;
            _interviewRepo = interviewRepo;
        }

        public async Task<DashboardStatisticsDto> GetStatisticsAsync()
        {
            var jobs = await _jobRepo.GetAllAsync();
            var apps = await _appRepo.GetAllAsync();
            var interviews = await _interviewRepo.GetAllAsync();

            var jobList = jobs.ToList();
            var appList = apps.ToList();
            var interviewList = interviews.ToList();

            return new DashboardStatisticsDto(
                TotalJobs:           jobList.Count,
                OpenJobs:            jobList.Count(j => j.Status == "Open"),
                ClosedJobs:          jobList.Count(j => j.Status == "Closed"),
                DraftJobs:           jobList.Count(j => j.Status == "Draft"),
                TotalApplications:   appList.Count,
                ShortlistedCandidates: appList.Count(a => a.Status == "Shortlisted"),
                RejectedCandidates:  appList.Count(a => a.Status == "Rejected"),
                HiredCandidates:     appList.Count(a => a.Status == "Hired"),
                PendingInterviews:   interviewList.Count
            );
        }

        public async Task<PaginatedResult<JobDashboardDto>> GetJobsAsync(
            int page, int pageSize, string? search, string? status, string? sortBy, bool ascending)
        {
            var jobs = (await _jobRepo.GetAllAsync()).AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search))
                jobs = jobs.Where(j =>
                    j.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (j.Department != null && j.Department.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (j.Location != null && j.Location.Contains(search, StringComparison.OrdinalIgnoreCase)));

            if (!string.IsNullOrWhiteSpace(status))
                jobs = jobs.Where(j => j.Status.Equals(status, StringComparison.OrdinalIgnoreCase));

            jobs = sortBy?.ToLower() switch
            {
                "title"      => ascending ? jobs.OrderBy(j => j.Title)    : jobs.OrderByDescending(j => j.Title),
                "status"     => ascending ? jobs.OrderBy(j => j.Status)   : jobs.OrderByDescending(j => j.Status),
                "posted"     => ascending ? jobs.OrderBy(j => j.PostedDate): jobs.OrderByDescending(j => j.PostedDate),
                _            => jobs.OrderByDescending(j => j.PostedDate)
            };

            var jobList = jobs.ToList();
            int totalCount = jobList.Count;
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var allApps = (await _appRepo.GetAllAsync()).ToList();
            var paged = jobList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(j => new JobDashboardDto(
                    j.Id,
                    j.Title,
                    j.Status,
                    j.Department ?? string.Empty,
                    j.Location ?? string.Empty,
                    allApps.Count(a => a.JobId == j.Id),
                    j.PostedDate));

            return new PaginatedResult<JobDashboardDto>(paged, totalCount, page, pageSize, totalPages);
        }

        public async Task<IEnumerable<RecentApplicationDto>> GetRecentApplicationsAsync(int count)
        {
            var apps = (await _appRepo.GetAllAsync())
                .OrderByDescending(a => a.AppliedDate)
                .Take(count);

            return apps.Select(a => new RecentApplicationDto(
                a.Id,
                a.CandidateId,
                a.Candidate != null ? $"{a.Candidate.FirstName} {a.Candidate.LastName}" : string.Empty,
                a.Candidate?.User?.Email ?? string.Empty,
                a.JobId,
                a.Job?.Title ?? string.Empty,
                a.Status,
                a.AppliedDate));
        }

        public async Task<IEnumerable<TopCandidateDto>> GetTopCandidatesAsync(int count)
        {
            var evals = (await _evalRepo.GetAllAsync())
                .Where(e => e.OverallScore > 0)
                .GroupBy(e => e.CandidateId)
                .Select(g => new
                {
                    CandidateId = g.Key,
                    AvgScore    = g.Average(e => e.OverallScore),
                    TopRec      = g.OrderByDescending(e => e.OverallScore).First().Recommendation
                })
                .OrderByDescending(x => x.AvgScore)
                .Take(count)
                .ToList();

            var apps = (await _appRepo.GetAllAsync()).ToList();
            var result = new List<TopCandidateDto>();

            foreach (var e in evals)
            {
                var candidate = await _candidateRepo.GetByIdAsync(e.CandidateId);
                if (candidate == null) continue;

                result.Add(new TopCandidateDto(
                    e.CandidateId,
                    $"{candidate.FirstName} {candidate.LastName}".Trim(),
                    candidate.User?.Email ?? string.Empty,
                    Math.Round(e.AvgScore, 2),
                    e.TopRec,
                    apps.Count(a => a.CandidateId == e.CandidateId)));
            }

            return result;
        }

        public async Task<IEnumerable<MonthlyStatDto>> GetMonthlyJobStatsAsync()
        {
            var jobs = await _jobRepo.GetAllAsync();
            return BuildMonthlyStats(jobs.Select(j => j.PostedDate));
        }

        public async Task<IEnumerable<MonthlyStatDto>> GetMonthlyApplicationStatsAsync()
        {
            var apps = await _appRepo.GetAllAsync();
            return BuildMonthlyStats(apps.Select(a => a.AppliedDate));
        }

        // ─── Helpers ────────────────────────────────────────────────────────

        private static IEnumerable<MonthlyStatDto> BuildMonthlyStats(IEnumerable<DateTime> dates)
        {
            var cutoff = DateTime.UtcNow.AddMonths(-11);
            return dates
                .Where(d => d >= cutoff)
                .GroupBy(d => new { d.Year, d.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new MonthlyStatDto(
                    g.Key.Year,
                    g.Key.Month,
                    new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                    g.Count()));
        }
    }
}
