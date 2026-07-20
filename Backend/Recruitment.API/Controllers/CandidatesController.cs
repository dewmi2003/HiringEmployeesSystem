using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.DTOs.Candidates;
using Recruitment.Application.Interfaces.Services;
using System.Security.Claims;

namespace Recruitment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CandidatesController : ControllerBase
    {
        private readonly ICandidateService _candidateService;

        public CandidatesController(ICandidateService candidateService)
        {
            _candidateService = candidateService;
        }

        /// <summary>Get the authenticated candidate's own profile.</summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var profile = await _candidateService.GetProfileByUserIdAsync(userId);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        /// <summary>Get a candidate profile by ID (Recruiter or Admin).</summary>
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var profile = await _candidateService.GetProfileAsync(id);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        /// <summary>Update the authenticated candidate's profile.</summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCandidateProfileDto dto)
        {
            try
            {
                await _candidateService.UpdateProfileAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
