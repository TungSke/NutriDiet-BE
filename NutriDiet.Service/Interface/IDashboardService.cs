using NutriDiet.Common.BusinessResult;
using NutriDiet.Service.ModelDTOs.Response;

namespace NutriDiet.Service.Interface
{
    public interface IDashboardService
    {
        Task<IBusinessResult> Dashboard();
        Task<IBusinessResult> Revenue();
        Task<IBusinessResult> Transaction(int pageIndex, int pageSize, string? search);
        Task<GoalChartResponse> GetGoalProgressChartData();
        Task<IBusinessResult> GetTopSelectedFoods(int top = 10);
    }
}
