using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;

namespace NutriDiet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly IFoodService _foodService;
        public FoodController(IFoodService foodService)
        {
            _foodService = foodService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFood(int pageindex = 1, int pagesize = 10, string foodType = "", string search = "")
        {
            var result = await _foodService.GetAllFood(pageindex, pagesize, foodType, search);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{foodId}")]
        public async Task<IActionResult> GetFoodById(int foodId)
        {
            var result = await _foodService.GetFoodById(foodId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFood([FromForm] FoodRequest request)
        {
            var result = await _foodService.CreateFood(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("insert-incredient")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> InsertIngredient([FromBody] InsertIngredientRequest request)
        {
            var result = await _foodService.InsertIngredient(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFood([FromForm] UpdateFoodRequest request)
        {
            var result = await _foodService.UpdateFood(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("update-incredient")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateIngredient([FromBody] UpdateIngredientRequest request)
        {
            var result = await _foodService.UpdateIngredient(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("delete-incredient/{ingredientId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteIngredient(int ingredientId)
        {
            var result = await _foodService.DeleteIngredient(ingredientId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("get-incredient/{ingredientId}")]
        public async Task<IActionResult> GetIngredientById(int ingredientId)
        {
            var result = await _foodService.GetIngredientById(ingredientId);
            return StatusCode(result.StatusCode, result);
        }


    }
}
