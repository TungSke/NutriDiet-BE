using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.Service.Interface;

namespace NutriDiet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServingSizeController : ControllerBase
    {
        private readonly IServingSizeService _servingSizeService;
        public ServingSizeController(IServingSizeService servingSizeService)
        {
            _servingSizeService = servingSizeService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllServingSizes()
        {
            var servingSizes = await _servingSizeService.GetAllServingSize();
            return Ok(servingSizes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServingSizeById(int id)
        {
            var servingSize = await _servingSizeService.GetServingSizeById(id);
            if (servingSize == null)
            {
                return NotFound();
            }
            return Ok(servingSize);
        }
    }
}
