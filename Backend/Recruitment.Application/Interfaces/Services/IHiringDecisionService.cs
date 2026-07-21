using Recruitment.Application.DTOs.HiringDecisions;
using Recruitment.Application.Interfaces.Repositories;
namespace Recruitment.Application.Interfaces.Services
{
    /// <summary>Phase 3 - Hiring Decision Workflow service contract.</summary>
    public interface IHiringDecisionService
    {
        /// <summary>Shortlist a candidate (Recruiter action).</summary>
        Task<HiringDecisionDto> ShortlistAsync(HiringDecisionActionDto dto);

        /// <summary>Reject a candidate (Recruiter action).</summary>
        Task<HiringDecisionDto> RejectAsync(HiringDecisionActionDto dto);

        /// <summary>Extend an offer to a candidate (Recruiter action).</summary>
        Task<HiringDecisionDto> OfferAsync(HiringDecisionActionDto dto);

        /// <summary>Mark candidate as Hired – typically Hiring Manager approval.</summary>
        Task<HiringDecisionDto> HireAsync(HiringDecisionActionDto dto);

        /// <summary>Get all hiring decisions.</summary>
        Task<IEnumerable<HiringDecisionDto>> GetAllAsync();

        /// <summary>Get a single hiring decision by ID.</summary>
        Task<HiringDecisionDto?> GetByIdAsync(Guid id);

        /// <summary>Get all hiring decisions for an application.</summary>
        Task<IEnumerable<HiringDecisionDto>> GetByApplicationIdAsync(Guid applicationId);
    }
}
