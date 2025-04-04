using NutriDiet.Common.BusinessResult;

namespace NutriDiet.Service.Interface
{
    public interface IDashboardService
    {
        Task<IBusinessResult> Dashboard();
        Task<IBusinessResult> Revenue();
        Task<IBusinessResult> Transaction(int pageIndex, int pageSize, string? search);
    }
}
