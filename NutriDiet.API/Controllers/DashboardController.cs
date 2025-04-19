using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.Service.Enums;
using NutriDiet.Service.Interface;
using NutriDiet.Service.Services;

namespace NutriDiet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        [HttpGet]
        [Authorize(Roles = $"{nameof(RoleEnum.Admin)},{nameof(RoleEnum.Nutritionist)}")]
        public async Task<IActionResult> Dashboard()
        {
            var result = await _dashboardService.Dashboard();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("revenue")]
        [Authorize(Roles = $"{nameof(RoleEnum.Admin)},{nameof(RoleEnum.Nutritionist)}")]
        public async Task<IActionResult> Revenue()
        {
            var result = await _dashboardService.Revenue();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("transaction")]
        [Authorize(Roles = $"{nameof(RoleEnum.Admin)},{nameof(RoleEnum.Nutritionist)}")]
        public async Task<IActionResult> Transaction(int pageIndex, int pageSize, string? search)
        {
            var result = await _dashboardService.Transaction(pageIndex, pageSize, search);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("goal")]
        [Authorize(Roles = $"{nameof(RoleEnum.Admin)},{nameof(RoleEnum.Nutritionist)}")]
        public async Task<IActionResult> GoalChart()
        {
            var result = await _dashboardService.GetGoalProgressChartData();
            return Ok(result);
        }
        [HttpGet("top-food")]
        [Authorize(Roles = $"{nameof(RoleEnum.Admin)},{nameof(RoleEnum.Nutritionist)}")]
        public async Task<IActionResult> TopFood(int top)
        {
            var result = await _dashboardService.GetTopSelectedFoods(top);
            return Ok(result);
        }
    }
}
