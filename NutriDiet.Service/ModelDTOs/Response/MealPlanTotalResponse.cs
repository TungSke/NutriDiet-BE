using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Response
{
    public class MealPlanTotalResponse
    {
        public List<TotalByMealType> TotalByMealType { get; set; } = new();
        public List<TotalByDayNumber> TotalByDayNumber { get; set; } = new();
    }

    public class TotalByMealType
    {
        public int DayNumber { get; set; }
        public string MealType { get; set; }
        public double? TotalCalories { get; set; }
        public double? TotalCarbs { get; set; }
        public double? TotalFat { get; set; }
        public double? TotalProtein { get; set; }
    }

    public class TotalByDayNumber
    {
        public int DayNumber { get; set; }
        public double? TotalCalories { get; set; }
        public double? TotalCarbs { get; set; }
        public double? TotalFat { get; set; }
        public double? TotalProtein { get; set; }
    }
}
