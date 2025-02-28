using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using NutriDiet.Service.Services;
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

        [HttpGet]
        public async Task<IActionResult> GetAllMealPlanDetail()
        {
            var result = await _mealPlanDetailService.GetAllMealPlanDetail();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMealPlanDetail(int mealPlanId, [FromBody] MealPlanDetailRequest mealPlanDetailRequest)
        {
            var mealPlan = await _mealPlanDetailService.CreateMealPlanDetail(mealPlanId, mealPlanDetailRequest);
            return Ok("Tạo thực đơn thành công");
        }

        [HttpDelete("{mealPlanDetailId}")]
        public async Task<IActionResult> DeleteMealPlanDetail(int mealPlanDetailId)
        {
            await _mealPlanDetailService.DeleteMealPlanDetail(mealPlanDetailId);
            return Ok("Xóa thành công");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMealPlan([Required] List<UpdateMealPlanDetailRequest> mealPlanDetailRequest)
        {
            await _mealPlanDetailService.UpdateMealPlanDetail(mealPlanDetailRequest);
            return Ok("Cập nhật thành công");
        }
    }
}
