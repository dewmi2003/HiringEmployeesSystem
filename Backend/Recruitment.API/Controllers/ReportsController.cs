using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.Interfaces.Services;

namespace Recruitment.API.Controllers
{

[ApiController]
[Route("api/reports")]
[Authorize(Roles="Admin,Recruiter")]
public class ReportsController : ControllerBase
{

private readonly IReportService _service;


public ReportsController(
IReportService service)
{
_service=service;
}



[HttpGet("recruitment")]
public async Task<IActionResult> Recruitment()
{
return Ok(await _service.GetRecruitmentReportAsync());
}



[HttpGet("candidates")]
public async Task<IActionResult> Candidates()
{
return Ok(await _service.GetCandidateReportAsync());
}



[HttpGet("jobs")]
public async Task<IActionResult> Jobs()
{
return Ok(await _service.GetJobReportAsync());
}



[HttpGet("interviews")]
public async Task<IActionResult> Interviews()
{
return Ok(await _service.GetInterviewReportAsync());
}


}

}