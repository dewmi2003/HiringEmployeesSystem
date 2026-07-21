using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.DTOs.HiringDecisions;
using Recruitment.Application.Interfaces.Services;

namespace Recruitment.API.Controllers
{
    /// <summary>Phase 3 – Hiring Decision Workflow endpoints.</summary>
    [ApiController]
    [Route("api/hiring-decisions")]
    [Authorize]
    public class HiringDecisionController : ControllerBase
    {
        private readonly IHiringDecisionService _hiringDecisionService;

        public HiringDecisionController(IHiringDecisionService hiringDecisionService)
        {
            _hiringDecisionService = hiringDecisionService;
        }

        /// <summary>Shortlist a candidate for an application.</summary>
        [HttpPost("shortlist")]
        [Authorize(Roles = "Recruiter,Admin")]
        [ProducesResponseType(typeof(HiringDecisionDto), 200)]
        public async Task<IActionResult> Shortlist([FromBody] HiringDecisionActionDto dto)
        {
            try
            {
                var result = await _hiringDecisionService.ShortlistAsync(dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }

        /// <summary>Reject a candidate for an application.</summary>
        [HttpPost("reject")]
        [Authorize(Roles = "Recruiter,Admin")]
        [ProducesResponseType(typeof(HiringDecisionDto), 200)]
        public async Task<IActionResult> Reject([FromBody] HiringDecisionActionDto dto)
        {
            try
            {
                var result = await _hiringDecisionService.RejectAsync(dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }

        /// <summary>Extend an offer to a candidate.</summary>
        [HttpPost("offer")]
        [Authorize(Roles = "Recruiter,Admin")]
        [ProducesResponseType(typeof(HiringDecisionDto), 200)]
        public async Task<IActionResult> Offer([FromBody] HiringDecisionActionDto dto)
        {
            try
            {
                var result = await _hiringDecisionService.OfferAsync(dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }

        /// <summary>Mark a candidate as Hired (Hiring Manager approval).</summary>
        [HttpPost("hire")]
        [Authorize(Roles = "Admin,Recruiter")]
        [ProducesResponseType(typeof(HiringDecisionDto), 200)]
        public async Task<IActionResult> Hire([FromBody] HiringDecisionActionDto dto)
        {
            try
            {
                var result = await _hiringDecisionService.HireAsync(dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }

        /// <summary>Get all hiring decisions.</summary>
        [HttpGet]
        [Authorize(Roles = "Recruiter,Admin")]
        [ProducesResponseType(typeof(IEnumerable<HiringDecisionDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var results = await _hiringDecisionService.GetAllAsync();
            return Ok(results);
        }

        /// <summary>Get a single hiring decision by ID.</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(HiringDecisionDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _hiringDecisionService.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>Get all hiring decisions for an application.</summary>
        [HttpGet("application/{applicationId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<HiringDecisionDto>), 200)]
        public async Task<IActionResult> GetByApplication(Guid applicationId)
        {
            var results = await _hiringDecisionService.GetByApplicationIdAsync(applicationId);
            return Ok(results);
        }
    }
}
