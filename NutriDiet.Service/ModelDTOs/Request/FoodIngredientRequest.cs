using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class FoodIngredientRequest
    {
        public int IngredientId { get; set; }

        public double? Quantity { get; set; }

        public string Unit { get; set; } = null!;
    }
}
