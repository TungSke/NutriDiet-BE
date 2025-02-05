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

        public async Task<IBusinessResult> CreateFood(FoodRequest request)
        {
            var existedfood = await _unitOfWork.FoodRepository.GetByWhere(x => x.FoodName.ToLower().Equals(request.FoodName.ToLower())).FirstOrDefaultAsync();

            if (existedfood != null)
            {
                return new BusinessResult(Const.HTTP_STATUS_CONFLICT, "Food name existed");
            }

            var food = request.Adapt<Food>();
            if(request.FoodImageUrl != null)
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


        public async Task UpdateFood(UpdateFoodRequest request)
        {
            var food = await _unitOfWork.FoodRepository.GetByIdAsync(request.FoodId);
            if (food == null)
            {
                throw new Exception("Food not found");
            }

            food.Adapt(request);

            // Cập nhật ảnh nếu có ảnh mới
            if (request.FoodImageUrl != null)
            {
                var cloudinaryHelper = new CloudinaryHelper();
                var imageUrl = await cloudinaryHelper.UploadImageWithCloudDinary(request.FoodImageUrl);
                food.ImageUrl = imageUrl;
            }

            await _unitOfWork.FoodRepository.UpdateAsync(food);
            await _unitOfWork.SaveChangesAsync();
        }

    }
}
