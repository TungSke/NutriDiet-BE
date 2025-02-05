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

        Task<IBusinessResult> CreateFood(FoodRequest request);

        Task<IBusinessResult> InsertIngredient(InsertIngredientRequest request);

        Task UpdateFood(UpdateFoodRequest request);
    }
}
