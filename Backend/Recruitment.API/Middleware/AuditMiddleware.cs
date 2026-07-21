using Recruitment.Application.DTOs.Audit;
using Recruitment.Application.Interfaces.Services;


namespace Recruitment.API.Middleware
{

public class AuditMiddleware
{


private readonly RequestDelegate _next;



public AuditMiddleware(
RequestDelegate next)
{
_next=next;
}




public async Task InvokeAsync(
HttpContext context,
IAuditLogService service)
{


await _next(context);



if(context.User.Identity?.IsAuthenticated == true)
{

await service.CreateAsync(
new AuditLogDto
{

UserId=null,

Action=context.Request.Method,

EntityName=context.Request.Path,

IpAddress=context.Connection.RemoteIpAddress?.ToString()
?? "",

CreatedAt=DateTime.UtcNow

});

}


}


}

}
