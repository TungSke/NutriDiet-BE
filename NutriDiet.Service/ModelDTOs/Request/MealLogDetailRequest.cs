using NutriDiet.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class MealLogDetailRequest
    {
        public int MealLogId { get; set; }

        public int FoodId { get; set; }

        public double? Quantity { get; set; }

        public double? Calories { get; set; }

        public virtual Food Food { get; set; } = null!;

        public virtual MealLog MealLog { get; set; } = null!;
    }
}
