using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Recruitment.Application.DTO.Resumes;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;
using Recruitment.Domain.Entities;
using Recruitment.Infrastructure.AI;

namespace Recruitment.Infrastructure.Services
{
    public class ResumeService : IResumeService
    {
        private readonly IResumeRepository _resumeRepository;
        private readonly ICandidateRepository _candidateRepository;
        private readonly IBlobStorage _blobStorage;
        private readonly IResumeAiService _resumeAiService;

        public ResumeService(
            IResumeRepository resumeRepository,
            ICandidateRepository candidateRepository,
            IBlobStorage blobStorage,
            IResumeAiService resumeAiService)
        {
            _resumeRepository = resumeRepository;
            _candidateRepository = candidateRepository;
            _blobStorage = blobStorage;
            _resumeAiService = resumeAiService;
        }

        public async Task<ResumeDto> UploadResumeAsync(Guid candidateId, Stream fileStream, string fileName, string contentType, long fileSize)
        {
            var candidate = await _candidateRepository.GetByIdAsync(candidateId);
            if (candidate == null)
            {
                throw new ArgumentException("Candidate not found.");
            }

            // Deactivate any existing active resumes for this candidate
            var existingResumes = await _resumeRepository.GetAllAsync();
            var candidateResumes = existingResumes.Where(r => r.CandidateId == candidateId && r.IsActive).ToList();
            foreach (var oldResume in candidateResumes)
            {
                oldResume.IsActive = false;
                await _resumeRepository.UpdateAsync(oldResume);
            }

            // Determine version
            var allResumes = existingResumes.Where(r => r.CandidateId == candidateId).ToList();
            int nextVersion = allResumes.Any() ? allResumes.Max(r => r.Version) + 1 : 1;

            await using var bufferedStream = new MemoryStream();
            await fileStream.CopyToAsync(bufferedStream);

            // Upload to storage
            bufferedStream.Position = 0;
            string storagePath = await _blobStorage.UploadAsync(bufferedStream, contentType, fileName);

            // Extract text
            bufferedStream.Position = 0;
            string parsedText = await ExtractTextAsync(bufferedStream, fileName, fileSize);

            // AI Parsing & Score computation
            int aiScore = 0;
            string aiAnalysis = "";
            try
            {
                aiAnalysis = await _resumeAiService.ParseResumeAsync(parsedText);
                aiScore = CalculateAiScore(parsedText, aiAnalysis);
            }
            catch
            {
                aiScore = CalculateLocalAtsScore(parsedText);
            }

            var now = DateTime.UtcNow;

            // Create new Resume
            var resume = new Resume
            {
                Id = Guid.NewGuid(),
                CandidateId = candidateId,
                FilePath = storagePath,
                FileName = fileName,
                FileSize = fileSize,
                FileType = contentType,
                ParsedText = parsedText,
                AiScore = aiScore,
                Version = nextVersion,
                IsActive = true,
                IsDeleted = false,
                UploadedDate = now,
                CreatedAt = now
            };

            await _resumeRepository.AddAsync(resume);

            return MapToDto(resume);
        }

        public async Task<ResumeDto> UpdateResumeAsync(Guid resumeId, Stream fileStream, string fileName, string contentType, long fileSize)
        {
            var oldResume = await _resumeRepository.GetByIdAsync(resumeId);
            if (oldResume == null)
            {
                throw new ArgumentException("Resume not found.");
            }

            return await UploadResumeAsync(oldResume.CandidateId, fileStream, fileName, contentType, fileSize);
        }

        public async Task<(Stream FileStream, string ContentType, string FileName)> DownloadResumeAsync(Guid id)
        {
            var resume = await _resumeRepository.GetByIdAsync(id);
            if (resume == null)
            {
                throw new ArgumentException("Resume not found.");
            }

            var stream = await _blobStorage.DownloadAsync(resume.FilePath);
            if (stream == null)
            {
                throw new FileNotFoundException("Resume file not found in storage.");
            }

            var contentType = GetContentType(resume.FileType);
            return (stream, contentType, resume.FileName);
        }

