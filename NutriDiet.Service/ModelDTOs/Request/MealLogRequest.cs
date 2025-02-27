using NutriDiet.Common.Enums;
using NutriDiet.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class MealLogRequest
    {
        public DateTime? LogDate { get; set; }

        public string? ServingSize { get; set; }

        public MealType MealType { get; set; }

        public int FoodId { get; set; }
        public double? Quantity { get; set; }

    }
}
