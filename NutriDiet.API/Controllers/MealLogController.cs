using Microsoft.AspNetCore.Mvc;

namespace NutriDiet.API.Controllers
{
    public class MealLogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
