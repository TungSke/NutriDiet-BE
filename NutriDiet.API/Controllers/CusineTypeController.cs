using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;

namespace NutriDiet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CusineTypeController : ControllerBase
    {
        private readonly ICuisineTypeService _cusineTypeService;

        public CusineTypeController(ICuisineTypeService cusineTypeService)
        {
            _cusineTypeService = cusineTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCusineType()
        {
            var result = await _cusineTypeService.GetAllCuisineTypes();
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCusineType([FromBody] CuisineTypeRequest cusineTypeRequest)
        {
            var result = await _cusineTypeService.CreateCuisineType(cusineTypeRequest);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCusineType(int id)
        {
            var result = await _cusineTypeService.DeleteCuisineType(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
