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
        public int? Age { get; set; }
        public Gender? Gender { get; set; }
        public string? Location { get; set; }
        public double? Height { get; set; }
        public double? Weight { get; set; }

        public ActivityLevel? ActivityLevel { get; set; }

        public string? Aisuggestion { get; set; }

        public List<int> AllergyIds { get; set; } = new();
        public List<int> DiseaseIds { get; set; } = new();
    }
}
