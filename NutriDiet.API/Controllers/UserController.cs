using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            return Ok(result);
        }

        [HttpPost("verify-account")]
        public async Task<IActionResult> VerifyAccount(VerifyAccountRequest request)
        {
            var result = await _userService.VerifyAccount(request);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await _userService.Login(request);
            return Ok(response);
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("Test");
        }
    }
}
