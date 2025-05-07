using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Response
{
    public class MealLogResponse
    {
        public int MealLogId { get; set; }
        public DateTime? LogDate { get; set; }
        public double? DailyCalories { get; set; }
        public double TotalCalories { get; set; }
        public double TotalProtein { get; set; }
        public double TotalCarbs { get; set; }
        public double TotalFat { get; set; }
        public List<MealLogDetailResponse> MealLogDetails { get; set; } = new List<MealLogDetailResponse>();
    }
}
