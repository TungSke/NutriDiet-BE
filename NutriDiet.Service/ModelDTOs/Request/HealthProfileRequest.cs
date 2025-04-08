using NutriDiet.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class HealthProfileRequest
    {
        [Range(50, 200,
            ErrorMessage = "Height must be between 50 and 200 cm")]
        public double? Height { get; set; }

        [Range(20, 250,
            ErrorMessage = "Weight must be between 20 and 250 kg")]
        public double? Weight { get; set; }

        public DietStyleEnum? DietStyle { get; set; }
        public ActivityLevel? ActivityLevel { get; set; }

        public string? Aisuggestion { get; set; }

        public List<int> AllergyIds { get; set; } = new();
        public List<int> DiseaseIds { get; set; } = new();
    }
}
