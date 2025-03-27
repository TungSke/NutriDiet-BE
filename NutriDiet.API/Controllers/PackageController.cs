using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using Swashbuckle.AspNetCore.Annotations;

namespace NutriDiet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpGet("user-package")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserPackages(int pageIndex, int pageSize, string? status, string? search)
        {
            var result = await _packageService.GetUserPackage(pageIndex, pageSize, status, search);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] PackageRequest request)
        {
            var result = await _packageService.CreatePackage(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{packageId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int packageId, [FromBody] PackageRequest request)
        {
            var result = await _packageService.UpdatePackage(packageId, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{packageId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int packageId)
        {
            var result = await _packageService.DeletePackage(packageId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("payment")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Thanh toán gói dịch vụ",
            Description = "API cho phép người dùng thanh toán một gói dịch vụ dựa trên PackageId. Cung cấp `cancelUrl` để huỷ thanh toán và `returnUrl` để xử lý khi thanh toán thành công."
        )]
        public async Task<IActionResult> PayforPackage(string cancelUrl, string returnUrl, int packageId)
        {
            var result = await _packageService.PayforPackage(cancelUrl, returnUrl, packageId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("payos-callback")]
        public async Task<IActionResult> PAYOSCallback(string status)
        {
            var result = await _packageService.PAYOSCallback(status);
            return StatusCode(result.StatusCode, result);
        }
    }
}