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

        public int? ServingSizeId { get; set; }

        public string? ServingSizeName { get; set; }

        public List<FoodServingSizeResponse> FoodServingSizes { get; set; } = new List<FoodServingSizeResponse>();

        public List<IngredientResponse>? Ingredients { get; set; }
    }
}
