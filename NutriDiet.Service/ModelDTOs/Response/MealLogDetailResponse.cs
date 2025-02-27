using NutriDiet.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Response
{
    public class MealLogDetailResponse
    {
        public string FoodName { get; set; } // ✅ Thêm tên Food
        public string? MealType { get; set; }
        public double? Quantity { get; set; }
        public string? ServingSize { get; set; }
        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }
    }
}
