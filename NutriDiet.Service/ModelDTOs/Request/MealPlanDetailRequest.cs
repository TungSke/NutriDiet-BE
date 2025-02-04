using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class MealPlanDetailRequest
    {
        public int FoodId { get; set; }
        public double? Quantity { get; set; }
        public string? MealType { get; set; }
        public int DayNumber { get; set; }
    }
}
