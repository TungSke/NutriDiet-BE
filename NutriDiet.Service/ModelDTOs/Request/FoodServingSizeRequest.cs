using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class FoodServingSizeRequest
    {
        public int ServingSizeId { get; set; } // ID của kích thước khẩu phần
        public double Quantity { get; set; } // Số lượng (mặc định 1.0)
        public double Calories { get; set; } // Calo
        public double Protein { get; set; } // Protein
        public double Carbs { get; set; } // Carbs
        public double Fat { get; set; } // Chất béo
        public double Glucid { get; set; } // Glucid
        public double Fiber { get; set; } // Chất xơ
    }
}
