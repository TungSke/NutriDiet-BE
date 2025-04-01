using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class HealthcareIndicatorRequest
    {
        public int ProfileID { get; set; }
        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Type { get; set; } = null!;

        public double? CurrentValue { get; set; }

        public double? MinValue { get; set; }

        public double? MediumValue { get; set; }

        public double? MaxValue { get; set; }

        public bool? Active { get; set; }

        public string? Aisuggestion { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
