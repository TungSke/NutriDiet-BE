using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class UpdateMealPlanRequest
    {
        public string PlanName { get; set; } = null!;
        public string? HealthGoal { get; set; }
        public List<UpdateMealPlanDetailRequest> MealPlanDetails { get; set; } = new();
    }
}
