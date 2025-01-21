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

        public async Task CreateFood(FoodRequest request)
        {
            var cloudinaryHelper = new CloudinaryHelper(); // Chỉ khởi tạo ở đây
            var imageUrl = "";

            if (request.FoodImageUrl != null)
            {
                imageUrl = await cloudinaryHelper.UploadImageWithCloudDinary(request.FoodImageUrl);
            }

            var food = request.Adapt<Food>();
            food.ImageUrl = imageUrl;
            await _unitOfWork.FoodRepository.AddAsync(food);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateFood(int foodId, FoodRequest request)
        {
            var food = await _unitOfWork.FoodRepository.GetByIdAsync(foodId);
            if (food == null)
            {
                throw new Exception("Food not found");
            }

            var cloudinaryHelper = new CloudinaryHelper(); // Khởi tạo ở đây nếu cần
            var imageUrl = "";

            if (request.FoodImageUrl != null)
            {
                imageUrl = await cloudinaryHelper.UploadImageWithCloudDinary(request.FoodImageUrl);
            }

            food = request.Adapt(food); // Cập nhật thông tin từ request
            food.ImageUrl = imageUrl;

            await _unitOfWork.FoodRepository.UpdateAsync(food);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
