using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;
using Recruitment.Application.DTO.Resumes;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Recruitment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ResumesController : ControllerBase
    {
        private readonly IResumeService _resumeService;
        private readonly ICandidateRepository _candidateRepository;

        public ResumesController(IResumeService resumeService, ICandidateRepository candidateRepository)
        {
            _resumeService = resumeService;
            _candidateRepository = candidateRepository;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] Guid? candidateId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded or file is empty.");
            }

            // Validation of file extension
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".txt" };
            var extension = System.IO.Path.GetExtension(file.FileName).ToLower();
            if (Array.IndexOf(allowedExtensions, extension) < 0)
            {
                return BadRequest("Invalid file type. Allowed extensions: .pdf, .doc, .docx, .txt");
            }

            // Validation of file size (e.g., 5MB limit)
            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest("File size exceeds 5MB limit.");
            }

            Guid targetCandidateId;

            if (User.IsInRole("Candidate"))
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var candidate = await _candidateRepository.GetByUserIdAsync(userId);
                if (candidate == null) return BadRequest("Candidate profile not found.");
                targetCandidateId = candidate.Id;
            }
            else if (User.IsInRole("Recruiter") || User.IsInRole("Admin"))
            {
                if (!candidateId.HasValue)
                {
                    return BadRequest("CandidateId is required for recruiters or administrators.");
                }
                targetCandidateId = candidateId.Value;
            }
            else
            {
                return Forbid();
            }

            using var stream = file.OpenReadStream();
            var result = await _resumeService.UploadResumeAsync(
                targetCandidateId, 
                stream, 
                file.FileName, 
                file.ContentType, 
                file.Length);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}/update")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(Guid id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded or file is empty.");
            }

            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".txt" };
            var extension = System.IO.Path.GetExtension(file.FileName).ToLower();
            if (Array.IndexOf(allowedExtensions, extension) < 0)
            {
                return BadRequest("Invalid file type. Allowed extensions: .pdf, .doc, .docx, .txt");
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest("File size exceeds 5MB limit.");
            }

            var existing = await _resumeService.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound("Resume not found.");
            }

            // Authorization check
            if (User.IsInRole("Candidate"))
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var candidate = await _candidateRepository.GetByUserIdAsync(userId);
                if (candidate == null || candidate.Id != existing.CandidateId)
                {
                    return Forbid();
                }
            }

            using var stream = file.OpenReadStream();
            var result = await _resumeService.UpdateResumeAsync(
                id, 
                stream, 
                file.FileName, 
                file.ContentType, 
                file.Length);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _resumeService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound("Resume not found.");
            }

            // Authorization check
            if (User.IsInRole("Candidate"))
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var candidate = await _candidateRepository.GetByUserIdAsync(userId);
                if (candidate == null || candidate.Id != result.CandidateId)
                {
                    return Forbid();
                }
            }

            return Ok(result);
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(Guid id)
        {
            try
            {
                var existing = await _resumeService.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound("Resume not found.");
                }

                // Authorization check
                if (User.IsInRole("Candidate"))
                {
                    var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                    var candidate = await _candidateRepository.GetByUserIdAsync(userId);
                    if (candidate == null || candidate.Id != existing.CandidateId)
                    {
                        return Forbid();
                    }
                }

                var (fileStream, contentType, fileName) = await _resumeService.DownloadResumeAsync(id);
                return File(fileStream, contentType, fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _resumeService.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound("Resume not found.");
            }

            // Authorization check
            if (User.IsInRole("Candidate"))
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var candidate = await _candidateRepository.GetByUserIdAsync(userId);
                if (candidate == null || candidate.Id != existing.CandidateId)
                {
                    return Forbid();
                }
            }
            else if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            await _resumeService.DeleteResumeAsync(id);
            return NoContent();
        }

        [HttpDelete("{id}/soft")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            var existing = await _resumeService.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound("Resume not found.");
            }

            // Authorization check
            if (User.IsInRole("Candidate"))
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var candidate = await _candidateRepository.GetByUserIdAsync(userId);
                if (candidate == null || candidate.Id != existing.CandidateId)
                {
                    return Forbid();
                }
            }
            else if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            await _resumeService.SoftDeleteResumeAsync(id);
            return NoContent();
        }

        [HttpGet("candidate/{candidateId}/history")]
        public async Task<IActionResult> GetVersionHistory(Guid candidateId)
        {
            // Authorization check
            if (User.IsInRole("Candidate"))
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var candidate = await _candidateRepository.GetByUserIdAsync(userId);
                if (candidate == null || candidate.Id != candidateId)
                {
                    return Forbid();
                }
            }

            var result = await _resumeService.GetVersionHistoryAsync(candidateId);
            return Ok(result);
        }

        [HttpGet("search")]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> Search([FromQuery] string searchTerm)
        {
            var result = await _resumeService.SearchResumesAsync(searchTerm);
            return Ok(result);
        }
    }
}
