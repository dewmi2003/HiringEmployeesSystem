using System;

namespace Recruitment.Application.DTO.Resumes
{
    public class ResumeDto
    {
        public Guid Id { get; set; }

        public Guid CandidateId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public DateTime UploadedDate { get; set; }
    }
}