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
        Task DeleteMealPlanDetail(int id);
        Task<IBusinessResult> CreateMealPlanDetail(int mealPlanId, MealPlanDetailRequest mealPlanDetailRequest);
        Task<IBusinessResult> UpdateMealPlanDetail(UpdateMealPlanDetailRequest updateRequest);
        Task<IBusinessResult> GetMealPlanDetailTotals(int mealPlanId);
    }
}
