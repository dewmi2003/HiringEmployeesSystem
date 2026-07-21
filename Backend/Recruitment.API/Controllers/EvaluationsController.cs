using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.DTOs.Evaluations;
using Recruitment.Application.Interfaces.Services;

namespace Recruitment.API.Controllers
{
    /// <summary>Phase 2 – Candidate Evaluation endpoints.</summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EvaluationsController : ControllerBase
    {
        private readonly IEvaluationService _evaluationService;

        public EvaluationsController(IEvaluationService evaluationService)
        {
            _evaluationService = evaluationService;
        }

        /// <summary>Create a new evaluation for a candidate/application.</summary>
        [HttpPost]
        [Authorize(Roles = "Recruiter,Admin")]
        [ProducesResponseType(typeof(EvaluationDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CreateEvaluationDto dto)
        {
            try
            {
                var result = await _evaluationService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Update an existing evaluation.</summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Recruiter,Admin")]
        [ProducesResponseType(typeof(EvaluationDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEvaluationDto dto)
        {
            try
            {
                var result = await _evaluationService.UpdateAsync(id, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Delete an evaluation.</summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Recruiter,Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _evaluationService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>Get all evaluations.</summary>
        [HttpGet]
        [Authorize(Roles = "Recruiter,Admin")]
        [ProducesResponseType(typeof(IEnumerable<EvaluationDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var results = await _evaluationService.GetAllAsync();
            return Ok(results);
        }

        /// <summary>Get a single evaluation by ID.</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(EvaluationDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _evaluationService.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>Get all evaluations for a specific application.</summary>
        [HttpGet("application/{applicationId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<EvaluationDto>), 200)]
        public async Task<IActionResult> GetByApplication(Guid applicationId)
        {
            var results = await _evaluationService.GetByApplicationIdAsync(applicationId);
            return Ok(results);
        }

        /// <summary>Get all evaluations for a specific candidate.</summary>
        [HttpGet("candidate/{candidateId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<EvaluationDto>), 200)]
        public async Task<IActionResult> GetByCandidate(Guid candidateId)
        {
            var results = await _evaluationService.GetByCandidateIdAsync(candidateId);
            return Ok(results);
        }
    }
}
