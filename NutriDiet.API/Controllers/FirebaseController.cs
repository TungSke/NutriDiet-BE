using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.Repository.Models;
using NutriDiet.Service.Utilities;

namespace NutriDiet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FirebaseController : ControllerBase
    {
        private readonly FirebaseService _firebaseService;
        public FirebaseController(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        [HttpPost("send-notification")]
        public async Task<IActionResult> SendNotification([FromBody] string fcmToken, string? title, string body)
        {
            await _firebaseService.SendNotification(fcmToken, title, body);
            return Ok();
        }

        [HttpPost("enable-reminder")]
        public async Task<IActionResult> EnableReminder(string mealType, [FromBody] string fcmToken)
        {
            try
            {
                var result = await _firebaseService.EnableReminder(mealType, fcmToken);
                if (result.StatusCode != 200)
                {
                    return StatusCode(result.StatusCode, result.Message);
                }
                return Ok(new { Message = result.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}