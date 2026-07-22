using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.DTOs.AI;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;

namespace Recruitment.API.Controllers
{
    [ApiController]
    [Route("api/ai")]
    [Authorize]
    public class AiController : ControllerBase
    {
        private readonly IAiFeatureService _aiFeatureService;
        private readonly IResumeRepository _resumeRepository;
        private readonly ICandidateRepository _candidateRepository;

        public AiController(
            IAiFeatureService aiFeatureService,
            IResumeRepository resumeRepository,
            ICandidateRepository candidateRepository)
        {
            _aiFeatureService = aiFeatureService;
            _resumeRepository = resumeRepository;
            _candidateRepository = candidateRepository;
        }

        [HttpPost("resume-analysis")]
        [Authorize(Roles = "Admin,Recruiter,Candidate")]
        public async Task<IActionResult> AnalyzeResume([FromBody] ResumeAnalysisRequestDto request)
        {
            try
            {
                return Ok(await _aiFeatureService.AnalyzeResumeAsync(request));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("resumes/{resumeId:guid}/analysis")]
        [Authorize(Roles = "Admin,Recruiter,Candidate")]
        public async Task<IActionResult> AnalyzeStoredResume(Guid resumeId, [FromQuery] Guid? jobId)
        {
            var resume = await _resumeRepository.GetByIdAsync(resumeId);
            if (resume == null)
            {
                return NotFound(new { message = "Resume not found." });
            }

            if (!await CanAccessCandidateAsync(resume.CandidateId))
            {
                return Forbid();
            }

            try
            {
                return Ok(await _aiFeatureService.AnalyzeStoredResumeAsync(resumeId, jobId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("job-match")]
        [Authorize(Roles = "Admin,Recruiter,Candidate")]
        public async Task<IActionResult> MatchJob([FromBody] JobMatchRequestDto request)
        {
            try
            {
                return Ok(await _aiFeatureService.MatchJobAsync(request));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("candidate-ranking")]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> RankCandidates([FromBody] CandidateRankingRequestDto request)
        {
            try
            {
                return Ok(await _aiFeatureService.RankCandidatesAsync(request));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("jobs/{jobId:guid}/candidate-ranking")]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> RankJobApplicants(Guid jobId)
        {
            try
            {
                return Ok(await _aiFeatureService.RankJobApplicantsAsync(jobId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("interview-questions")]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> GenerateInterviewQuestions(
            [FromBody] InterviewQuestionsRequestDto request)
        {
            try
            {
                return Ok(await _aiFeatureService.GenerateInterviewQuestionsAsync(request));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private async Task<bool> CanAccessCandidateAsync(Guid candidateId)
        {
            if (User.IsInRole("Admin") || User.IsInRole("Recruiter"))
            {
                return true;
            }

            if (!User.IsInRole("Candidate"))
            {
                return false;
            }

            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return false;
            }

            var candidate = await _candidateRepository.GetByUserIdAsync(userId);
            return candidate?.Id == candidateId;
        }
    }
}
