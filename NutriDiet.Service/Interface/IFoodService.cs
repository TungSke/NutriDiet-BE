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

        Task<IBusinessResult> DeleteFood(int foodId);

        Task<IBusinessResult> UpdateFood(UpdateFoodRequest request);

        Task<IBusinessResult> GetFoodRecommend(int pageIndex, int pageSize, string searchName);

        Task<IBusinessResult> FoodRecipe(int foodId);
    }
}
