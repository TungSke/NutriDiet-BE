using Microsoft.AspNetCore.Http;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Common.Enums;
using NutriDiet.Service.ModelDTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Interface
{
    public interface IMealLogService
    {
        Task<IBusinessResult> AddOrUpdateMealLog(MealLogRequest request);
        Task<IBusinessResult> RemoveMealLog(int mealLogId);
        Task<IBusinessResult> RemoveMealLogDetail(int mealLogId,int detailId);
        Task<IBusinessResult> GetMealLogsByDateRange(DateTime? logDate, DateTime? fromDate, DateTime? toDate);
        Task<IBusinessResult> QuickAddMealLogDetail(QuickMealLogRequest request);
        Task<IBusinessResult> CopyMealLogDetails(CopyMealLogRequest request);
        Task<IBusinessResult> CreateMealLogAI();
        Task<IBusinessResult> SaveMeallogAI(string feedback);
        Task<IBusinessResult> TransferMealLogDetail(int detailId, MealType targetMealType);
        Task<IBusinessResult> AddMealToMultipleDays(AddMultipleDaysMealLogRequest request);
        Task<IBusinessResult> GetNutritionSummary(DateTime date);
        Task<IBusinessResult> AddImageToMealLogDetail(int detailId, AddImageRequest request);
        Task<IBusinessResult> GetMealLogDetail(int detailId);
        Task<IBusinessResult> UpdateMealLogDetailNutrition(int detailId, UpdateMealLogNutritionRequest request);
        Task<bool> IsDailyCaloriesExceeded(DateTime? logDate, double additionalCalories);
        Task<IBusinessResult> AnalyzeAndPredictMealImprovements(DateTime logDate);
    }
}
