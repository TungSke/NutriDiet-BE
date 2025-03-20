using Mapster;
using Microsoft.EntityFrameworkCore;
using NutriDiet.Common;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using NutriDiet.Service.Helpers;
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
        private readonly TokenHandlerHelper _tokenHandlerHelper;

        public IngreDientSevice(IUnitOfWork unitOfWork, TokenHandlerHelper tokenHandlerHelper)
        {
            _unitOfWork = unitOfWork;
            _tokenHandlerHelper = tokenHandlerHelper;
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
            var ingre = await _unitOfWork.IngredientRepository.GetByWhere(x => x.IngredientName.ToLower() == request.IngredientName).FirstOrDefaultAsync();
            if (ingre != null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Food not found");
            }
            ingre = request.Adapt<Ingredient>();
            await _unitOfWork.IngredientRepository.AddAsync(ingre);
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

        public async Task<IBusinessResult> PreferenceIngredient(int ingredientId, int preferenceLevel)
        {
            var userId = await _tokenHandlerHelper.GetUserId();

            var ingredients = await _unitOfWork.IngredientRepository.GetByIdAsync(ingredientId);
            if (ingredients == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "No preference ingredient found");
            }

            var userIngredient = await _unitOfWork.UserIngredientPreferenceRepository.GetByWhere(x => x.UserId == userId && x.IngredientId == ingredientId).FirstOrDefaultAsync();
            if (userIngredient == null)
            {
                userIngredient = new UserIngreDientPreference
                {
                    UserId = userId,
                    IngredientId = ingredientId,
                    Level = preferenceLevel
                };
                await _unitOfWork.UserIngredientPreferenceRepository.AddAsync(userIngredient);
            }
            else
            {
                userIngredient.Level = preferenceLevel;
                await _unitOfWork.UserIngredientPreferenceRepository.UpdateAsync(userIngredient);
            }

            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG);
        }
    }
}
