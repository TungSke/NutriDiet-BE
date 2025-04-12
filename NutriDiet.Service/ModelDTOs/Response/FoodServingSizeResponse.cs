using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Response
{
    public class FoodServingSizeResponse
    {
        public int ServingSizeId { get; set; }

        public string? ServingSizeName { get; set; }

        public double? Quantity { get; set; }

        public double? Calories { get; set; }

        public double? Protein { get; set; }

        public double? Carbs { get; set; }

        public double? Fat { get; set; }

        public double? Glucid { get; set; }

        public double? Fiber { get; set; }
    }
}
