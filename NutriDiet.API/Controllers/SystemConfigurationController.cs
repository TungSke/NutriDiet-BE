using Microsoft.AspNetCore.Mvc;
using NutriDiet.Common.Enums;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;

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
        public async Task<IActionResult> GetSystemConfig(int pageIndex = 1, int pageSize = 10, SystemConfigEnum? search = null)
        {
            var result = await _systemConfigurationService.GetSystemConfig(pageIndex, pageSize, search.ToString());
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSystemConfig(SystemConfigurationRequest request)
        {
            var result = await _systemConfigurationService.CreateSystemConfig(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{configId}")]
        public async Task<IActionResult> UpdateSystemConfig(int configId, SystemConfigurationRequest request)
        {
            var result = await _systemConfigurationService.UpdateSystemConfig(configId, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{configId}")]
        public async Task<IActionResult> DeleteSystemConfig(int configId)
        {
            var result = await _systemConfigurationService.DeleteSystemConfig(configId);
            return StatusCode(result.StatusCode, result);
        }


    }
}
