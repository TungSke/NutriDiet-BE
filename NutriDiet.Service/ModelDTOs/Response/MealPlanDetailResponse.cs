using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Response
{
    public class MealPlanDetailResponse
    {
        public int MealPlanDetailId { get; set; }
        public int? FoodId { get; set; }
        public string? FoodName { get; set; }

        public double? Quantity { get; set; }

        public string? MealType { get; set; }

        public int DayNumber { get; set; }

        public double? TotalCalories { get; set; }
        public double? TotalCarbs { get; set; }
        public double? TotalFat { get; set; }
        public double? TotalProtein { get; set; }
    }
}
