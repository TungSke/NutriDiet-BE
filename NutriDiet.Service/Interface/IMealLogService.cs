using NutriDiet.Common.BusinessResult;
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
    }
}
