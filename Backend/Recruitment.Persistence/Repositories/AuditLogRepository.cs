using Microsoft.EntityFrameworkCore;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Domain.Entities;
using Recruitment.Persistence.Context;


namespace Recruitment.Persistence.Repositories
{

public class AuditLogRepository 
: IAuditLogRepository
{

private readonly ApplicationDbContext _context;



public AuditLogRepository(
ApplicationDbContext context)
{
_context=context;
}




public async Task AddAsync(
AuditLog log)
{

await _context.AuditLogs.AddAsync(log);

await _context.SaveChangesAsync();

}




public async Task<IEnumerable<AuditLog>> GetAllAsync()
{

return await _context.AuditLogs
.Include(x=>x.User)
.OrderByDescending(x=>x.CreatedAt)
.ToListAsync();

}


}

}