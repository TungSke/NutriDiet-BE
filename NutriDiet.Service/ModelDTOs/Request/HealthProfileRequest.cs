using NutriDiet.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class HealthProfileRequest
    {
        public string FullName { get; set; } = null!;
        public string? Avatar { get; set; }
        public int? Age { get; set; }
        public Gender? Gender { get; set; }
        public string? Location { get; set; }
        public string? MedicalCondition { get; set; }
        public double? Height { get; set; }
        public double? Weight { get; set; }

        public ActivityLevel? ActivityLevel { get; set; }

        public string? HealthGoal { get; set; }
        public double? TargetWeight { get; set; }
    }
}
