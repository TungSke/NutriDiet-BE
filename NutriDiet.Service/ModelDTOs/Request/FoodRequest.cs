using Microsoft.AspNetCore.Http;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class FoodRequest
    {
        public string FoodName { get; set; } = null!;

        public string? MealType { get; set; }

        public string? FoodType { get; set; }

        public string? Description { get; set; }
        public int? ServingSizeId { get; set; } // ID của kích thước khẩu phần mặc định
        public List<FoodServingSizeRequest>? FoodServingSizes { get; set; } // Danh sách kích thước khẩu phần
        public List<int>? Ingredients { get; set; } // Danh sách ID của nguyên liệu
    }
}
