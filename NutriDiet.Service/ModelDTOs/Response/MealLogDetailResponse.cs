using System.Text.Json.Serialization;

namespace NutriDiet.Service.ModelDTOs.Response
{
    public class MealLogDetailResponse
    {
        [JsonPropertyName("detailId")]
        public int DetailId { get; set; }

        [JsonPropertyName("foodName")]
        public string FoodName { get; set; } = string.Empty;

        [JsonPropertyName("mealType")]
        public string? MealType { get; set; }

        [JsonPropertyName("servingSize")]
        public string? ServingSize { get; set; }

        [JsonPropertyName("quantity")]
        public double? Quantity { get; set; }

        [JsonPropertyName("calories")]
        public double? Calories { get; set; }

        [JsonPropertyName("protein")]
        public double? Protein { get; set; }

        [JsonPropertyName("carbs")]
        public double? Carbs { get; set; }

        [JsonPropertyName("fat")]
        public double? Fat { get; set; }

        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }
    }
}
