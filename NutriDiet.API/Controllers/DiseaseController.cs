using Microsoft.AspNetCore.Mvc;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using NutriDiet.Service.Services;
using System.Threading.Tasks;

namespace NutriDiet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiseaseController : ControllerBase
    {
        private readonly IDiseaseService _diseaseService;

        public DiseaseController(IDiseaseService diseaseService)
        {
            _diseaseService = diseaseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDisease(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string diseaseName = null)
        {
            var result = await _diseaseService.GetAllDisease(pageIndex, pageSize, diseaseName);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{diseaseId}")]
        public async Task<IActionResult> GetDiseaseById(int diseaseId)
        {
            var result = await _diseaseService.GetDiseaseById(diseaseId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDisease([FromForm] DiseaseRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _diseaseService.CreateDisease(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDisease([FromForm] DiseaseRequest request, int diseaseId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _diseaseService.UpdateDisease(request, diseaseId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{diseaseId}")]
        public async Task<IActionResult> DeleteDisease(int diseaseId)
        {
            var result = await _diseaseService.DeleteDisease(diseaseId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
