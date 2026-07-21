using Recruitment.Application.DTOs.HiringDecisions;

namespace Recruitment.Application.Interfaces.Services
{
    /// <summary>Phase 3 - Application Status History service contract.</summary>
    public interface IApplicationStatusHistoryService
    {
        /// <summary>Get audit trail for an application.</summary>
        Task<IEnumerable<ApplicationStatusHistoryDto>> GetByApplicationIdAsync(Guid applicationId);
    }
}
