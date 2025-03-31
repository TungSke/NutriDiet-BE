using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.Service.Utilities;

namespace NutriDiet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BarcodeController : ControllerBase
    {
        private readonly BarcodeHelper _barcodeHelper;

        public BarcodeController(BarcodeHelper barcodeHelper)
        {
            _barcodeHelper = barcodeHelper;
        }

        [HttpGet("{barcode}")]
        public async Task<IActionResult> GetProductFromBarcode(string barcode)
        {
            try
            {
                var food = await _barcodeHelper.GetProductFromBarcodeAsync(barcode);
                return Ok(food);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
