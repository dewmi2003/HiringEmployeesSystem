using System;

namespace Recruitment.Application.DTOs.Companies
{
    public class CompanyDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Website { get; set; }

        public string? Address { get; set; }
    }
}