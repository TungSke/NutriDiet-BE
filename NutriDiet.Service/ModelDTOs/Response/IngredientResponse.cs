using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Response
{
    public class IngredientResponse
    {
        public int IngredientId { get; set; }

        public string IngredientName { get; set; } = null!;

        public double? Calories { get; set; }

        public double? Protein { get; set; }

        public double? Carbs { get; set; }

        public double? Fat { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
