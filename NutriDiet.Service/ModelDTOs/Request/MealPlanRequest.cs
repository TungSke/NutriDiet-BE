using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class MealPlanRequest
    {
        [JsonPropertyName("planName")]
        public string PlanName { get; set; }

        [JsonPropertyName("healthGoal")]
        public string HealthGoal { get; set; }

        [JsonPropertyName("mealPlanDetails")]
        public List<MealPlanDetailRequest> MealPlanDetails { get; set; }
    }
}
