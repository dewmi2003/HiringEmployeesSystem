using Recruitment.Application.DTOs.Evaluations;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;
using Recruitment.Domain.Entities;

namespace Recruitment.Application.Services
{
    public class EvaluationService : IEvaluationService
    {
        private readonly IEvaluationRepository _evalRepo;
        private readonly IApplicationRepository _appRepo;
        private readonly ICandidateRepository _candidateRepo;
        private readonly IRecruiterRepository _recruiterRepo;
        private readonly IUserRepository _userRepo;

        public EvaluationService(
            IEvaluationRepository evalRepo,
            IApplicationRepository appRepo,
            ICandidateRepository candidateRepo,
            IRecruiterRepository recruiterRepo,
            IUserRepository userRepo)
        {
            _evalRepo = evalRepo;
            _appRepo = appRepo;
            _candidateRepo = candidateRepo;
            _recruiterRepo = recruiterRepo;
            _userRepo = userRepo;
        }

        public async Task<EvaluationDto> CreateAsync(CreateEvaluationDto dto)
        {
            ValidateScores(dto.TechnicalScore, dto.CommunicationScore, dto.ExperienceScore, dto.CultureFitScore);

            var application = await _appRepo.GetByIdAsync(dto.ApplicationId)
                ?? throw new KeyNotFoundException($"Application {dto.ApplicationId} not found.");

            var candidate = await _candidateRepo.GetByIdAsync(dto.CandidateId)
                ?? throw new KeyNotFoundException($"Candidate {dto.CandidateId} not found.");

            double overall = CalculateOverallScore(dto.TechnicalScore, dto.CommunicationScore, dto.ExperienceScore, dto.CultureFitScore);
            string recommendation = DetermineRecommendation(overall);

            var evaluation = new Evaluation
            {
                Id = Guid.NewGuid(),
                ApplicationId = dto.ApplicationId,
                CandidateId = dto.CandidateId,
                InterviewId = dto.InterviewId,
                InterviewerId = dto.InterviewerId,
                HiringManagerId = dto.HiringManagerId,
                TechnicalScore = dto.TechnicalScore,
                CommunicationScore = dto.CommunicationScore,
                ExperienceScore = dto.ExperienceScore,
                CultureFitScore = dto.CultureFitScore,
                OverallScore = overall,
                Recommendation = recommendation,
                Notes = dto.Notes ?? string.Empty,
                Score = (int)Math.Round(overall),
                CreatedAt = DateTime.UtcNow
            };

            await _evalRepo.AddAsync(evaluation);

            return await MapToDtoAsync(evaluation);
        }

        public async Task<EvaluationDto> UpdateAsync(Guid id, UpdateEvaluationDto dto)
        {
            ValidateScores(dto.TechnicalScore, dto.CommunicationScore, dto.ExperienceScore, dto.CultureFitScore);

            var evaluation = await _evalRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Evaluation {id} not found.");

            evaluation.TechnicalScore = dto.TechnicalScore;
            evaluation.CommunicationScore = dto.CommunicationScore;
            evaluation.ExperienceScore = dto.ExperienceScore;
            evaluation.CultureFitScore = dto.CultureFitScore;
            evaluation.Notes = dto.Notes ?? string.Empty;
            evaluation.HiringManagerId = dto.HiringManagerId;
            evaluation.OverallScore = CalculateOverallScore(dto.TechnicalScore, dto.CommunicationScore, dto.ExperienceScore, dto.CultureFitScore);
            evaluation.Recommendation = DetermineRecommendation(evaluation.OverallScore);
            evaluation.Score = (int)Math.Round(evaluation.OverallScore);
            evaluation.UpdatedAt = DateTime.UtcNow;

            await _evalRepo.UpdateAsync(evaluation);

            return await MapToDtoAsync(evaluation);
        }

        public async Task DeleteAsync(Guid id)
        {
            var evaluation = await _evalRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Evaluation {id} not found.");
            await _evalRepo.DeleteAsync(evaluation);
        }

        public async Task<IEnumerable<EvaluationDto>> GetAllAsync()
        {
            var all = await _evalRepo.GetAllAsync();
            var dtos = new List<EvaluationDto>();
            foreach (var e in all)
                dtos.Add(await MapToDtoAsync(e));
            return dtos;
        }

        public async Task<EvaluationDto?> GetByIdAsync(Guid id)
        {
            var eval = await _evalRepo.GetByIdAsync(id);
            return eval == null ? null : await MapToDtoAsync(eval);
        }

        public async Task<IEnumerable<EvaluationDto>> GetByApplicationIdAsync(Guid applicationId)
        {
            var all = await _evalRepo.GetAllAsync();
            var filtered = all.Where(e => e.ApplicationId == applicationId);
            var dtos = new List<EvaluationDto>();
            foreach (var e in filtered)
                dtos.Add(await MapToDtoAsync(e));
            return dtos;
        }

        public async Task<IEnumerable<EvaluationDto>> GetByCandidateIdAsync(Guid candidateId)
        {
            var all = await _evalRepo.GetAllAsync();
            var filtered = all.Where(e => e.CandidateId == candidateId);
            var dtos = new List<EvaluationDto>();
            foreach (var e in filtered)
                dtos.Add(await MapToDtoAsync(e));
            return dtos;
        }

        // ─── Helpers ────────────────────────────────────────────────────────

        private static double CalculateOverallScore(int technical, int communication, int experience, int cultural)
            => (technical + communication + experience + cultural) / 4.0;

        private static string DetermineRecommendation(double overall)
        {
            if (overall >= 90) return "Hire";
            if (overall >= 75) return "Strong Candidate";
            if (overall >= 60) return "Consider";
            return "Reject";
        }

        private static void ValidateScores(params int[] scores)
        {
            if (scores.Any(s => s < 0 || s > 100))
                throw new ArgumentException("All scores must be between 0 and 100.");
        }

        private async Task<EvaluationDto> MapToDtoAsync(Evaluation e)
        {
            string interviewerName = string.Empty;
            if (e.Interviewer != null)
            {
                interviewerName = $"{e.Interviewer.FirstName} {e.Interviewer.LastName}".Trim();
            }
            else
            {
                var recruiter = await _recruiterRepo.GetByIdAsync(e.InterviewerId);
                if (recruiter != null)
                    interviewerName = $"{recruiter.FirstName} {recruiter.LastName}".Trim();
            }

            string? hmName = null;
            if (e.HiringManagerId.HasValue)
            {
                if (e.HiringManager != null)
                    hmName = e.HiringManager.FullName;
                else
                {
                    var hm = await _userRepo.GetByIdAsync(e.HiringManagerId.Value);
                    hmName = hm?.FullName;
                }
            }

            return new EvaluationDto(
                e.Id,
                e.ApplicationId,
                e.CandidateId,
                e.InterviewId,
                e.InterviewerId,
                interviewerName,
                e.HiringManagerId,
                hmName,
                e.TechnicalScore,
                e.CommunicationScore,
                e.ExperienceScore,
                e.CultureFitScore,
                e.OverallScore,
                e.Recommendation,
                e.Notes,
                e.CreatedAt,
                e.UpdatedAt
            );
        }
    }
}
