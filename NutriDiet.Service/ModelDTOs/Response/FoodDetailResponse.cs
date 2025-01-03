using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Response
{
    public class FoodDetailResponse
    {
        public int FoodDetailId { get; set; }

        public string? FoodDetailName { get; set; }

        public string Unit { get; set; } = null!;

        public double? Amount { get; set; }

        public string? Description { get; set; }
    }
}
