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
        Task<IBusinessResult> RemoveMealLogDetail(int mealLogId,int detailId);
        Task<IBusinessResult> GetMealLogById(int mealLogId);
        Task<IBusinessResult> GetMealLogsByDateRange(DateTime? logDate, DateTime? fromDate, DateTime? toDate);
        Task<IBusinessResult> QuickAddMealLogDetail(QuickMealLogRequest request);
        Task<IBusinessResult> CopyMealLogDetails(CopyMealLogRequest request);

        Task<IBusinessResult> CreateMealLogAI();
        Task<IBusinessResult> SaveMeallogAI();
        Task<IBusinessResult> TransferMealLogDetail(int detailId, MealType targetMealType);
        Task<IBusinessResult> GetRecentFoods();
        Task<IBusinessResult> AddMealToMultipleDays(AddMultipleDaysMealLogRequest request);
    }
}
