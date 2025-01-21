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
            //var result = await _foodService.GetFoodById(foodId);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateFood([FromForm] FoodRequest request)
        {
            await _foodService.CreateFood(request);
            return Ok();
        }

        [HttpPut("{foodId}")]
        public async Task<IActionResult> UpdateFood(int foodId, [FromForm] FoodRequest request)
        {
            await _foodService.UpdateFood(foodId, request);
            return Ok();
        }
    }
}
