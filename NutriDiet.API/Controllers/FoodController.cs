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

        [HttpGet("avoidance/{foodId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetAvoidFood(int foodId)
        {
            var result = await _foodService.CheckFoodAvoidance(foodId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFood([FromForm] FoodRequest request)
        {
            var result = await _foodService.CreateFood(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{foodId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFood(int foodId, [FromForm] FoodRequest request)
        {
            var result = await _foodService.UpdateFood(foodId, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{foodId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFood(int foodId)
        {
            var result = await _foodService.DeleteFood(foodId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("recommend")]
        [Authorize]
        public async Task<IActionResult> RecommendFood(int pageIndex = 1, int pageSize = 10, string searchName = "")
        {
            var result = await _foodService.GetFoodRecommend(pageIndex, pageSize, searchName);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("recipe/{foodId}/{cuisineId}")]
        [Authorize]
        public async Task<IActionResult> FoodRecipe(int foodId, int cuisineId)
        {
            var result = await _foodService.CreateFoodRecipeByAI(foodId, cuisineId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("reject-recipe")]
        [Authorize]
        public async Task<IActionResult> RejectRecipe([FromBody] RejectRecipeRequest request)
        {
            var result = await _foodService.RejectRecipe(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("recipe/{foodId}")]
        [Authorize]
        public async Task<IActionResult> GetFoodByType(int foodId)
        {
            var result = await _foodService.GetFoodRecipe(foodId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("user-food-preference")]
        [Authorize]
        public async Task<IActionResult> GetFavoriteFoods(int pageIndex, int pageSize)
        {
            var result = await _foodService.GetFavoriteFoods(pageIndex, pageSize);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("user-food-preference/{foodId}")]
        [Authorize]
        public async Task<IActionResult> AddFavoriteFood(int foodId)
        {
            var result = await _foodService.AddFavoriteFood(foodId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("user-food-preference/{foodId}")]
        [Authorize]
        public async Task<IActionResult> RemoveFavoriteFood(int foodId)
        {
            var result = await _foodService.RemoveFavoriteFood(foodId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
