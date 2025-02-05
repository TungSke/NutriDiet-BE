using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class IngredientRequest
    {
        public string IngredientName { get; set; } = null!;

        public string? Category { get; set; }

        public string Unit { get; set; } = null!;

        public double? Calories { get; set; }
    }
}
