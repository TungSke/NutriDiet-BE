using Microsoft.AspNetCore.Http;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class FoodRequest
    {
        public string FoodName { get; set; } = null!;

        public string? MealType { get; set; }

        public IFormFile? FoodImageUrl { get; set; }

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

        public IEnumerable<int>? AllergyId { get; set; }

        public IEnumerable<int>? DiseaseId { get; set; }
    }
}
