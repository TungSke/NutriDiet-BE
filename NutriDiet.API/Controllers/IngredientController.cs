using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.Common.Enums;
using NutriDiet.Service.Enums;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using Sprache;

namespace NutriDiet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientController : ControllerBase
    {
        private readonly IIngreDientService _ingredientService;
        public IngredientController(IIngreDientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetIngredients(int pageIndex, int pageSize, string? search)
        {
            var result = await _ingredientService.GetIngreDients(pageIndex, pageSize, search);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{ingredientId}")]
        public async Task<IActionResult> GetIngredientById(int ingredientId)
        {
            var result = await _ingredientService.GetIngredientById(ingredientId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public async Task<IActionResult> AddIngredient(IngredientRequest request)
        {
            var result = await _ingredientService.AddIngredient(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{ingredientId}")]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public async Task<IActionResult> UpdateIngredient(IngredientRequest request, int ingredientId)
        {
            var result = await _ingredientService.UpdateIngredient(ingredientId, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{ingredientId}")]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public async Task<IActionResult> DeleteIngredient(int ingredientId)
        {
            var result = await _ingredientService.DeleteIngredient(ingredientId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{ingredientId}/preference")]
        [Authorize]
        public async Task<IActionResult> PreferenceIngredient(int ingredientId, PreferenceLevel preferenceLevel)
        {
            var result = await _ingredientService.UpdatePreferenceIngredient(ingredientId, (int)preferenceLevel);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("preference")]
        [Authorize]
        public async Task<IActionResult> GetPreferenceIngredient()
        {
            var result = await _ingredientService.GetPreferenceIngredient();
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("excel-analyze")]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public async Task<IActionResult> AnalyzeIngredientImport(IFormFile excelFile)
        {
            var result = await _ingredientService.AnalyzeIngredientImport(excelFile);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("excel")]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public async Task<IActionResult> ImportIngredientsFromExcel(IFormFile excelFile)
        {
            var result = await _ingredientService.ImportIngredientsFromExcel(excelFile);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("excel-duplicate")]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public async Task<IActionResult> ImportAndUpdateIngredientsFromExcel(IFormFile excelFile)
        {
            var result = await _ingredientService.ImportAndUpdateIngredientsFromExcel(excelFile);
            return StatusCode(result.StatusCode, result);
        }

    }
}
