using NutriDiet.Common;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using NutriDiet.Service.ModelDTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Services
{
    public class FoodIngredientService
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

            return new BusinessResult();
        }


    }
}
