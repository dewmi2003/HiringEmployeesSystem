using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.DTOs.Companies;
using Recruitment.Application.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Recruitment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _service;

        public CompanyController(ICompanyService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var company = await _service.GetByIdAsync(id);

            if (company == null)
                return NotFound();

            return Ok(company);
        }

        [Authorize(Roles = "Admin,Recruiter")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCompanyDto dto)
        {
            var company = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(Get),
                new { id = company.Id },
                company);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);

            return NoContent();
        }
    }
}