using Microsoft.AspNetCore.Mvc;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;

namespace NutriDiet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AllergyController : ControllerBase
    {
        private readonly IAllergyService _allergyService;

        public AllergyController(IAllergyService allergyService)
        {
            _allergyService = allergyService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllergy(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string allergyName = null)
        {
            var result = await _allergyService.GetAllAllergy(pageIndex, pageSize, allergyName);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("{allergyId}")]
        public async Task<IActionResult> GetAllergyById(int allergyId)
        {
            var result = await _allergyService.GetAllergyById(allergyId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAllergy([FromForm] AllergyRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _allergyService.CreateAllergy(request);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAllergy([FromForm] AllergyRequest request, int allergyId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _allergyService.UpdateAllergy(request, allergyId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpDelete("{allergyId}")]
        public async Task<IActionResult> DeleteAllergy(int allergyId)
        {
            var result = await _allergyService.DeleteAllergy(allergyId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
