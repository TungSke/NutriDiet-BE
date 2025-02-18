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
using Microsoft.AspNetCore.Authorization;

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
        [HttpGet()]
        public async Task<IActionResult> GetAll(int pageIndex, int pageSize, string? status, string? search)
        {
            var user = await _userService.SearchUser(pageIndex, pageSize, status, search);
            return Ok(user);
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
        public async Task<IActionResult> LoginWithFacebook(string idToken)
        {
            var result = await _userService.LoginWithFacebook(idToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("whoami")]
        [Authorize]
        public async Task<IActionResult> WhoAmI()
        {
            var user = await _userService.findUserById(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));
            if (user != null)
            {
                return Ok(new
                {
                    Id = user.UserId,
                    Email = user.Email ?? "noemail@example.com",
                    Role = user.Role?.RoleName,
                    name = user.FullName ?? "Anonymous",
                    gender = user.Gender ?? "not specified",
                    age = user.Age.ToString(),
                    phoneNumber = user.Phone ?? "0123456789",
                    address = user.Location ?? "Vietnam"
                });
            }
            return Unauthorized();
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var result = await _userService.ForgotPassword(email);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var result = await _userService.ResetPassword(request);
            return StatusCode(result.StatusCode, result);
        }

    }
}
