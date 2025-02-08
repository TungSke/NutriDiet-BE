using Mapster;
using Microsoft.EntityFrameworkCore;
using NutriDiet.Common;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using NutriDiet.Service.ModelDTOs.Response;
using NutriDiet.Service.Utilities;

namespace NutriDiet.Service.Services
{
    public class FoodService : IFoodService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FoodService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IBusinessResult> GetAllFood(int pageIndex, int pageSize, string foodType, string search)
        {
            search = search?.ToLower() ?? string.Empty;
            foodType = foodType?.ToLower();

            var foods = await _unitOfWork.FoodRepository.GetPagedAsync(
                pageIndex,
                pageSize,
                x => (string.IsNullOrEmpty(search) || x.FoodName.ToLower().Contains(search)) &&
                     (string.IsNullOrEmpty(foodType) || x.FoodType.ToLower() == foodType)
            );

            if (foods == null || !foods.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
            }

            var response = foods.Adapt<List<FoodResponse>>();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> GetFoodById(int foodId)
        {
            var food = await _unitOfWork.FoodRepository.GetByWhere(x => x.FoodId == foodId).Include(x => x.Ingredients).FirstOrDefaultAsync();
            if (food == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Food not found");
            }

            var response = food.Adapt<FoodResponse>();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> CreateFood(FoodRequest request)
        {
            var existedfood = await _unitOfWork.FoodRepository.GetByWhere(x => x.FoodName.ToLower().Equals(request.FoodName.ToLower())).FirstOrDefaultAsync();

            if (existedfood != null)
            {
                return new BusinessResult(Const.HTTP_STATUS_CONFLICT, "Food name existed");
            }

            var food = request.Adapt<Food>();
            if (request.FoodImageUrl != null)
            {
                var cloudinaryHelper = new CloudinaryHelper();
                var imageUrl = await cloudinaryHelper.UploadImageWithCloudDinary(request.FoodImageUrl);
                food.ImageUrl = imageUrl;
            }
            await _unitOfWork.FoodRepository.AddAsync(food);
            await _unitOfWork.SaveChangesAsync();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_CREATE_MSG);
        }

        public async Task<IBusinessResult> InsertIngredient(InsertIngredientRequest request)
        {
            var food = await _unitOfWork.FoodRepository.GetByIdAsync(request.FoodId);
            if (food == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Food not found");
            }

            food.Ingredients = request.Ingredients.Adapt<List<Ingredient>>();
            await _unitOfWork.FoodRepository.UpdateAsync(food);
            await _unitOfWork.SaveChangesAsync();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_CREATE_MSG);
        }

        public async Task CreateFoodWithIngredient()
        {
        }

        public async Task<IBusinessResult> UpdateFood(UpdateFoodRequest request)
        {
            var food = await _unitOfWork.FoodRepository.GetByIdAsync(request.FoodId);
            if (food == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Food not found");
            }

            request.Adapt(food);

            if (request.FoodImageUrl != null)
            {
                var cloudinaryHelper = new CloudinaryHelper();
                var imageUrl = await cloudinaryHelper.UploadImageWithCloudDinary(request.FoodImageUrl);
                food.ImageUrl = imageUrl;
            }

            await _unitOfWork.FoodRepository.UpdateAsync(food);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_UPDATE_MSG);
        }

        public async Task<IBusinessResult> UpdateIngredient(UpdateIngredientRequest request)
        {
            var ingredient = await _unitOfWork.IngredientRepository.GetByIdAsync(request.IngredientId);
            if (ingredient == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "ingredient not found");
            }

            request.Adapt(ingredient);
            await _unitOfWork.IngredientRepository.UpdateAsync(ingredient);   
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_UPDATE_MSG);
        }

        public async Task<IBusinessResult> DeleteFood(int foodId)
        {
            var food = await _unitOfWork.FoodRepository.GetByIdAsync(foodId);
            if (food == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Food not found");
            }

            await _unitOfWork.FoodRepository.DeleteAsync(food);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_DELETE_MSG);
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
    }
}
