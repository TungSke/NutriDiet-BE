using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NutriDiet.Common.Enums;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class MealPlanManualInputRequest
    {
        public Gender Gender { get; set; }
        public int Age { get; set; }
        public float Height { get; set; }
        public float Weight { get; set; }
        public ActivityLevel ActivityLevel { get; set; }
        public GoalType HealthGoal { get; set; } 
        public string Allergies { get; set; } 
        public string Diseases { get; set; } 
        public DietStyleEnum DietStyle { get; set; } 
        [Required]
        [Range(500, 10000, ErrorMessage = "Calories phải nằm trong khoảng từ 500 đến 10000")]
        public int DailyCalories { get; set; }

        [Required]
        [Range(0, 1000, ErrorMessage = "Carb phải là số dương hợp lệ")]
        public int DailyCarb { get; set; }

        [Required]
        [Range(0, 1000, ErrorMessage = "Fat phải là số dương hợp lệ")]
        public int DailyFat { get; set; }

        [Required]
        [Range(0, 1000, ErrorMessage = "Protein phải là số dương hợp lệ")]
        public int DailyProtein { get; set; }
    }

}
