using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.Interfaces.Services;


namespace Recruitment.API.Controllers
{

[ApiController]
[Route("api/auditlogs")]
[Authorize(Roles="Admin")]
public class AuditLogsController:ControllerBase
{


private readonly IAuditLogService _service;



public AuditLogsController(
IAuditLogService service)
{
_service=service;
}



[HttpGet]
public async Task<IActionResult> GetAll()
{

return Ok(
await _service.GetAllAsync());

}


}

}