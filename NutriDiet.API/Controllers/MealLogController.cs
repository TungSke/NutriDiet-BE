using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.Common.Enums;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using NutriDiet.Service.Services;
using System;
using System.Threading.Tasks;

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

        [HttpPost("multiple-days")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddMealToMultipleDays([FromForm] AddMultipleDaysMealLogRequest request)
        {
            var result = await _mealLogService.AddMealToMultipleDays(request);
            return StatusCode(result.StatusCode, result);
        }
        [HttpDelete("{mealLogId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> RemoveMealLog(int mealLogId)
        {
            var result = await _mealLogService.RemoveMealLog(mealLogId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{mealLogId}/detail/{detailId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> RemoveMealLogDetail(int mealLogId, int detailId)
        {
            var result = await _mealLogService.RemoveMealLogDetail(mealLogId, detailId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMealLogsByDateRange([FromQuery] DateTime? logDate, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var result = await _mealLogService.GetMealLogsByDateRange(logDate, fromDate, toDate);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("quick")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> QuickAddMealLogDetail([FromForm] QuickMealLogRequest request)
        {
            var result = await _mealLogService.QuickAddMealLogDetail(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("clone")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CopyMealLogDetails([FromForm] CopyMealLogRequest request)
        {
            var result = await _mealLogService.CopyMealLogDetails(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("meallogai")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MeallogAI()
        {
            var result = await _mealLogService.CreateMealLogAI();
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("savemeallogai")]
        [Authorize]
        public async Task<IActionResult> SaveMeallogAI([FromForm] string? feedback)
        {
            var result = await _mealLogService.SaveMeallogAI(feedback);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("detail/transfer/{detailId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> TransferMealLogDetail(int detailId, [FromQuery] MealType targetMealType)
        {
            var result = await _mealLogService.TransferMealLogDetail(detailId, targetMealType);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("recent-food")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetRecentFood()
        {
            var result = await _mealLogService.GetRecentFoods();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("nutrition")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> NutritionSummary([FromQuery] DateTime date)
        {
            var result = await _mealLogService.GetNutritionSummary(date);
            return StatusCode(result.StatusCode, result);
        }
    }
}
