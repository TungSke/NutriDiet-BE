using NutriDiet.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class QuickMealLogRequest
    {
        public DateTime? LogDate { get; set; }
        public MealType MealType { get; set; }
        public double? Calories { get; set; }
        public double? Carbohydrates { get; set; }
        public double? Fats { get; set; }
        public double? Protein { get; set; }
    }

}
