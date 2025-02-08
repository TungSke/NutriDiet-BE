using NutriDiet.Common.BusinessResult;
using NutriDiet.Service.ModelDTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Interface
{
    public interface IFoodService
    {
        Task<IBusinessResult> GetAllFood(int pageindex, int pagesize, string foodtype, string search);

        Task<IBusinessResult> GetFoodById(int foodId);

        Task<IBusinessResult> CreateFood(FoodRequest request);

        Task<IBusinessResult> InsertIngredient(InsertIngredientRequest request);

        Task<IBusinessResult> UpdateFood(UpdateFoodRequest request);

        Task<IBusinessResult> UpdateIngredient(UpdateIngredientRequest request);

        Task<IBusinessResult> DeleteIngredient(int ingredientId);

        Task<IBusinessResult> GetIngredientById(int ingredientId);
    }
}
