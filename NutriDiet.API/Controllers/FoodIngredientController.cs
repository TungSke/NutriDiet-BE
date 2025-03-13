using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;

namespace NutriDiet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodIngredientController : ControllerBase
    {
        private readonly IFoodIngredientService _foodIngredientService;
        public FoodIngredientController(IFoodIngredientService foodIngredientService)
        {
            _foodIngredientService = foodIngredientService;
        }

        [HttpPost("{foodId}")]
        public async Task<IActionResult> AddFoodIngredientAsync(int foodId, List<FoodIngredientRequest> foodIngredient)
        {
            var result = await _foodIngredientService.AddFoodIngredientAsync(foodId, foodIngredient);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{foodId}")]
        public async Task<IActionResult> UpdateFoodIngredientAsync(int foodId, List<FoodIngredientRequest> foodIngredients)
        {
            var result = await _foodIngredientService.UpdateFoodIngredientAsync(foodId, foodIngredients);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{foodId}")]
        public async Task<IActionResult> DeleteFoodIngredientAsync(int foodId, int ingredientId)
        {
            var result = await _foodIngredientService.DeleteFoodIngredientAsync(foodId, ingredientId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
