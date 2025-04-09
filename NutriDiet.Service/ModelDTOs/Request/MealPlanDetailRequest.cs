using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class MealPlanDetailRequest
    {
        [JsonPropertyName("foodId")]
        public int FoodId { get; set; }

        [JsonPropertyName("quantity")]
        public double? Quantity { get; set; }

        [JsonPropertyName("mealType")]
        public string? MealType { get; set; }

        [JsonPropertyName("dayNumber")]
        public int DayNumber { get; set; }
    }
}
