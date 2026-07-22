using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Recruitment.Application.DTOs.AI;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;

namespace Recruitment.Infrastructure.AI
{
    public class AiFeatureService : IAiFeatureService
    {
        private static readonly string[] SkillKeywords =
        {
            "c#", ".net", "asp.net", "entity framework", "sql", "azure", "aws", "docker",
            "kubernetes", "react", "typescript", "javascript", "html", "css", "node",
            "python", "java", "spring", "api", "rest", "graphql", "microservices",
            "git", "github", "devops", "ci/cd", "testing", "unit testing", "agile",
            "scrum", "machine learning", "ai", "openai", "data analysis", "power bi",
            "communication", "leadership", "problem solving", "cloud", "security",
            "linux", "figma", "ui", "ux", "mobile", "flutter", "android", "ios"
        };

        private static readonly HashSet<string> StopWords =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "the", "and", "for", "with", "that", "this", "from", "you", "your",
                "are", "will", "have", "has", "was", "were", "been", "our", "their",
                "they", "job", "role", "candidate", "resume", "cv", "work", "team",
                "using", "used", "into", "about", "over", "under", "within", "plus",
                "such", "must", "should", "can", "all", "any", "per", "not", "but"
            };

        private readonly IAiAdapter _adapter;
        private readonly IResumeRepository _resumeRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly ILogger<AiFeatureService> _logger;

        public AiFeatureService(
            IAiAdapter adapter,
            IResumeRepository resumeRepository,
            IJobRepository jobRepository,
            IApplicationRepository applicationRepository,
            ILogger<AiFeatureService> logger)
        {
            _adapter = adapter;
            _resumeRepository = resumeRepository;
            _jobRepository = jobRepository;
            _applicationRepository = applicationRepository;
            _logger = logger;
        }

