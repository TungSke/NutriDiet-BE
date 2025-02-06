using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Response
{
    public class FoodResponse
    {
        public int FoodId { get; set; }

        public string FoodName { get; set; } = null!;

        public string? MealType { get; set; }

        public string? ImageUrl { get; set; }

        public string? FoodType { get; set; }

        public string? Description { get; set; }

        public string? ServingSize { get; set; }

        public double? Calories { get; set; }

        public double? Protein { get; set; }

        public double? Carbs { get; set; }

        public double? Fat { get; set; }

        public double? Glucid { get; set; }

        public double? Fiber { get; set; }

        public string? Others { get; set; }

        public IEnumerable<IngredientResponse>? Ingredients { get; set; }

    }
}
