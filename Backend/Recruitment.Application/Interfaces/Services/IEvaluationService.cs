using Recruitment.Application.DTOs.Evaluations;

namespace Recruitment.Application.Interfaces.Services
{
    /// <summary>
    /// Phase 2 - Candidate Evaluation service contract.
    /// </summary>
    public interface IEvaluationService
    {
        /// <summary>Create a new evaluation for a candidate application.</summary>
        Task<EvaluationDto> CreateAsync(CreateEvaluationDto dto);

        /// <summary>Update an existing evaluation.</summary>
        Task<EvaluationDto> UpdateAsync(Guid id, UpdateEvaluationDto dto);

        /// <summary>Delete an evaluation by ID.</summary>
        Task DeleteAsync(Guid id);

        /// <summary>Get all evaluations (recruiter / admin view).</summary>
        Task<IEnumerable<EvaluationDto>> GetAllAsync();

        /// <summary>Get a single evaluation by ID.</summary>
        Task<EvaluationDto?> GetByIdAsync(Guid id);

        /// <summary>Get all evaluations for a specific application.</summary>
        Task<IEnumerable<EvaluationDto>> GetByApplicationIdAsync(Guid applicationId);

        /// <summary>Get all evaluations for a specific candidate.</summary>
        Task<IEnumerable<EvaluationDto>> GetByCandidateIdAsync(Guid candidateId);
    }
}
