using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.API.Extensions;
using NutriDiet.Common.Enums;
using NutriDiet.Service.Enums;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using NutriDiet.Service.Services;
using Sprache;
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

        [HttpPost("AI-warning/{mealPlanId}")]
        [Authorize]
        public async Task<IActionResult> CreateAIWarning(int mealPlanId)
        {
            var result = await _mealPlanService.CreateAIWarning(mealPlanId);
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

        [HttpPut("mobile")]
        [Authorize]
        public async Task<IActionResult> UpdateMealPlanMobile(int mealPlanId, string planName, string healthGoal)
        {
            var result = await _mealPlanService.UpdateMealPlanMobile(mealPlanId, planName, healthGoal);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("feedback")]
        [Authorize(Roles = $"{nameof(RoleEnum.Admin)},{nameof(RoleEnum.Nutritionist)}")]
        public async Task<IActionResult> GetAllFeedback(int pageIndex = 1, int pageSize = 10, string? search = "")
        {
            var result = await _mealPlanService.GetFeedback(pageIndex, pageSize, search);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("mealplan-ai-simulation")]
        [Authorize(Roles = $"{nameof(RoleEnum.Nutritionist)}")]
        public async Task<IActionResult> CreateMealLogAIMock([FromForm] MealPlanManualInputRequest request)
        {
            var result = await _mealPlanService.CreateMealPlanByManualInput(request);
            return StatusCode(result.StatusCode, result);
        }
    }
}

