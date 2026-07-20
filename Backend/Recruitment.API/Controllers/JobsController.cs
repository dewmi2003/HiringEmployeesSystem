using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.DTOs.Jobs;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Recruitment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly IRecruiterRepository _recruiterRepository;


        public JobsController(
            IJobService jobService,
            IRecruiterRepository recruiterRepository)
        {
            _jobService = jobService;
            _recruiterRepository = recruiterRepository;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? title,
            [FromQuery] string? location)
        {
            var jobs = await _jobService
                .GetAllActiveJobsAsync(location, title);

            return Ok(jobs);
        }


        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var job = await _jobService
                .GetJobByIdAsync(id);

            if (job == null)
                return NotFound();

            return Ok(job);
        }



        [HttpPost]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> Create(
            [FromBody] CreateJobDto dto)
        {

            var userId = Guid.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!
            );


            var recruiter =
                await _recruiterRepository
                .GetByUserIdAsync(userId);



            if (recruiter == null)
            {
                return BadRequest(
                    "Recruiter profile not found."
                );
            }



            var id =
                await _jobService.CreateJobAsync(
                    recruiter.Id,
                    dto
                );


            return CreatedAtAction(
                nameof(GetById),
                new { id },
                new { id }
            );
        }



        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateJobDto dto)
        {
            try
            {
                await _jobService.UpdateJobAsync(id, dto);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }



        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _jobService.DeleteJobAsync(id);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}