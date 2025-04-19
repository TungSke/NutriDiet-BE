using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Response
{
    public class ActivityLevelDistribution
    {
        public string ActivityLevel { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
