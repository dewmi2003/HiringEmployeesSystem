using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Recruitment.Infrastructure.AI;

namespace Recruitment.Infrastructure.AI
{
    public interface IResumeAiService
    {
        Task<string> ParseResumeAsync(string resumeText);
        Task<string> GenerateInterviewQuestionsAsync(string jobDescription, string candidateSummary);
        Task<string> RankCandidatesAsync(string jobDescription, string[] candidateSummaries);
    }

    public class ResumeAiService : IResumeAiService
    {
        private readonly IAiAdapter _adapter;
        private readonly ILogger<ResumeAiService> _logger;

        public ResumeAiService(IAiAdapter adapter, ILogger<ResumeAiService> logger)
        {
            _adapter = adapter;
            _logger = logger;
        }

        public async Task<string> ParseResumeAsync(string resumeText)
        {
            var prompt = $"Extract structured candidate data (name, email, summary, skills, experience) from the following resume:\n{resumeText}";
            return await _adapter.GetCompletionAsync(prompt);
        }

        public async Task<string> GenerateInterviewQuestionsAsync(string jobDescription, string candidateSummary)
        {
            var prompt = $"Given the job description:\n{jobDescription}\nand the candidate summary:\n{candidateSummary}\nGenerate 6 focused interview questions.";
            return await _adapter.GetCompletionAsync(prompt);
        }

        public async Task<string> RankCandidatesAsync(string jobDescription, string[] candidateSummaries)
        {
            var joined = string.Join("\n---\n", candidateSummaries);
            var prompt = $"Rank the following candidates for this job description:\n{jobDescription}\nCandidates:\n{joined}\nProvide a score and short rationale for each.";
            return await _adapter.GetCompletionAsync(prompt);
        }
    }
}
