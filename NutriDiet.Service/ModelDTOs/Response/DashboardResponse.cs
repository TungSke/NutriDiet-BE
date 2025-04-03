using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Response
{
    public class DashboardResponse
    {
        public int TotalUser { get; set; }
        public int TotalPremiumUser { get; set; }
        public int TotalPackage { get; set; }
        public int TotalAllergy { get; set; }
        public int TotalDisease { get; set; }
        public int TotalIngredient { get; set; }
        public int TotalFood { get; set; }
        public int TotalMealPlan { get; set; }
        public int TotalFeedbackAI { get; set; }
        public object Revenue { get; set; }
    }
}
