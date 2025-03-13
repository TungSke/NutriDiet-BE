using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.Service.Enums;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;

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
            var ingredients = await _ingredientService.GetIngreDients(pageIndex, pageSize, search);
            return Ok(ingredients);
        }

        [HttpGet("{ingredientId}")]
        public async Task<IActionResult> GetIngredientById(int ingredientId)
        {
            var ingredient = await _ingredientService.GetIngredientById(ingredientId);
            return Ok(ingredient);
        }

        [HttpPost]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public async Task<IActionResult> AddIngredient(IngredientRequest request)
        {
            var result = await _ingredientService.AddIngredient(request);
            return Ok(result);
        }

        [HttpPut("{ingredientId}")]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public async Task<IActionResult> UpdateIngredient(IngredientRequest request, int ingredientId)
        {
            var result = await _ingredientService.UpdateIngredient(ingredientId, request);
            return Ok(result);
        }

        [HttpDelete("{ingredientId}")]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public async Task<IActionResult> DeleteIngredient(int ingredientId)
        {
            var result = await _ingredientService.DeleteIngredient(ingredientId);
            return Ok(result);
        }
    }
}
