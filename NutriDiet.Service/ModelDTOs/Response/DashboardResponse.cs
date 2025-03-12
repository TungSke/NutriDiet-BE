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

        public int MealPlanNumber { get; set; }

        public int PackageNumber { get; set; }
    }
}
