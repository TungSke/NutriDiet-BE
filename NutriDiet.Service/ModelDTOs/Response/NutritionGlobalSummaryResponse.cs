using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Response
{
    public class NutritionGlobalSummaryResponse
    {
        public double TotalCalories { get; set; }
        public double TotalCarbs { get; set; }
        public double TotalProtein { get; set; }
        public double TotalFat { get; set; }
        public double CarbsPercentage { get; set; }
        public double ProteinPercentage { get; set; }
        public double FatPercentage { get; set; }
    }

}
