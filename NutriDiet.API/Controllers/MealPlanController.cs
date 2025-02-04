using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;

namespace NutriDiet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealPlanController : ControllerBase
    {
        private readonly IMealPlanService _mealPlanService;
        public MealPlanController(IMealPlanService mealPlanService)
        {
            _mealPlanService = mealPlanService;
        }
        [HttpGet]
        public async Task<IActionResult> SearchMealPlan(string? planName, string? healthGoal, int? userID)
        {
            var mealPlans = await _mealPlanService.SearchMealPlan(planName, healthGoal, userID);
            return Ok(mealPlans);
        }
        [HttpPost]
        public async Task<IActionResult> CreateMealPlan([FromBody] MealPlanRequest mealPlanRequest)
        {
            var mealPlan = await _mealPlanService.CreateMealPlan(mealPlanRequest);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteMealPlan(int id)
        {
            await _mealPlanService.DeleteMealPlan(id);
            return Ok("Xóa thành công");
        }
        [HttpPut("change-status")]
        public async Task<IActionResult> ChangStatusMealPlan(int id, string status)
        {
            await _mealPlanService.ChangStatusMealPlan(id, status);
            return Ok("Cập nhật trạng thái thành công");
        }
    }
}
