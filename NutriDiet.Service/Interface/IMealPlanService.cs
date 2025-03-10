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
        Task<IBusinessResult> SearchMealPlan(int pageIndex, int pageSize, string? status, string? search);
        Task<IBusinessResult> CreateMealPlan(MealPlanRequest mealPlanRequest);
        Task<IBusinessResult> UpdateMealPlan(int mealPlanID, UpdateMealPlanRequest mealPlanRequest);
        Task DeleteMealPlan(int id);
        Task ChangStatusMealPlan(int id, string status);

        Task<IBusinessResult> GetMealPlanDetailByMealPlanID(int mealPlanID);
        Task<IBusinessResult> CloneSampleMealPlan(int mealPlanID);

        Task<IBusinessResult> CreateSuitableMealPlanByAI();
        Task<IBusinessResult> GetMealPlanByID(int mealPlanId);

        Task<IBusinessResult> RejectMealplan(string rejectReason);

        Task<IBusinessResult> SaveMealPlanAI();
        Task<IBusinessResult> ApplyMealPlan(int mealPlanId);
        Task<IBusinessResult> GetSampleMealPlan(int pageIndex, int pageSize, string? search);
        Task<IBusinessResult> GetMyMealPlan(int pageIndex, int pageSize, string? search);
    }
}