        public async Task<ResumeAnalysisDto> AnalyzeResumeAsync(ResumeAnalysisRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.ResumeText))
            {
                throw new ArgumentException("Resume text is required.");
            }

            var candidateName = string.IsNullOrWhiteSpace(request.CandidateName)
                ? "Candidate"
                : request.CandidateName.Trim();

            var metrics = BuildMetrics(request.ResumeText, request.JobDescription);
            var providerResult = await TryGetProviderResultAsync(BuildResumeAnalysisPrompt(request, metrics));
            var usedProvider = IsRealProviderResult(providerResult);

            return new ResumeAnalysisDto(
                candidateName,
                metrics.AtsScore,
                metrics.MatchScore,
                BuildSummary(candidateName, metrics),
                metrics.MatchedSkills,
                metrics.MissingSkills,
                metrics.Strengths,
                metrics.Suggestions,
                metrics.AtsIssues,
                metrics.Keywords,
                usedProvider,
                providerResult,
                DateTime.UtcNow);
        }

        public async Task<ResumeAnalysisDto> AnalyzeStoredResumeAsync(Guid resumeId, Guid? jobId)
        {
            var resume = await _resumeRepository.GetByIdAsync(resumeId)
                ?? throw new KeyNotFoundException("Resume not found.");

            var job = jobId.HasValue
                ? await _jobRepository.GetByIdAsync(jobId.Value)
                : null;

            var resumeText = string.IsNullOrWhiteSpace(resume.ParsedText)
                ? $"Resume file: {resume.FileName}"
                : resume.ParsedText;

            var jobDescription = job == null
                ? null
                : BuildJobDescription(job.Title, job.Description, job.Requirements);

            return await AnalyzeResumeAsync(new ResumeAnalysisRequestDto(
                resumeText,
                jobDescription,
                resume.FileName));
        }

        public async Task<JobMatchDto> MatchJobAsync(JobMatchRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.ResumeText))
            {
                throw new ArgumentException("Resume text is required.");
            }

            if (string.IsNullOrWhiteSpace(request.JobDescription))
            {
                throw new ArgumentException("Job description is required.");
            }

            var metrics = BuildMetrics(request.ResumeText, request.JobDescription);
            var providerResult = await TryGetProviderResultAsync(BuildJobMatchPrompt(request, metrics));
            var usedProvider = IsRealProviderResult(providerResult);
            var jobTitle = string.IsNullOrWhiteSpace(request.JobTitle)
                ? "Selected role"
                : request.JobTitle.Trim();

            return new JobMatchDto(
                jobTitle,
                metrics.MatchScore,
                metrics.MatchedSkills,
                metrics.MissingSkills,
                metrics.ExperienceFit,
                metrics.EducationFit,
                BuildRecommendation(metrics.MatchScore),
                BuildRationale(metrics),
                BuildInterviewFocus(metrics),
                usedProvider,
                providerResult,
                DateTime.UtcNow);
        }

        public async Task<CandidateRankingResponseDto> RankCandidatesAsync(CandidateRankingRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.JobDescription))
            {
                throw new ArgumentException("Job description is required.");
            }

            if (request.Candidates == null || request.Candidates.Count == 0)
            {
                throw new ArgumentException("At least one candidate summary is required.");
            }

            var ranked = request.Candidates
                .Where(c => !string.IsNullOrWhiteSpace(c.Summary))
                .Select(c =>
                {
                    var metrics = BuildMetrics(c.Summary, request.JobDescription);
                    return new CandidateRankDto(
                        0,
                        c.CandidateId,
                        string.IsNullOrWhiteSpace(c.CandidateName) ? "Candidate" : c.CandidateName.Trim(),
                        metrics.MatchScore,
                        metrics.MatchedSkills,
                        metrics.MissingSkills,
                        BuildRationale(metrics),
                        BuildRecommendation(metrics.MatchScore));
                })
                .OrderByDescending(c => c.Score)
                .Select((candidate, index) => candidate with { Rank = index + 1 })
                .ToList();

            var providerResult = await TryGetProviderResultAsync(BuildRankingPrompt(request, ranked));
            var usedProvider = IsRealProviderResult(providerResult);

            return new CandidateRankingResponseDto(
                null,
                string.IsNullOrWhiteSpace(request.JobTitle) ? "Selected role" : request.JobTitle.Trim(),
                ranked,
                usedProvider,
                providerResult,
                DateTime.UtcNow);
        }

        public async Task<CandidateRankingResponseDto> RankJobApplicantsAsync(Guid jobId)
        {
            var job = await _jobRepository.GetByIdAsync(jobId)
                ?? throw new KeyNotFoundException("Job not found.");

            var applications = await _applicationRepository.GetByJobIdAsync(jobId);
            var resumes = await _resumeRepository.GetAllAsync();
            var jobDescription = BuildJobDescription(job.Title, job.Description, job.Requirements);

            var candidates = applications.Select(application =>
            {
                var resume = resumes
                    .Where(r => r.CandidateId == application.CandidateId && r.IsActive && !r.IsDeleted)
                    .OrderByDescending(r => r.Version)
                    .ThenByDescending(r => r.CreatedAt)
                    .FirstOrDefault();

                var candidateName = application.Candidate == null
                    ? "Candidate"
                    : $"{application.Candidate.FirstName} {application.Candidate.LastName}".Trim();

                var summary = resume?.ParsedText;
                if (string.IsNullOrWhiteSpace(summary))
                {
                    summary = string.Join(
                        Environment.NewLine,
                        application.Candidate?.Bio,
                        application.Candidate?.Experience,
                        application.Candidate?.Education);
                }

                return new CandidateSummaryDto(
                    application.CandidateId,
                    string.IsNullOrWhiteSpace(candidateName) ? "Candidate" : candidateName,
                    string.IsNullOrWhiteSpace(summary) ? "No resume text available." : summary);
            }).ToList();

            var result = await RankCandidatesAsync(new CandidateRankingRequestDto(
                jobDescription,
                candidates,
                job.Title));

            return result with { JobId = job.Id };
        }

        public async Task<InterviewQuestionsResponseDto> GenerateInterviewQuestionsAsync(
            InterviewQuestionsRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.JobDescription))
            {
                throw new ArgumentException("Job description is required.");
            }

            if (string.IsNullOrWhiteSpace(request.CandidateSummary))
            {
                throw new ArgumentException("Candidate summary is required.");
            }

            var count = Math.Clamp(request.Count <= 0 ? 6 : request.Count, 3, 12);
            var metrics = BuildMetrics(request.CandidateSummary, request.JobDescription);
            var focusAreas = BuildInterviewFocus(metrics).Take(count).ToList();
            var questions = BuildQuestions(focusAreas, count);
            var providerResult = await TryGetProviderResultAsync(
                BuildInterviewQuestionsPrompt(request, focusAreas, count));
            var usedProvider = IsRealProviderResult(providerResult);

            return new InterviewQuestionsResponseDto(
                questions,
                focusAreas,
                usedProvider,
                providerResult,
                DateTime.UtcNow);
        }

        private async Task<string> TryGetProviderResultAsync(string prompt)
        {
            try
            {
                return await _adapter.GetCompletionAsync(prompt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI provider request failed.");
                return "[ai-error] AI provider request failed.";
            }
        }

        private static AtsMetrics BuildMetrics(string resumeText, string? jobDescription)
        {
            var cleanResume = NormalizeWhitespace(resumeText);
            var cleanJob = NormalizeWhitespace(jobDescription ?? string.Empty);
            var hasJob = !string.IsNullOrWhiteSpace(cleanJob);

            var resumeSkills = ExtractSkills(cleanResume);
            var jobSkills = ExtractSkills(cleanJob);
            var resumeKeywords = ExtractKeywords(cleanResume, 20);
            var jobKeywords = ExtractKeywords(cleanJob, 20);
            var matchedSkills = hasJob
                ? jobSkills.Intersect(resumeSkills, StringComparer.OrdinalIgnoreCase).ToList()
                : resumeSkills.Take(12).ToList();
            var missingSkills = hasJob
                ? jobSkills.Except(resumeSkills, StringComparer.OrdinalIgnoreCase).Take(12).ToList()
                : new List<string>();
            var matchedKeywords = hasJob
                ? jobKeywords.Intersect(resumeKeywords, StringComparer.OrdinalIgnoreCase).ToList()
                : resumeKeywords;

            var atsIssues = BuildAtsIssues(cleanResume, resumeSkills);
            var strengths = BuildStrengths(cleanResume, resumeSkills, matchedSkills);
            var suggestions = BuildSuggestions(cleanResume, missingSkills, atsIssues);

            var atsScore = CalculateAtsScore(cleanResume, resumeSkills, atsIssues);
            var matchScore = hasJob
                ? CalculateMatchScore(cleanResume, jobSkills, matchedSkills, jobKeywords, matchedKeywords, atsScore)
                : atsScore;

            return new AtsMetrics(
                atsScore,
                matchScore,
                matchedSkills,
                missingSkills,
                strengths,
                suggestions,
                atsIssues,
                resumeKeywords,
                BuildExperienceFit(cleanResume),
                BuildEducationFit(cleanResume));
        }

        private static int CalculateAtsScore(string resumeText, IReadOnlyList<string> resumeSkills, IReadOnlyList<string> atsIssues)
        {
            var score = 20;

            if (Regex.IsMatch(resumeText, @"[A-Z0-9._%+\-]+@[A-Z0-9.\-]+\.[A-Z]{2,}", RegexOptions.IgnoreCase))
            {
                score += 10;
            }

            if (Regex.IsMatch(resumeText, @"(\+?\d[\d\s().-]{7,}\d)"))
            {
                score += 8;
            }

            if (HasAny(resumeText, "summary", "profile", "objective"))
            {
                score += 8;
            }

            if (HasAny(resumeText, "experience", "employment", "work history"))
            {
                score += 14;
            }

            if (HasAny(resumeText, "education", "degree", "university", "college", "diploma"))
            {
                score += 10;
            }

            if (HasAny(resumeText, "skills", "technical skills", "core skills"))
            {
                score += 10;
            }

            score += Math.Min(resumeSkills.Count * 3, 18);

            var words = CountWords(resumeText);
            if (words is >= 250 and <= 1200)
            {
                score += 12;
            }
            else if (words is >= 120 and <= 1800)
            {
                score += 7;
            }
            else
            {
                score += 3;
            }

            score -= Math.Min(atsIssues.Count * 4, 20);

            return ClampScore(score);
        }

        private static int CalculateMatchScore(
            string resumeText,
            IReadOnlyList<string> jobSkills,
            IReadOnlyList<string> matchedSkills,
            IReadOnlyList<string> jobKeywords,
            IReadOnlyList<string> matchedKeywords,
            int atsScore)
        {
            var skillScore = jobSkills.Count == 0
                ? 20
                : (int)Math.Round((double)matchedSkills.Count / jobSkills.Count * 45);

            var keywordScore = jobKeywords.Count == 0
                ? 20
                : (int)Math.Round((double)matchedKeywords.Count / jobKeywords.Count * 30);

            var experienceScore = HasAny(resumeText, "experience", "project", "developed", "managed", "implemented")
                ? 12
                : 4;

            var educationScore = HasAny(resumeText, "education", "degree", "university", "college", "diploma")
                ? 8
                : 3;

            var atsQualityScore = (int)Math.Round(atsScore * 0.05);

            return ClampScore(skillScore + keywordScore + experienceScore + educationScore + atsQualityScore);
        }

        private static IReadOnlyList<string> BuildAtsIssues(string resumeText, IReadOnlyList<string> resumeSkills)
        {
            var issues = new List<string>();

            if (!Regex.IsMatch(resumeText, @"[A-Z0-9._%+\-]+@[A-Z0-9.\-]+\.[A-Z]{2,}", RegexOptions.IgnoreCase))
            {
                issues.Add("Missing clear email address.");
            }

            if (!Regex.IsMatch(resumeText, @"(\+?\d[\d\s().-]{7,}\d)"))
            {
                issues.Add("Missing clear phone number.");
            }

            if (!HasAny(resumeText, "skills", "technical skills", "core skills") && resumeSkills.Count < 4)
            {
                issues.Add("Skills section is weak or missing.");
            }

            if (!HasAny(resumeText, "experience", "employment", "work history", "project"))
            {
                issues.Add("Experience or project section is missing.");
            }

            if (!HasAny(resumeText, "education", "degree", "university", "college", "diploma"))
            {
                issues.Add("Education section is missing.");
            }

            if (CountWords(resumeText) < 120)
            {
                issues.Add("Resume text is too short for strong ATS matching.");
            }

            if (resumeText.StartsWith("Extracted text from", StringComparison.OrdinalIgnoreCase))
            {
                issues.Add("Resume text extraction is limited. Upload a text-searchable PDF, DOCX, or TXT file.");
            }

            return issues;
        }

        private static IReadOnlyList<string> BuildStrengths(
            string resumeText,
            IReadOnlyList<string> resumeSkills,
            IReadOnlyList<string> matchedSkills)
        {
            var strengths = new List<string>();

            if (resumeSkills.Count >= 6)
            {
                strengths.Add("Strong skills coverage.");
            }

            if (matchedSkills.Count >= 3)
            {
                strengths.Add("Good match with role-specific keywords.");
            }

            if (HasAny(resumeText, "project", "implemented", "developed", "built", "managed"))
            {
                strengths.Add("Includes delivery or project experience.");
            }

            if (HasAny(resumeText, "azure", "aws", "cloud", "devops", "docker"))
            {
                strengths.Add("Includes modern cloud or DevOps experience.");
            }

            if (strengths.Count == 0)
            {
                strengths.Add("Resume has enough information for initial screening.");
            }

            return strengths;
        }

        private static IReadOnlyList<string> BuildSuggestions(
            string resumeText,
            IReadOnlyList<string> missingSkills,
            IReadOnlyList<string> atsIssues)
        {
            var suggestions = new List<string>();

            if (missingSkills.Count > 0)
            {
                suggestions.Add($"Add evidence for missing role keywords: {string.Join(", ", missingSkills.Take(6))}.");
            }

            if (!HasAny(resumeText, "summary", "profile", "objective"))
            {
                suggestions.Add("Add a short professional summary with target role keywords.");
            }

            if (!Regex.IsMatch(resumeText, @"\d+%|\d+\s*(users|projects|clients|revenue|hours|tickets)", RegexOptions.IgnoreCase))
            {
                suggestions.Add("Add measurable achievements such as percentages, users, projects, or business impact.");
            }

            foreach (var issue in atsIssues.Take(4))
            {
                suggestions.Add($"Fix ATS issue: {issue}");
            }

            return suggestions.Distinct(StringComparer.OrdinalIgnoreCase).Take(8).ToList();
        }

        private static IReadOnlyList<string> BuildInterviewFocus(AtsMetrics metrics)
        {
            var focus = new List<string>();

            focus.AddRange(metrics.MissingSkills.Take(5));

            if (metrics.MatchScore < 70)
            {
                focus.Add("role fit");
                focus.Add("practical project experience");
            }

            if (metrics.AtsIssues.Count > 0)
            {
                focus.Add("resume gaps");
            }

            if (focus.Count == 0)
            {
                focus.AddRange(metrics.MatchedSkills.Take(5));
            }

            if (focus.Count == 0)
            {
                focus.Add("technical depth");
                focus.Add("communication");
            }

            return focus.Distinct(StringComparer.OrdinalIgnoreCase).Take(8).ToList();
        }

        private static IReadOnlyList<string> BuildQuestions(IReadOnlyList<string> focusAreas, int count)
        {
            var questions = focusAreas
                .Select(focus => $"Can you explain your hands-on experience with {focus} and give one real example?")
                .ToList();

            questions.Add("What was the most challenging project you worked on, and what was your exact contribution?");
            questions.Add("How do you validate the quality of your work before delivery?");
            questions.Add("Tell us about a time you had to learn a missing skill quickly for a project.");
            questions.Add("How would you approach the first 30 days in this role?");

            return questions.Distinct(StringComparer.OrdinalIgnoreCase).Take(count).ToList();
        }

        private static string BuildSummary(string candidateName, AtsMetrics metrics)
            => $"{candidateName} has an ATS score of {metrics.AtsScore} and a job match score of {metrics.MatchScore}. {BuildRecommendation(metrics.MatchScore)}";

        private static string BuildRecommendation(int score)
            => score switch
            {
                >= 85 => "Strong match. Prioritize for screening.",
                >= 70 => "Good match. Review experience details and shortlist if role requirements align.",
                >= 55 => "Moderate match. Consider only if trainable gaps are acceptable.",
                _ => "Weak match. Significant role keyword or experience gaps found."
            };

        private static string BuildRationale(AtsMetrics metrics)
        {
            var matched = metrics.MatchedSkills.Count == 0
                ? "few direct skill matches"
                : $"{metrics.MatchedSkills.Count} matched skill(s): {string.Join(", ", metrics.MatchedSkills.Take(5))}";

            var missing = metrics.MissingSkills.Count == 0
                ? "no major missing skills detected"
                : $"missing: {string.Join(", ", metrics.MissingSkills.Take(5))}";

            return $"The score is based on {matched}, {missing}, resume structure, experience signals, and education signals.";
        }

        private static string BuildExperienceFit(string resumeText)
        {
            var years = ExtractYears(resumeText);
            if (years > 0)
            {
                return $"{years}+ year(s) of experience detected.";
            }

            return HasAny(resumeText, "experience", "project", "developed", "implemented")
                ? "Experience is present, but years are not clearly stated."
                : "Experience evidence is limited.";
        }

        private static string BuildEducationFit(string resumeText)
            => HasAny(resumeText, "education", "degree", "university", "college", "diploma")
                ? "Education evidence is present."
                : "Education evidence is not clearly stated.";

        private static string BuildJobDescription(string title, string description, string requirements)
            => NormalizeWhitespace($"{title}\n{description}\n{requirements}");

        private static string BuildResumeAnalysisPrompt(ResumeAnalysisRequestDto request, AtsMetrics metrics)
            => $"""
               Act as an ATS recruitment analyst. Review this resume and return concise hiring feedback.
               Current deterministic ATS score: {metrics.AtsScore}
               Current deterministic job match score: {metrics.MatchScore}
               Matched skills: {string.Join(", ", metrics.MatchedSkills)}
               Missing skills: {string.Join(", ", metrics.MissingSkills)}
               Job description: {request.JobDescription}
               Resume:
               {request.ResumeText}
               """;

        private static string BuildJobMatchPrompt(JobMatchRequestDto request, AtsMetrics metrics)
            => $"""
               Act as an ATS job matching analyst. Explain the score and the candidate-role fit.
               Job title: {request.JobTitle}
               Deterministic match score: {metrics.MatchScore}
               Matched skills: {string.Join(", ", metrics.MatchedSkills)}
               Missing skills: {string.Join(", ", metrics.MissingSkills)}
               Job description:
               {request.JobDescription}
               Resume:
               {request.ResumeText}
               """;

        private static string BuildRankingPrompt(
            CandidateRankingRequestDto request,
            IReadOnlyList<CandidateRankDto> ranked)
            => $"""
               Act as a recruitment screening analyst. Review this candidate ranking and note any risks.
               Job title: {request.JobTitle}
               Job description: {request.JobDescription}
               Ranking:
               {string.Join("\n", ranked.Select(c => $"{c.Rank}. {c.CandidateName} - {c.Score}"))}
               """;

        private static string BuildInterviewQuestionsPrompt(
            InterviewQuestionsRequestDto request,
            IReadOnlyList<string> focusAreas,
            int count)
            => $"""
               Generate {count} interview questions for this candidate and job.
               Focus areas: {string.Join(", ", focusAreas)}
               Job description:
               {request.JobDescription}
               Candidate summary:
               {request.CandidateSummary}
               """;

        private static IReadOnlyList<string> ExtractSkills(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Array.Empty<string>();
            }

            return SkillKeywords
                .Where(skill => ContainsTerm(text, skill))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(skill => skill)
                .ToList();
        }

        private static IReadOnlyList<string> ExtractKeywords(string text, int max)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Array.Empty<string>();
            }

            var skillMatches = ExtractSkills(text);
            var wordMatches = Regex.Matches(text.ToLowerInvariant(), @"[a-z][a-z0-9+#.-]{2,}")
                .Select(m => m.Value.Trim('.', '-'))
                .Where(word => word.Length > 2 && !StopWords.Contains(word))
                .GroupBy(word => word, StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(group => group.Count())
                .ThenBy(group => group.Key)
                .Select(group => group.Key)
                .Take(max)
                .ToList();

            return skillMatches
                .Concat(wordMatches)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(max)
                .ToList();
        }

        private static bool ContainsTerm(string text, string term)
        {
            if (term.Length <= 3 || term.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                return text.Contains(term, StringComparison.OrdinalIgnoreCase);
            }

            return Regex.IsMatch(text, $@"\b{Regex.Escape(term)}\b", RegexOptions.IgnoreCase);
        }

        private static bool HasAny(string text, params string[] terms)
            => terms.Any(term => ContainsTerm(text, term));

        private static int CountWords(string text)
            => Regex.Matches(text, @"\b[\w+#.-]+\b").Count;

        private static int ExtractYears(string text)
        {
            var matches = Regex.Matches(text, @"(\d{1,2})\+?\s*(years|year|yrs|yr)", RegexOptions.IgnoreCase);
            return matches.Count == 0
                ? 0
                : matches.Select(m => int.Parse(m.Groups[1].Value)).Max();
        }

        private static int ClampScore(int score)
            => Math.Clamp(score, 0, 100);

        private static string NormalizeWhitespace(string text)
            => Regex.Replace(text ?? string.Empty, @"\s+", " ").Trim();

        private static bool IsRealProviderResult(string result)
            => !string.IsNullOrWhiteSpace(result)
                && !result.StartsWith("[ai-not-configured]", StringComparison.OrdinalIgnoreCase)
                && !result.StartsWith("[ai-error]", StringComparison.OrdinalIgnoreCase)
                && !result.StartsWith("[mock-ai-response]", StringComparison.OrdinalIgnoreCase);

        private record AtsMetrics(
            int AtsScore,
            int MatchScore,
            IReadOnlyList<string> MatchedSkills,
            IReadOnlyList<string> MissingSkills,
            IReadOnlyList<string> Strengths,
            IReadOnlyList<string> Suggestions,
            IReadOnlyList<string> AtsIssues,
            IReadOnlyList<string> Keywords,
            string ExperienceFit,
            string EducationFit);
    }
}
