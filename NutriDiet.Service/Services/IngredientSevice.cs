using Mapster;
using Microsoft.EntityFrameworkCore;
using NutriDiet.Common;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using NutriDiet.Service.ModelDTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Services
{
    public class IngreDientSevice : IIngreDientService
    {
        private readonly IUnitOfWork _unitOfWork;

        public IngreDientSevice(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IBusinessResult> GetIngreDients(int pageIndex, int pageSize, string search)
        {
            search = search?.ToLower() ?? string.Empty;

            var foods = await _unitOfWork.IngredientRepository.GetPagedAsync(
                pageIndex,
                pageSize,
                x => (string.IsNullOrEmpty(search) || x.IngredientName.ToLower().Contains(search))
            );

            if (foods == null || !foods.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
            }

            var response = foods.Adapt<List<IngredientResponse>>();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> GetIngredientById(int ingredientId)
        {
            var ingredient = await _unitOfWork.IngredientRepository.GetByIdAsync(ingredientId);
            if (ingredient == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Ingredient not found");
            }

            var response = ingredient.Adapt<IngredientResponse>();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> AddIngredient(IngredientRequest request)
        {
            var ingre = await _unitOfWork.FoodRepository.GetByIdAsync(request.IngredientName);
            if (ingre == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Food not found");
            }

            await _unitOfWork.FoodRepository.AddAsync(ingre);
            await _unitOfWork.SaveChangesAsync();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_CREATE_MSG);
        }

        public async Task<IBusinessResult> UpdateIngredient(int ingredientid, IngredientRequest request)
        {
            var ingredient = await _unitOfWork.IngredientRepository.GetByIdAsync(ingredientid);
            if (ingredient == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "ingredient not found");
            }

            request.Adapt(ingredient);
            await _unitOfWork.IngredientRepository.UpdateAsync(ingredient);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_UPDATE_MSG);
        }

        public async Task<IBusinessResult> DeleteIngredient(int ingredientId)
        {
            var ingredient = await _unitOfWork.IngredientRepository.GetByIdAsync(ingredientId);
            if (ingredient == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Ingredient not found");
            }

            await _unitOfWork.IngredientRepository.DeleteAsync(ingredient);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_DELETE_MSG);
        }
    }
}
