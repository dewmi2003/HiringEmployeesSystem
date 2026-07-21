using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.DTOs.Applications;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;
using System.Security.Claims;


namespace Recruitment.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ApplicationsController : ControllerBase
    {


        private readonly IApplicationService _service;

        private readonly ICandidateRepository _candidateRepository;



        public ApplicationsController(
            IApplicationService service,
            ICandidateRepository candidateRepository)
        {

            _service = service;

            _candidateRepository =
                candidateRepository;

        }






        [HttpPost]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> Apply(
            CreateApplicationDto dto)
        {

            var userId =
                Guid.Parse(
                User.FindFirstValue(
                ClaimTypes.NameIdentifier)!
                );



            var candidate =
                await _candidateRepository
                .GetByUserIdAsync(userId);



            if (candidate == null)
            {
                return BadRequest(
                    "Candidate profile not found."
                );
            }

            try
            {
                var id =
                    await _service.ApplyToJobAsync(
                        candidate.Id,
                        dto
                    );



                return StatusCode(
                    201,
                    new { id }
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }








        [HttpGet("my")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> GetMyApplications()
        {

            var userId =
                Guid.Parse(
                User.FindFirstValue(
                ClaimTypes.NameIdentifier)!
                );



            var candidate =
                await _candidateRepository
                .GetByUserIdAsync(userId);



            if (candidate == null)
                return BadRequest(
                    "Candidate profile not found."
                );



            var result =
                await _service
                .GetApplicationsByCandidateAsync(
                    candidate.Id);



            return Ok(result);

        }








        [HttpGet("job/{jobId}")]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> GetByJob(
            Guid jobId)
        {

            var result =
                await _service
                .GetApplicationsByJobAsync(jobId);


            return Ok(result);

        }







        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> UpdateStatus(
            Guid id,
            UpdateApplicationStatusDto dto)
        {

            try
            {
                await _service
                    .UpdateStatusAsync(id, dto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }


            return NoContent();

        }

    }

}
