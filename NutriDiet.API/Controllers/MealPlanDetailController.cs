using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using NutriDiet.Service.Services;
using Sprache;
using System.ComponentModel.DataAnnotations;

namespace NutriDiet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealPlanDetailController : ControllerBase
    {
        private readonly IMealPlanDetailService _mealPlanDetailService;
        public MealPlanDetailController(IMealPlanDetailService mealPlanDetailService)
        {
            _mealPlanDetailService = mealPlanDetailService;
        }

        [HttpGet("{mealPlanId}/{dayNumber}")]
        [Authorize]
        public async Task<IActionResult> GetMealPlanDetailByDayNumber(int mealPlanId, int dayNumber)
        {
            var result = await _mealPlanDetailService.GetMealPlanDetailByDayNumber(mealPlanId, dayNumber);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("meal-plan-detail-total/{mealPlanId}")]
        [Authorize]
        public async Task<IActionResult> GetMealPlanTotals(int mealPlanId)
        {
            var mealPlan = await _mealPlanDetailService.GetMealPlanDetailTotals(mealPlanId);
            return Ok(mealPlan);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateMealPlanDetail(int mealPlanId, [FromBody] MealPlanDetailRequest mealPlanDetailRequest)
        {
            var mealPlan = await _mealPlanDetailService.CreateMealPlanDetail(mealPlanId, mealPlanDetailRequest);
            return Ok("Tạo thực đơn thành công");
        }

        [HttpDelete("{mealPlanDetailId}")]
        [Authorize]
        public async Task<IActionResult> DeleteMealPlanDetail(int mealPlanDetailId)
        {
            await _mealPlanDetailService.DeleteMealPlanDetail(mealPlanDetailId);
            return Ok("Xóa thành công");
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateMealPlan([Required] UpdateMealPlanDetailRequest mealPlanDetailRequest)
        {
            await _mealPlanDetailService.UpdateMealPlanDetail(mealPlanDetailRequest);
            return Ok("Cập nhật thành công");
        }

        [HttpPost("copy/{mealPlanId}")]
        [Authorize]
        public async Task<IActionResult> CopyMealPlanDetail(int mealPlanId, CopyMealPlanDetailRequest request)
        {
            var result = await _mealPlanDetailService.CopyMealPlanDetail(mealPlanId, request);
            return StatusCode(result.StatusCode, result);
        }
    }
}
