using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class AllergyRequest
    {
        public string AllergyName { get; set; } = null!;

        public string? Notes { get; set; }

        public List<int> ingredientIds { get; set; } = new List<int>();

    }
}
