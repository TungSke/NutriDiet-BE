using NutriDiet.Common.Enums;
using NutriDiet.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class MealLogRequest
    {
        public int UserId { get; set; }

        public MealType MealType { get; set; }

        public DateTime? LogDate { get; set; }

        public double? TotalCalories { get; set; }

        public virtual ICollection<MealLogDetailRequest> MealLogDetailRequest { get; set; } = new List<MealLogDetailRequest>();
    }
}
