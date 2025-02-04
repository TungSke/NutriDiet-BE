using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class MealPlanRequest
    {
        public int UserId { get; set; }
        public string PlanName { get; set; } = null!;
        public string? HealthGoal { get; set; }
        public List<MealPlanDetailRequest> MealPlanDetails { get; set; } = new();
    }
}
