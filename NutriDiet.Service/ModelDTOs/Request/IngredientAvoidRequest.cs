using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class IngredientAvoidRequest
    {
        public List<int> ingredientIds { get; set; } = new List<int>();
    }
}
