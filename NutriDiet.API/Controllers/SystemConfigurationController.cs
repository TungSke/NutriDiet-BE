using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.Common.Enums;
using NutriDiet.Service.Enums;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using System.ComponentModel.DataAnnotations;

namespace NutriDiet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemConfigurationController : ControllerBase
    {
        private readonly ISystemConfigurationService _systemConfigurationService;
        public SystemConfigurationController(ISystemConfigurationService systemConfigurationService)
        {
            _systemConfigurationService=systemConfigurationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSystemConfig(int pageIndex = 1, int pageSize = 10, string? search = null)
        {
            var result = await _systemConfigurationService.GetSystemConfig(pageIndex, pageSize, search);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{configId}")]
        public async Task<IActionResult> GetSystemConfigById([Required] int configId)
        {
            var result = await _systemConfigurationService.GetSystemConfigById(configId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public async Task<IActionResult> CreateSystemConfig(SystemConfigurationRequest request)
        {
            var result = await _systemConfigurationService.CreateSystemConfig(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{configId}")]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public async Task<IActionResult> UpdateSystemConfig(int configId, SystemConfigurationRequest request)
        {
            var result = await _systemConfigurationService.UpdateSystemConfig(configId, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{configId}")]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public async Task<IActionResult> DeleteSystemConfig(int configId)
        {
            var result = await _systemConfigurationService.DeleteSystemConfig(configId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
