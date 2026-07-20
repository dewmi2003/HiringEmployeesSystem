using System.ComponentModel.DataAnnotations;

namespace Recruitment.Application.DTOs.Companies
{
    public class UpdateCompanyDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Website { get; set; }

        public string? Address { get; set; }
    }
}