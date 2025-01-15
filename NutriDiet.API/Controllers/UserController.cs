using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.Service.Utilities;
using Sprache;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.Facebook;
using Newtonsoft.Json.Linq;

namespace NutriDiet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _userService.Register(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("verify-account")]
        public async Task<IActionResult> VerifyAccount(VerifyAccountRequest request)
        {
            var result = await _userService.VerifyAccount(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOTP(ResendOtpRequest request)
        {
            var result = await _userService.ResendOTP(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _userService.Login(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("login-with-google")]
        public async Task<IActionResult> LoginWithGoogle(string idToken)
        {
            var result = await _userService.LoginWithGoogle(idToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("login-with-facebook")]
        public async Task<IActionResult> FacebookLogin([FromBody] string accessToken)
        {
            try
            {
                // Kiểm tra token với Facebook
                var urlConnect = $"https://graph.facebook.com/v21.0/me?fields=id,name,email,phone&access_token={accessToken}";
                var userAvatar = $"https://graph.facebook.com/v21.0/me/picture?type=large&access_token={accessToken}"; // Dùng link này là xem luôn dc avatar 
                var response = await _httpClient.GetAsync(urlConnect);
                var response2 = await _httpClient.GetAsync(userAvatar);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return BadRequest($"Invalid Facebook access token. Details: {errorContent}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var userData = JsonConvert.DeserializeObject<JObject>(content);

                // Kiểm tra sự tồn tại của các trường trước khi truy cập
                var name = userData?["name"]?.ToString();
                var email = userData?["email"]?.ToString();

                if (name == null || email == null)
                {
                    return BadRequest("Name or email not found in Facebook response.");
                }

                // Trả về kết quả
                return Ok(new
                {
                    name,
                    email
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}
