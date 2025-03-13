using Microsoft.EntityFrameworkCore;
using NutriDiet.Common;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Services
{
    public class FoodIngredientService : IFoodIngredientService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FoodIngredientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IBusinessResult> AddFoodIngredientAsync(int foodId, List<FoodIngredientRequest> foodIngredient)
        {
            var food = await _unitOfWork.FoodRepository.GetByIdAsync(foodId);
            if (food == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "food not found");
            }

            foreach (var item in foodIngredient)
            {
                var ingredient = await _unitOfWork.IngredientRepository.GetByIdAsync(item.IngredientId);
                if (ingredient == null)
                {
                    return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "ingredient not found");
                }
                var foodIngredientEntity = new FoodIngredient
                {
                    FoodId = foodId,
                    IngredientId = item.IngredientId,
                    Quantity = item.Quantity,
                    Unit = item.Unit
                };
                await _unitOfWork.FoodIngredientRepository.AddAsync(foodIngredientEntity);
            }

            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, "Success");
        }

        public async Task<IBusinessResult> UpdateFoodIngredientAsync(int foodId, List<FoodIngredientRequest> foodIngredients)
        {
            var food = await _unitOfWork.FoodRepository
                .GetByWhere(x => x.FoodId == foodId)
                .Include(x => x.FoodIngredients)
                .FirstOrDefaultAsync();

            if (food == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Food not found");
            }

            await _unitOfWork.FoodIngredientRepository.RemoveRange(food.FoodIngredients);

            foreach (var item in foodIngredients)
            {
                var ingredient = await _unitOfWork.IngredientRepository.GetByIdAsync(item.IngredientId);
                if (ingredient == null)
                {
                    return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, $"Ingredient with ID {item.IngredientId} not found");
                }

                var newFoodIngredient = new FoodIngredient
                {
                    FoodId = foodId,
                    IngredientId = item.IngredientId,
                    Quantity = item.Quantity,
                    Unit = item.Unit
                };

                await _unitOfWork.FoodIngredientRepository.AddAsync(newFoodIngredient);
            }

            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, "Food ingredients updated successfully");
        }

        public async Task<IBusinessResult> DeleteFoodIngredientAsync(int foodId, int ingredientId)
        {
            var food = await _unitOfWork.FoodRepository
                .GetByWhere(x => x.FoodId == foodId)
                .Include(x => x.FoodIngredients)
                .FirstOrDefaultAsync();
            if (food == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Food not found");
            }
            var ingredient = food.FoodIngredients.FirstOrDefault(x => x.IngredientId == ingredientId);
            await _unitOfWork.FoodIngredientRepository.DeleteAsync(ingredient);
            await _unitOfWork.SaveChangesAsync();
            return new BusinessResult(Const.HTTP_STATUS_OK, "Food ingredients deleted successfully");
        }



    }
}