        public async Task DeleteResumeAsync(Guid id)
        {
            var resume = await _resumeRepository.GetByIdAsync(id);
            if (resume != null)
            {
                await _resumeRepository.DeleteAsync(resume);

                try
                {
                    await _blobStorage.DeleteAsync(resume.FilePath);
                }
                catch
                {
                    // ignore
                }
            }
        }

        public async Task SoftDeleteResumeAsync(Guid id)
        {
            var resume = await _resumeRepository.GetByIdAsync(id);
            if (resume != null)
            {
                resume.IsDeleted = true;
                resume.IsActive = false;
                await _resumeRepository.UpdateAsync(resume);

                // If this was the active resume, activate candidate's most recent remaining active resume
                var allResumes = await _resumeRepository.GetAllAsync();
                var remaining = allResumes
                    .Where(r => r.CandidateId == resume.CandidateId && r.Id != id && !r.IsDeleted)
                    .OrderByDescending(r => r.Version)
                    .ToList();

                if (remaining.Any())
                {
                    var latest = remaining.First();
                    latest.IsActive = true;
                    await _resumeRepository.UpdateAsync(latest);
                }
            }
        }

        public async Task<IEnumerable<ResumeDto>> GetVersionHistoryAsync(Guid candidateId)
        {
            var allResumes = await _resumeRepository.GetAllAsync();
            return allResumes
                .Where(r => r.CandidateId == candidateId)
                .OrderByDescending(r => r.Version)
                .Select(MapToDto);
        }

        public async Task<IEnumerable<ResumeDto>> SearchResumesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Enumerable.Empty<ResumeDto>();
            }

            var allResumes = await _resumeRepository.GetAllAsync();
            return allResumes
                .Where(r => r.FileName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            (r.ParsedText ?? string.Empty).Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .Select(MapToDto);
        }

        public async Task<ResumeDto?> GetByIdAsync(Guid id)
        {
            var resume = await _resumeRepository.GetByIdAsync(id);
            return resume == null ? null : MapToDto(resume);
        }

        private async Task<string> ExtractTextAsync(Stream fileStream, string fileName, long fileSize)
        {
            try
            {
                if (fileStream.CanSeek)
                {
                    fileStream.Position = 0;
                }

                using var memory = new MemoryStream();
                await fileStream.CopyToAsync(memory);
                var bytes = memory.ToArray();
                var extension = Path.GetExtension(fileName).ToLowerInvariant();

                var extractedText = extension switch
                {
                    ".txt" => NormalizeExtractedText(Encoding.UTF8.GetString(bytes)),
                    ".docx" => ExtractDocxText(bytes),
                    ".pdf" => ExtractPdfText(bytes),
                    ".doc" => ExtractLegacyDocText(bytes),
                    _ => string.Empty
                };

                if (!string.IsNullOrWhiteSpace(extractedText))
                {
                    return extractedText;
                }
            }
            catch
            {
                // Fallback
            }

            return $"Text extraction from {fileName} was limited. File size is {fileSize} bytes.";
        }

        private static string ExtractDocxText(byte[] bytes)
        {
            using var memory = new MemoryStream(bytes);
            using var archive = new ZipArchive(memory, ZipArchiveMode.Read);
            var builder = new StringBuilder();

            foreach (var entry in archive.Entries.Where(entry =>
                         entry.FullName.Equals("word/document.xml", StringComparison.OrdinalIgnoreCase) ||
                         entry.FullName.StartsWith("word/header", StringComparison.OrdinalIgnoreCase) ||
                         entry.FullName.StartsWith("word/footer", StringComparison.OrdinalIgnoreCase)))
            {
                AppendOpenXmlText(entry, builder);
            }

            return NormalizeExtractedText(builder.ToString());
        }

        private static void AppendOpenXmlText(ZipArchiveEntry entry, StringBuilder builder)
        {
            using var stream = entry.Open();
            var document = XDocument.Load(stream);
            XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

            foreach (var paragraph in document.Descendants(word + "p"))
            {
                var text = string.Join(" ", paragraph.Descendants(word + "t").Select(node => node.Value));
                if (!string.IsNullOrWhiteSpace(text))
                {
                    builder.AppendLine(text);
                }
            }
        }

