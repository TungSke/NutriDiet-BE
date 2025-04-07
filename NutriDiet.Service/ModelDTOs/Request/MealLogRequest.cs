using NutriDiet.Common.Enums;
using NutriDiet.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class MealLogRequest
    {
        [JsonPropertyName("logDate")]
        public DateTime? LogDate { get; set; }

        [JsonPropertyName("mealType")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MealType MealType { get; set; }

        [JsonPropertyName("servingSize")]
        public string? ServingSize { get; set; }

        [JsonPropertyName("foodId")]
        public int? FoodId { get; set; }

        [JsonPropertyName("quantity")]
        public double? Quantity { get; set; }
    }
}
