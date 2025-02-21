using NutriDiet.Common.BusinessResult;
using NutriDiet.Service.ModelDTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Interface
{
    public interface IMealPlanDetailService
    {
        Task<IBusinessResult> GetAllMealPlanDetail();
        Task DeleteMealPlanDetail(int id);
        Task<IBusinessResult> CreateMealPlanDetail(int mealPlanId, List<MealPlanDetailRequest> mealPlanDetailRequest);
        Task<IBusinessResult> UpdateMealPlanDetail(List<UpdateMealPlanDetailRequest> updateRequest);
    }
}
