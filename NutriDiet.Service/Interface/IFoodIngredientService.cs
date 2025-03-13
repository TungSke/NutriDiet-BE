using NutriDiet.Common.BusinessResult;
using NutriDiet.Service.ModelDTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Interface
{
    public interface IFoodIngredientService
    {
        Task<IBusinessResult> AddFoodIngredientAsync(int foodId, List<FoodIngredientRequest> foodIngredient);

        Task<IBusinessResult> UpdateFoodIngredientAsync(int foodId, List<FoodIngredientRequest> foodIngredients);

        Task<IBusinessResult> DeleteFoodIngredientAsync(int foodId, int ingredientId);
    }
}
