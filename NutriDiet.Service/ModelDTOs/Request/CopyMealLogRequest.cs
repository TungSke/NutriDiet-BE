using NutriDiet.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class CopyMealLogRequest
    {
        public DateTime? SourceDate { get; set; } 
        public DateTime? TargetDate { get; set; }  
        public MealType MealType { get; set; }     
    }

}
