using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using NutriDiet.Service.ModelDTOs.Response;
using System.Threading.Tasks;

namespace NutriDiet.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PersonalGoalController : ControllerBase
    {
        private readonly IPersonalGoalService _personalGoalService;

        public PersonalGoalController(IPersonalGoalService personalGoalService)
        {
            _personalGoalService = personalGoalService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPersonalGoal()
        {
            var result = await _personalGoalService.GetPersonalGoal();
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePersonalGoal([FromForm] PersonalGoalRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //await _personalGoalService.CreatePersonalGoal(request);
            return StatusCode(201); // HTTP 201 for resource creation
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePersonalGoal([FromForm] PersonalGoalRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _personalGoalService.UpdatePersonalGoal(request);
            return StatusCode(result.StatusCode, result);
        }
    }
}
