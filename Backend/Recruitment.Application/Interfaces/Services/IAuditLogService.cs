using Recruitment.Application.DTOs.Audit;

namespace Recruitment.Application.Interfaces.Services
{
    public interface IAuditLogService
    {

        Task CreateAsync(
            AuditLogDto dto);


        Task<IEnumerable<AuditLogDto>>
            GetAllAsync();

    }
}