        private static string ExtractPdfText(byte[] bytes)
        {
            var raw = Encoding.Latin1.GetString(bytes);
            var chunks = new List<string>();
            chunks.AddRange(ExtractPdfStrings(raw));

            foreach (Match streamMatch in Regex.Matches(raw, @"stream\r?\n(?<data>.*?)\r?\nendstream", RegexOptions.Singleline))
            {
                var headerStart = Math.Max(0, streamMatch.Index - 300);
                var header = raw.Substring(headerStart, streamMatch.Index - headerStart);
                if (!header.Contains("FlateDecode", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                try
                {
                    var compressedBytes = Encoding.Latin1.GetBytes(streamMatch.Groups["data"].Value.Trim('\r', '\n'));
                    using var compressed = new MemoryStream(compressedBytes);
                    using var zlib = new ZLibStream(compressed, CompressionMode.Decompress);
                    using var decompressed = new MemoryStream();
                    zlib.CopyTo(decompressed);

                    chunks.AddRange(ExtractPdfStrings(Encoding.Latin1.GetString(decompressed.ToArray())));
                }
                catch
                {
                    // Some PDF streams use filters we do not support without an external parser.
                }
            }

            return NormalizeExtractedText(string.Join(" ", chunks));
        }

        private static IEnumerable<string> ExtractPdfStrings(string content)
        {
            foreach (Match match in Regex.Matches(content, @"\((?:\\.|[^\\()]){2,}\)"))
            {
                var decoded = DecodePdfLiteral(match.Value);
                if (LooksLikeHumanText(decoded))
                {
                    yield return decoded;
                }
            }

            foreach (Match match in Regex.Matches(content, @"<(?<hex>[0-9A-Fa-f\s]{6,})>"))
            {
                var decoded = DecodePdfHex(match.Groups["hex"].Value);
                if (LooksLikeHumanText(decoded))
                {
                    yield return decoded;
                }
            }
        }

        private static string DecodePdfLiteral(string value)
        {
            var inner = value.Length >= 2 ? value[1..^1] : value;
            var builder = new StringBuilder();

            for (var i = 0; i < inner.Length; i++)
            {
                if (inner[i] != '\\' || i == inner.Length - 1)
                {
                    builder.Append(inner[i]);
                    continue;
                }

                var escaped = inner[++i];
                builder.Append(escaped switch
                {
                    'n' => '\n',
                    'r' => '\r',
                    't' => '\t',
                    'b' => '\b',
                    'f' => '\f',
                    _ => escaped
                });
            }

            return builder.ToString();
        }

        private static string DecodePdfHex(string hex)
        {
            var cleanHex = Regex.Replace(hex, @"\s+", string.Empty);
            if (cleanHex.Length % 2 == 1)
            {
                cleanHex += "0";
            }

            var bytes = new byte[cleanHex.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(cleanHex.Substring(i * 2, 2), 16);
            }

            if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
            {
                return Encoding.BigEndianUnicode.GetString(bytes, 2, bytes.Length - 2);
            }

            return Encoding.UTF8.GetString(bytes);
        }

        private static string ExtractLegacyDocText(byte[] bytes)
        {
            var text = Encoding.Latin1.GetString(bytes);
            var printable = Regex.Replace(text, @"[^\u0020-\u007E\r\n\t]", " ");
            return NormalizeExtractedText(printable);
        }

        private static string NormalizeExtractedText(string text)
        {
            var withoutXmlEntities = text
                .Replace("&amp;", "&", StringComparison.OrdinalIgnoreCase)
                .Replace("&lt;", "<", StringComparison.OrdinalIgnoreCase)
                .Replace("&gt;", ">", StringComparison.OrdinalIgnoreCase);

            return Regex.Replace(withoutXmlEntities, @"\s+", " ").Trim();
        }

        private static bool LooksLikeHumanText(string text)
            => Regex.Matches(text, @"[A-Za-z]{2,}").Count >= 2;

        private static int CalculateAiScore(string parsedText, string aiAnalysis)
        {
            var providerScore = TryExtractProviderScore(aiAnalysis);
            if (providerScore.HasValue)
            {
                return providerScore.Value;
            }

            return CalculateLocalAtsScore(parsedText);
        }

        private static int? TryExtractProviderScore(string aiAnalysis)
        {
            if (string.IsNullOrWhiteSpace(aiAnalysis) ||
                aiAnalysis.StartsWith("[mock-ai-response]", StringComparison.OrdinalIgnoreCase) ||
                aiAnalysis.StartsWith("[ai-not-configured]", StringComparison.OrdinalIgnoreCase) ||
                aiAnalysis.StartsWith("[ai-error]", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var match = Regex.Match(
                aiAnalysis,
                @"(?:ats|score|match)[^\d]{0,25}(?<score>\d{1,3})",
                RegexOptions.IgnoreCase);

            return match.Success && int.TryParse(match.Groups["score"].Value, out var score)
                ? Math.Clamp(score, 0, 100)
                : null;
        }

        private static int CalculateLocalAtsScore(string text)
        {
            var score = 20;

            if (Regex.IsMatch(text, @"[A-Z0-9._%+\-]+@[A-Z0-9.\-]+\.[A-Z]{2,}", RegexOptions.IgnoreCase))
            {
                score += 10;
            }

            if (Regex.IsMatch(text, @"\+?\d[\d\s().-]{7,}\d"))
            {
                score += 8;
            }

            if (HasAny(text, "summary", "profile", "objective"))
            {
                score += 8;
            }

            if (HasAny(text, "experience", "employment", "project", "work history"))
            {
                score += 14;
            }

            if (HasAny(text, "education", "degree", "university", "college", "diploma"))
            {
                score += 10;
            }

            if (HasAny(text, "skills", "technical skills", "core skills"))
            {
                score += 10;
            }

            score += Math.Min(ExtractSkillCount(text) * 3, 18);

            var wordCount = Regex.Matches(text, @"\b[\w+#.-]+\b").Count;
            if (wordCount is >= 250 and <= 1200)
            {
                score += 12;
            }
            else if (wordCount is >= 120 and <= 1800)
            {
                score += 7;
            }
            else
            {
                score += 3;
            }

            if (text.StartsWith("Text extraction from", StringComparison.OrdinalIgnoreCase))
            {
                score -= 18;
            }

            return Math.Clamp(score, 0, 100);
        }

        private static int ExtractSkillCount(string text)
        {
            var skills = new[]
            {
                "c#", ".net", "asp.net", "entity framework", "sql", "azure", "aws", "docker",
                "kubernetes", "react", "typescript", "javascript", "html", "css", "node",
                "python", "java", "api", "rest", "git", "github", "devops", "ci/cd",
                "testing", "agile", "machine learning", "ai", "openai", "power bi"
            };

            return skills.Count(skill => ContainsTerm(text, skill));
        }

        private static bool HasAny(string text, params string[] terms)
            => terms.Any(term => ContainsTerm(text, term));

        private static bool ContainsTerm(string text, string term)
        {
            if (term.Length <= 3 || term.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                return text.Contains(term, StringComparison.OrdinalIgnoreCase);
            }

            return Regex.IsMatch(text, $@"\b{Regex.Escape(term)}\b", RegexOptions.IgnoreCase);
        }

        private string GetContentType(string extension)
        {
            if (extension.Contains('/'))
            {
                return extension;
            }

            return extension.ToLower() switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }

        private ResumeDto MapToDto(Resume resume)
        {
            return new ResumeDto
            {
                Id = resume.Id,
                CandidateId = resume.CandidateId,
                FileName = resume.FileName,
                FilePath = resume.FilePath,
                UploadedDate = resume.UploadedDate == default
                    ? resume.CreatedAt
                    : resume.UploadedDate,
                FileSize = resume.FileSize,
                FileType = resume.FileType,
                ParsedText = resume.ParsedText,
                AiScore = resume.AiScore,
                Version = resume.Version,
                IsActive = resume.IsActive,
                IsDeleted = resume.IsDeleted
            };
        }
    }
}
