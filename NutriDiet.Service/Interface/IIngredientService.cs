using NutriDiet.Common.BusinessResult;
using NutriDiet.Service.ModelDTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Interface
{
    public interface IIngredientService
    {
        Task<IBusinessResult> GetIngreDients(int pageIndex, int pageSize, string search);

        Task<IBusinessResult> GetIngredientById(int ingredientId);

        Task<IBusinessResult> InsertIngredient(InsertIngredientRequest request);

        Task<IBusinessResult> UpdateIngredient(UpdateIngredientRequest request);

        Task<IBusinessResult> DeleteIngredient(int ingredientId);
    }
}
