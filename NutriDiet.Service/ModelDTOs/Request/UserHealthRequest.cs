using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class UserHealthRequest
    {
        public string FullName { get; set; } = null!;

        public int? Age { get; set; }

        public string? Gender { get; set; }

        public string? Location { get; set; }

        public string? MedicalCondition { get; set; }

        public double? Height { get; set; }

        public double? Weight { get; set; }

        public string? ActivityLevel { get; set; }

        public string? HealthGoal { get; set; }

        public double? TargetWeight { get; set; }

        public DateTime? DurationTarget { get; set; }

    }
}
