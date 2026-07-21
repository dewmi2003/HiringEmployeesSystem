using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.Interfaces.Services;


namespace Recruitment.API.Controllers
{

[ApiController]
[Route("api/analytics")]
[Authorize(Roles="Admin,Recruiter")]
public class AnalyticsController : ControllerBase
{

private readonly IAnalyticsService _service;


public AnalyticsController(
IAnalyticsService service)
{
_service = service;
}



[HttpGet("hiring-trends")]
public async Task<IActionResult> HiringTrends()
{
return Ok(
await _service.GetHiringTrendsAsync());
}



[HttpGet("top-skills")]
public async Task<IActionResult> TopSkills()
{
return Ok(
await _service.GetTopSkillsAsync());
}



[HttpGet("performance")]
public async Task<IActionResult> Performance()
{
return Ok(
await _service.GetPerformanceAsync());
}


}

}