using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;

namespace NutriDiet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;

        public IngredientsController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetIngredients(int pageIndex = 1, int pageSize = 10, string search = "")
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> InsertIngredient([FromBody] InsertIngredientRequest request)
        {
            var result = await _ingredientService.InsertIngredient(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateIngredient([FromBody] UpdateIngredientRequest request)
        {
            var result = await _ingredientService.UpdateIngredient(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{ingredientId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteIngredient(int ingredientId)
        {
            var result = await _ingredientService.DeleteIngredient(ingredientId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
