using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using NutriDiet.Service.Services;
using Sprache;

namespace NutriDiet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthProfileController : ControllerBase
    {
        private readonly IGeneralHealthProfileService _healthprofileService;
        public HealthProfileController(IGeneralHealthProfileService healthProfileService)
        {
            _healthprofileService = healthProfileService;
        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Get() { 
            var result = await _healthprofileService.GetHealthProfile();
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateHealthProfileRecord([FromForm] HealthProfileRequest request)
        {
            await _healthprofileService.CreateHealthProfileRecord(request);
            return Ok();
        }

        [HttpDelete("{userid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFood(int userid)
        {
            var result = await _healthprofileService.DeleteHealthProfile(userid);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("reports")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Tracking()
        {
            var result = await _healthprofileService.TrackingHealthProfile();
            return StatusCode(result.StatusCode, result);
        }
    }
}
