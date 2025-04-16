using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Response
{
    public class FoodResponse
    {
        [JsonPropertyName("foodId")]
        public int FoodId { get; set; }

        [JsonPropertyName("foodName")]
        public string FoodName { get; set; } = null!;

        [JsonPropertyName("mealType")]
        public string? MealType { get; set; }

        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("foodType")]
        public string? FoodType { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("servingSize")]
        public string? ServingSize { get; set; }

        [JsonPropertyName("calories")]
        public double? Calories { get; set; }

        [JsonPropertyName("protein")]
        public double? Protein { get; set; }

        [JsonPropertyName("carbs")]
        public double? Carbs { get; set; }

        [JsonPropertyName("fat")]
        public double? Fat { get; set; }

        [JsonPropertyName("glucid")]
        public double? Glucid { get; set; }

        [JsonPropertyName("fiber")]
        public double? Fiber { get; set; }

        public List<IngredientResponse>? Ingredients { get; set; }
    }
}
