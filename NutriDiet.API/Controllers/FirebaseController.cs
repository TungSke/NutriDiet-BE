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
        public async Task<IActionResult> EnableReminder([FromBody] string fcmToken)
        {
            var result = await _firebaseService.EnableReminder(fcmToken);
            return StatusCode(result.StatusCode, result.Message);
        }
    }
}