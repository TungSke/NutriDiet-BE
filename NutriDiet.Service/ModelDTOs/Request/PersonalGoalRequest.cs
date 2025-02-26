using NutriDiet.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class PersonalGoalRequest
    {
        public GoalType GoalType { get; set; } 
        public double? TargetWeight { get; set; }
        public WeightChangeRate? WeightChangeRate { get; set; }
        public string GoalDescription { get; set; } = null!;
        public string? Notes { get; set; }
    }
}
