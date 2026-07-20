using System;

namespace Recruitment.Application.DTO.Resumes
{
    public class UploadResumeDto
    {
        public Guid CandidateId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;
    }
}