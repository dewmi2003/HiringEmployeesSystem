using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.DTOs;
using Recruitment.Application.Interfaces.Services;


namespace Recruitment.API.Controllers
{

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{


private readonly INotificationService _service;



public NotificationsController(
INotificationService service)
{
_service = service;
}





[HttpGet("{userId}")]
public async Task<IActionResult> GetUser(
Guid userId)
{

return Ok(
await _service.GetUserNotificationsAsync(userId));

}





[HttpGet]
[Authorize(Roles="Admin")]
public async Task<IActionResult> GetAll()
{

return Ok(
await _service.GetAllAsync());

}





[HttpPost]
public async Task<IActionResult> Create(
NotificationDto dto)
{

await _service.CreateAsync(dto);


return Ok();

}





[HttpPut("{id}/read")]
public async Task<IActionResult> MarkRead(
Guid id)
{

await _service.MarkAsReadAsync(id);


return Ok();

}


}

}