using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Response
{
    public class PersonalGoalResponse
    {
        public string GoalType { get; set; } = null!;
        public double? TargetWeight { get; set; }

        public string GoalDescription { get; set; } = null!;

        public DateTime? StartDate { get; set; }

        public DateTime TargetDate { get; set; }

        public int? ProgressRate { get; set; }

        public string? Status { get; set; }

        public double? ProgressPercentage { get; set; }

        public string? Notes { get; set; }

        public double? DailyCalories { get; set; }

        public double? DailyCarb { get; set; }

        public double? DailyFat { get; set; }

        public double? DailyProtein { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
