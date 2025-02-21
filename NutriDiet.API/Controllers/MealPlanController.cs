using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.Common.Enums;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using System.ComponentModel.DataAnnotations;

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
        public async Task<IActionResult> SearchMealPlan(int pageIndex, int pageSize, string? status, string? search)
        {
            var mealPlans = await _mealPlanService.SearchMealPlan(pageIndex, pageSize, status, search);
            return Ok(mealPlans);
        }

        [HttpGet("{mealplanId}")]
        public async Task<IActionResult> GetMealPlanByID(int mealplanId)
        {
            var mealPlan = await _mealPlanService.GetMealPlanByID(mealplanId);
            return Ok(mealPlan);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateMealPlan([FromBody] MealPlanRequest mealPlanRequest)
        {
            var mealPlan = await _mealPlanService.CreateMealPlan(mealPlanRequest);
            return Ok("Tạo thực đơn thành công");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMealPlan(int id)
        {
            await _mealPlanService.DeleteMealPlan(id);
            return Ok("Xóa thành công");
        }

        [HttpPut()]
        [Authorize]
        public async Task<IActionResult> UpdateMealPlan(int id, [Required] UpdateMealPlanRequest mealPlanRequest)
        {
            await _mealPlanService.UpdateMealPlan(id, mealPlanRequest);
            return Ok("Cập nhật thực đơn thành công");
        }

        [HttpPut("status")]
        [Authorize]
        public async Task<IActionResult> ChangStatusMealPlan(int id,[Required] MealplanStatus status)
        {
            await _mealPlanService.ChangStatusMealPlan(id, status.ToString());
            return Ok("Cập nhật trạng thái thành công");
        }

        //[HttpGet("meal-plan-detail")]
        //public async Task<IActionResult> GetMealPlanDetailByMealPlanID(int mealPlanID) 
        //{
        //    var mealPlanDetail = await _mealPlanService.GetMealPlanDetailByMealPlanID(mealPlanID);
        //    return Ok(mealPlanDetail);
        //}

        [HttpPost("clone")]
        [Authorize]
        public async Task<IActionResult> CloneSampleMealPlan(int mealPlanID)
        {
            var mealPlan = await _mealPlanService.CloneSampleMealPlan(mealPlanID);
            return Ok("Clone thực đơn thành công");
        }

        [HttpPost("create-suitable-meal-plan")]
        //[Authorize]
        public async Task<IActionResult> CreateSuitableMealPlanByAI()
        {
            var mealPlan = await _mealPlanService.CreateSuitableMealPlanByAI();
            return Ok(mealPlan);

        }
    }
}

