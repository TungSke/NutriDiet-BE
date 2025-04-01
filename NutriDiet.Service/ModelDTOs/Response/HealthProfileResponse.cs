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
        public int ProfileId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Avatar { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }
        public string? Location { get; set; }
        public double? Height { get; set; }
        public double? Weight { get; set; }
        public string? ActivityLevel { get; set; }
        public string? Aisuggestion { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public virtual ICollection<HealthcareIndicatorResponse> HealthcareIndicators { get; set; } = new List<HealthcareIndicatorResponse>();
        public virtual ICollection<AllergyResponse> Allergies { get; set; } = new List<AllergyResponse>();
        public virtual ICollection<DiseaseResponse> Diseases { get; set; } = new List<DiseaseResponse>();
    }

}
