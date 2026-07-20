using Recruitment.Application.DTOs.Candidates;

namespace Recruitment.Application.Interfaces.Services
{
    public interface ICandidateService
    {
        Task<CandidateProfileDto?> GetProfileAsync(Guid candidateId);
        Task<CandidateProfileDto?> GetProfileByUserIdAsync(Guid userId);
        Task UpdateProfileAsync(Guid candidateId, UpdateCandidateProfileDto dto);
    }
}
