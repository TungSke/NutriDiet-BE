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
        public double? Height { get; set; }
        public double? Weight { get; set; }

        public DietStyleEnum? DietStyle { get; set; }
        public ActivityLevel? ActivityLevel { get; set; }
        public ProfileOption ProfileOption { get; set; } = ProfileOption.ADD;

        public List<int> AllergyIds { get; set; } = new();
        public List<int> DiseaseIds { get; set; } = new();
    }
}
