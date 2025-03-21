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

        [HttpGet("{mealPlanId}")]
        public async Task<IActionResult> GetMealPlanById(int mealPlanId)
        {
            var mealPlan = await _mealPlanService.GetMealPlanByID(mealPlanId);
            return Ok(mealPlan);
        }

        [HttpGet("sample-mealplan")]
        public async Task<IActionResult> GetSampleMealPlan(int pageIndex, int pageSize, string? search)
        {
            var mealPlans = await _mealPlanService.GetSampleMealPlan(pageIndex, pageSize, search);
            return Ok(mealPlans);
        }

        [HttpGet("my-mealplan")]
        [Authorize]
        public async Task<IActionResult> GetMyMealPlan(int pageIndex, int pageSize, string? search)
        {
            var mealPlans = await _mealPlanService.GetMyMealPlan(pageIndex, pageSize, search);
            return Ok(mealPlans);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateMealPlan([FromBody] MealPlanRequest mealPlanRequest)
        {
            var result = await _mealPlanService.CreateMealPlan(mealPlanRequest);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{mealPlanId}")]
        [Authorize]
        public async Task<IActionResult> DeleteMealPlan(int mealPlanId)
        {
            await _mealPlanService.DeleteMealPlan(mealPlanId);
            return Ok("Xóa thành công");
        }

        [HttpPut()]
        [Authorize]
        public async Task<IActionResult> UpdateMealPlan(int mealPlanId, [Required] UpdateMealPlanRequest mealPlanRequest)
        {
            await _mealPlanService.UpdateMealPlan(mealPlanId, mealPlanRequest);
            return Ok("Cập nhật thực đơn thành công");
        }

        [HttpPut("status")]
        [Authorize]
        public async Task<IActionResult> ChangStatusMealPlan(int mealPlanId, [Required] MealplanStatus status)
        {
            await _mealPlanService.ChangStatusMealPlan(mealPlanId, status.ToString());
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
        public async Task<IActionResult> CloneSampleMealPlan(int mealPlanId)
        {
            var result = await _mealPlanService.CloneSampleMealPlan(mealPlanId);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPost("suitable-meal-plan-by-AI")]
        [Authorize]
        public async Task<IActionResult> CreateSuitableMealPlanByAI()
        {
            var result = await _mealPlanService.CreateSuitableMealPlanByAI();
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("reject-mealplan-AI")]
        [Authorize]
        public async Task<IActionResult> RejectMealplan([FromForm][Required] string rejectReason)
        {
            var result = await _mealPlanService.RejectMealplan(rejectReason);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("save-mealplan-AI")]
        [Authorize]
        public async Task<IActionResult> SaveMealPlanAI([FromForm]string? feedback)
        {
            var result = await _mealPlanService.SaveMealPlanAI(feedback);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("apply-mealplan/{mealPlanId}")]
        [Authorize]
        public async Task<IActionResult> ApplyMealPlan(int mealPlanId)
        {
            var result = await _mealPlanService.ApplyMealPlan(mealPlanId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("my-current-mealplan")]
        [Authorize]
        public async Task<IActionResult> GetMyCurrentMealPlan()
        {
            var result = await _mealPlanService.GetMyCurrentMealPlan();
            return StatusCode(result.StatusCode, result);
        }
    }
}

