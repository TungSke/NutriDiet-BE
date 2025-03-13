using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    }
}
