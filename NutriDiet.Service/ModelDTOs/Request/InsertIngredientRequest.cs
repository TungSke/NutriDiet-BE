using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class InsertIngredientRequest
    {
        public int FoodId { get; set; }

        public IEnumerable<IngredientRequest> Ingredients { get; set; } = null!;
    }
}
