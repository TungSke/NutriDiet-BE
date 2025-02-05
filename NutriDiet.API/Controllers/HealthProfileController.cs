using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;

namespace NutriDiet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthProfileController : ControllerBase
    {
        private readonly IHealthProfileService _healthprofileService;
        public HealthProfileController(IHealthProfileService healthProfileService)
        {
            _healthprofileService = healthProfileService;
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddUserHealthRecord([FromForm] UserHealthRequest request)
        {
            await _healthprofileService.AddUserHealthRecord(request);
            return Ok();
        }
    }
}
