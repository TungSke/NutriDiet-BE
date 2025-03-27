using Microsoft.AspNetCore.Http;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Service.ModelDTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Interface
{
    public interface IIngreDientService
    {
        Task<IBusinessResult> GetIngreDients(int pageIndex, int pageSize, string search);

        Task<IBusinessResult> GetIngredientById(int ingredientId);

        Task<IBusinessResult> AddIngredient(IngredientRequest request);

        Task<IBusinessResult> UpdateIngredient(int ingredientId, IngredientRequest request);

        Task<IBusinessResult> DeleteIngredient(int ingredientId);

        Task<IBusinessResult> UpdatePreferenceIngredient(int ingredientId, int preferenceLevel);

        Task<IBusinessResult> GetPreferenceIngredient();

        Task<IBusinessResult> ImportIngredientsFromExcel(IFormFile excelFile);
    }
}
