using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class MealPlanDetailRequest
    {
        public int FoodId { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public double? Quantity { get; set; }
        public string? MealType { get; set; }
        public int DayNumber { get; set; }
    }
}
