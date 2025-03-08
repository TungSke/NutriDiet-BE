using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class RejectRecipeRequest
    {
        public int FoodId { get; set; }
        public string RejectionReason { get; set; }
    }
}
