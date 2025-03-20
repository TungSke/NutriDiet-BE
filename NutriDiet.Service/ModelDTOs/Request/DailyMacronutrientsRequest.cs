using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class EditDailyMacronutrientsRequest
    {
        public double DailyCarb { get; set; }
        public double DailyProtein { get; set; }
        public double DailyFat { get; set; }
    }
}
