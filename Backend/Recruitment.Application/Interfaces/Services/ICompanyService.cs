using Recruitment.Application.DTOs.Companies;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Recruitment.Application.Interfaces.Services
{
    public interface ICompanyService
    {
        Task<IEnumerable<CompanyDto>> GetAllAsync();

        Task<CompanyDto?> GetByIdAsync(Guid id);

        Task<CompanyDto> CreateAsync(CreateCompanyDto dto);

        Task DeleteAsync(Guid id);
    }
}