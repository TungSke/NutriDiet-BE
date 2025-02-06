using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class UpdateIngredientRequest
    {
        public int IngredientId { get; set; }

        public string IngredientName { get; set; } = null!;

        public string? Category { get; set; }

        public string Unit { get; set; } = null!;

        public double? Calories { get; set; }
    }
}
