using Recruitment.Application.DTOs.Candidates;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;

namespace Recruitment.Application.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository _repo;

        public CandidateService(ICandidateRepository repo)
        {
            _repo = repo;
        }


        public async Task<CandidateProfileDto?> GetProfileAsync(Guid candidateId)
        {
            var candidate = await _repo.GetByIdAsync(candidateId);

            if (candidate == null)
                return null;

            return MapToDto(candidate);
        }



        public async Task<CandidateProfileDto?> GetProfileByUserIdAsync(Guid userId)
        {
            var candidate = await _repo.GetByUserIdAsync(userId);

            if (candidate == null)
                return null;

            return MapToDto(candidate);
        }



        public async Task UpdateProfileAsync(
            Guid candidateId,
            UpdateCandidateProfileDto dto)
        {
            var candidate = await _repo.GetByIdAsync(candidateId);

            if (candidate == null)
                throw new KeyNotFoundException(
                    "Candidate not found.");


            if (dto.Phone != null)
                candidate.Phone = dto.Phone;


            if (dto.Address != null)
                candidate.Address = dto.Address;


            if (dto.Bio != null)
                candidate.Bio = dto.Bio;


            if (dto.Experience != null)
                candidate.Experience = dto.Experience;


            if (dto.Education != null)
                candidate.Education = dto.Education;


            await _repo.UpdateAsync(candidate);
        }



        private static CandidateProfileDto MapToDto(
            Domain.Entities.Candidate c)
        {

            var skills = c.CandidateSkills?
                .Select(cs => cs.Skill?.Name ?? "")
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList()
                ?? new List<string>();



            return new CandidateProfileDto(

                c.Id,

                c.UserId,


                c.User != null
                    ? c.User.FullName
                    : $"{c.FirstName} {c.LastName}",


                c.User?.Email ?? "",


                c.Phone,

                c.Address,

                c.Bio,

                c.Experience,

                c.Education,

                skills
            );
        }
    }
}