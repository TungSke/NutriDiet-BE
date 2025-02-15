using NutriDiet.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Response
{
    public class HealthProfileResponse
    {
        public string FullName { get; set; } = null!;
        public string? Avatar { get; set; }
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
        public virtual ICollection<AllergyResponse> Allergies { get; set; } = new List<AllergyResponse>();
        public virtual ICollection<DiseaseResponse> Diseases { get; set; } = new List<DiseaseResponse>();
    }

}
