using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using NutriDiet.Service.Services;

namespace NutriDiet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealLogController : Controller
    {
        private readonly IMealLogService _mealLogService;

        public MealLogController(IMealLogService mealLogService)
        {
            _mealLogService = mealLogService;
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddMealLog([FromForm] MealLogRequest request)
        {
            var result = await _mealLogService.AddOrUpdateMealLog(request);
            return StatusCode(result.StatusCode, result);
        
        }
        [HttpDelete("{mealLogId}/detail/{detailId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> RemoveMealLogDetail(int mealLogId, int detailId)
        {
            var result = await _mealLogService.RemoveMealLogDetail(mealLogId, detailId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("{mealLogId}")]
        public async Task<IActionResult> GetMealLogById(int mealLogId)
        {
            var result = await _mealLogService.GetMealLogById(mealLogId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMealLogsByDateRange([FromQuery] DateTime? logDate, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var result = await _mealLogService.GetMealLogsByDateRange(logDate, fromDate, toDate);
            return StatusCode(result.StatusCode, result);
        }
    }
}
