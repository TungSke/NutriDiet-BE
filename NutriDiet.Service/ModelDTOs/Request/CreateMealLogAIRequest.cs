using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NutriDiet.Common.Enums;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class CreateMealLogAIRequest
    {
        public string FullName { get; set; }
        public Gender Gender { get; set; } 
        public int Age { get; set; }
        public double Height { get; set; } 
        public double Weight { get; set; } 
        public ActivityLevel ActivityLevel { get; set; }
        public DietStyleEnum DietStyle { get; set; }
        public GoalType GoalType { get; set; }
        [Range(500, 10000, ErrorMessage = "Calories phải nằm trong khoảng từ 500 đến 10000")]
        public double DailyCalories { get; set; } = 2000;
    }

}
