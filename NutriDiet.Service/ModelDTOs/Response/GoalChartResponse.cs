using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Response
{
    public class GoalChartResponse
    {
        public string[] Labels { get; set; }            
        public int[] Achieved { get; set; }            
        public int[] NotAchieved { get; set; }          
        public double[] ProgressPercentages { get; set; }
    }
}
