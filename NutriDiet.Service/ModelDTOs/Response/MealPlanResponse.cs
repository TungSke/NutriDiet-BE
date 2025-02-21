using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Response
{
    public class MealPlanResponse
    {
        public int MealPlanId { get; set; }
        public string PlanName { get; set; } = null!;

        public string? HealthGoal { get; set; }

        public int? Duration { get; set; }

        public string? Status { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
