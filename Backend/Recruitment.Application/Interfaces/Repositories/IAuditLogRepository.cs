using Recruitment.Domain.Entities;


namespace Recruitment.Application.Interfaces.Repositories
{
    public interface IAuditLogRepository
    {

        Task AddAsync(AuditLog log);


        Task<IEnumerable<AuditLog>> GetAllAsync();


    }
}