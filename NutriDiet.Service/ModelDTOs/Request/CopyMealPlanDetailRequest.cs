using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class CopyMealPlanDetailRequest
    {
        public int dayNumberFrom {  get; set; }
        public int dayNumberTo {  get; set; }
    }
}
