using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.Common.Enums;
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

        [HttpGet("reports")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Tracking([FromQuery] HealProfileFields field)
        {
            if (string.IsNullOrEmpty(field.ToString()))
            {
                return BadRequest(new { message = "Field parameter is required." });
            }

            var result = await _healthprofileService.TrackingHealthProfile(field);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("ai-suggestion")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateAISuggestion()
        {
            var result = await _healthprofileService.CreateAISuggestion();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("list")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetHealthProfiles()
        {
            var result = await _healthprofileService.GetHealthProfiles();
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{profileId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DeleteProfileById(int profileId)
        {
            var result = await _healthprofileService.DeleteProfileById(profileId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{profileId}/image")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddImageToHealthProfile(int profileId, [FromForm] AddImageRequest request)
        {
            var result = await _healthprofileService.AddImageToHealthProfile(profileId, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{profileId}/image")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DeleteImageFromHealthProfile(int profileId)
        {
            var result = await _healthprofileService.DeleteImageFromHealthProfile(profileId);
            return StatusCode(result.StatusCode, result);
        }

    }
}
