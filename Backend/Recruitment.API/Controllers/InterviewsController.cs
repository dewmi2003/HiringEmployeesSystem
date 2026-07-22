using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.DTOs.Interviews;
using Recruitment.Application.Interfaces.Services;

namespace Recruitment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InterviewsController : ControllerBase
    {
        private readonly IInterviewService _interviewService;

        public InterviewsController(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        [HttpGet]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _interviewService.GetAllAsync());
        }

        [HttpPost("schedule")]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> Schedule(CreateInterviewDto dto)
        {
            try
            {
                var result = await _interviewService.ScheduleInterviewAsync(dto);
                return StatusCode(201, result);
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
    }
}
