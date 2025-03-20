using NutriDiet.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class AddMultipleDaysMealLogRequest
    {
        public List<DateTime> Dates { get; set; } = new List<DateTime>();

        public int FoodId { get; set; }
        public double Quantity { get; set; }
        public MealType MealType { get; set; }
    }

}
