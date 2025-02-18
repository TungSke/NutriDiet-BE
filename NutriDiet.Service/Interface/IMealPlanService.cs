using NutriDiet.Common.BusinessResult;
using NutriDiet.Service.ModelDTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Interface
{
    public interface IMealPlanService
    {
        Task<IBusinessResult> SearchMealPlan(string? planName, string? healthGoal);
        Task<IBusinessResult> CreateMealPlan(MealPlanRequest mealPlanRequest);
        Task DeleteMealPlan(int id);
        Task ChangStatusMealPlan(int id, string status);

        Task<IBusinessResult> GetMealPlanDetailByMealPlanID(int mealPlanID);
        Task<IBusinessResult> CloneSampleMealPlan(int mealPlanID);


        Task<IBusinessResult> CreateSuitableMealPlanByAI();
        Task<IBusinessResult> GetMealPlanByID(int mealPlanId);

    }
}
