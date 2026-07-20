using Recruitment.Application.DTOs.Companies;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;
using Recruitment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Recruitment.Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _repository;

        public CompanyService(ICompanyRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CompanyDto>> GetAllAsync()
        {
            var companies = await _repository.GetAllAsync();

            return companies.Select(c => new CompanyDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Website = c.Website,
                Address = c.Address
            });
        }

        public async Task<CompanyDto?> GetByIdAsync(Guid id)
        {
            var company = await _repository.GetByIdAsync(id);

            if (company == null)
                return null;

            return new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                Description = company.Description,
                Website = company.Website,
                Address = company.Address
            };
        }

        public async Task<CompanyDto> CreateAsync(CreateCompanyDto dto)
        {
            var existing = await _repository.GetByNameAsync(dto.Name);

            if (existing != null)
                throw new InvalidOperationException("Company already exists.");

            var company = new Company
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Website = dto.Website,
                Address = dto.Address
            };

            await _repository.AddAsync(company);

            return new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                Description = company.Description,
                Website = company.Website,
                Address = company.Address
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            var company = await _repository.GetByIdAsync(id);

            if (company == null)
                throw new KeyNotFoundException("Company not found.");

            await _repository.DeleteAsync(company);
        }
    }
}