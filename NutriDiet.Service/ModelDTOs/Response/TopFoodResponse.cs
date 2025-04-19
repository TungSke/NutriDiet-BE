using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Response
{
    public class TopFoodResponse
    {
        public string FoodName { get; set; }
        public int MealLogCount { get; set; }
        public int MealPlanCount { get; set; }
        public int TotalCount { get; set; } 
    }
}
