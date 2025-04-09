using NutriDiet.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class PersonalGoalRequest
    {
        public GoalType GoalType { get; set; }
        [Range(30, 250,
            ErrorMessage = "TargetWeight must be between 30 and 250 kg")]
        public double? TargetWeight { get; set; }
        public WeightChangeRate? WeightChangeRate { get; set; }
        public string GoalDescription { get; set; } = null!;
        public string? Notes { get; set; }
    }
}
