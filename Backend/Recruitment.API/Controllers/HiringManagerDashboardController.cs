using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.Interfaces.Services;


namespace Recruitment.API.Controllers
{

    [ApiController]
    [Route("api/hiring-manager-dashboard")]
    [Authorize(Roles = "HiringManager,Admin")]
    public class HiringManagerDashboardController 
        : ControllerBase
    {


        private readonly IHiringManagerDashboardService _service;



        public HiringManagerDashboardController(
            IHiringManagerDashboardService service)
        {
            _service = service;
        }




        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {

            var result =
                await _service.GetDashboardAsync();


            return Ok(result);
        }

    }
}