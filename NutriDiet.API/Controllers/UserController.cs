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
        public async Task<IActionResult> LoginWithFacebook(string idToken)
        {
            var result = await _userService.LoginWithFacebook(idToken);
            return StatusCode(result.StatusCode, result);
        }

    }
}
