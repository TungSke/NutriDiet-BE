using NutriDiet.Common.BusinessResult;

namespace NutriDiet.Service.Interface
{
    public interface IDashboardService
    {
        Task<IBusinessResult> Dashboard();
    }
}
