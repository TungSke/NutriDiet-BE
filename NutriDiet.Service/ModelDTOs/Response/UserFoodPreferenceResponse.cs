using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Response
{
    public class UserFoodPreferenceResponse
    {
        public int UserFoodPreferenceId { get; set; }

        public int UserId { get; set; }
        public string FullName { get; set; }

        public int FoodId { get; set; }
        public string FoodName { get; set; }
        
    }
}
