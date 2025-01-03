using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Response
{
    public class FoodResponse
    {
        public int FoodId { get; set; }

        public string FoodName { get; set; } = null!;

        public string? FoodType { get; set; }

        public string? FoodImageUrl { get; set; }

        public string? Description { get; set; }

        public string? ServingSize { get; set; }
    }
}
