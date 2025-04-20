using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Response
{
    internal class DietStyleDistribution
    {
        public string DietStyle { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
