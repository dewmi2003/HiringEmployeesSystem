using Recruitment.Application.DTOs.Audit;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;
using Recruitment.Domain.Entities;


namespace Recruitment.Application.Services
{

public class AuditLogService : IAuditLogService
{

private readonly IAuditLogRepository _repository;



public AuditLogService(
IAuditLogRepository repository)
{
_repository = repository;
}



public async Task CreateAsync(
AuditLogDto dto)
{

var log = new AuditLog
{

Id = Guid.NewGuid(),

UserId = dto.UserId,

Action = dto.Action,

EntityName = dto.EntityName,

IpAddress = dto.IpAddress,

CreatedAt = DateTime.UtcNow

};


await _repository.AddAsync(log);

}





public async Task<IEnumerable<AuditLogDto>> GetAllAsync()
{

var logs =
await _repository.GetAllAsync();


return logs.Select(x=>new AuditLogDto
{

Id=x.Id,

UserId=x.UserId,

Action=x.Action,

EntityName=x.EntityName,

IpAddress=x.IpAddress,

CreatedAt=x.CreatedAt


});

}

}

}
