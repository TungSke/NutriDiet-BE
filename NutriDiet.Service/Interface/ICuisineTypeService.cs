using NutriDiet.Common.BusinessResult;
using NutriDiet.Service.ModelDTOs.Request;

namespace NutriDiet.Service.Interface
{
    public interface ICuisineTypeService
    {
        Task<IBusinessResult> GetAllCuisineTypes();
        Task<IBusinessResult> GetCuisineTypeById(int id);
        Task<IBusinessResult> CreateCuisineType(CuisineTypeRequest cuisineType);
        Task<IBusinessResult> DeleteCuisineType(int id);
    }
}
