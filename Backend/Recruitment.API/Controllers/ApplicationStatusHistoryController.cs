using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.DTOs.HiringDecisions;
using Recruitment.Application.Interfaces.Services;

namespace Recruitment.API.Controllers
{
    /// <summary>Phase 3 – Application Status History (audit trail) endpoints.</summary>
    [ApiController]
    [Route("api/application-status-history")]
    [Authorize]
    public class ApplicationStatusHistoryController : ControllerBase
    {
        private readonly IApplicationStatusHistoryService _historyService;

        public ApplicationStatusHistoryController(IApplicationStatusHistoryService historyService)
        {
            _historyService = historyService;
        }

        /// <summary>Get the full status change history for an application.</summary>
        [HttpGet("{applicationId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<ApplicationStatusHistoryDto>), 200)]
        public async Task<IActionResult> GetByApplication(Guid applicationId)
        {
            var results = await _historyService.GetByApplicationIdAsync(applicationId);
            return Ok(results);
        }
    }
}
