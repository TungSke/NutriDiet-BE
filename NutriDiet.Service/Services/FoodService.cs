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
        private readonly CloudinaryHelper _cloudinaryHelper;
        public FoodService(IUnitOfWork unitOfWork, CloudinaryHelper cloudinaryHelper)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryHelper = cloudinaryHelper;
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


        //public async Task<IBusinessResult> GetFoodById(int foodId)
        //{
        //    var food = await _unitOfWork.FoodRepository.GetByWhere(x => x.FoodId == foodId).Include(x => x.FoodDetails).FirstOrDefaultAsync();
        //    if (food == null)
        //    {
        //        return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, Const.FAIL_READ_MSG);
        //    }
        //    var resposne = food.Adapt<FoodResponse>();
        //    return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, resposne);
        //}

        public async Task CreateFood(FoodRequest request)
        {
            var imageUrl = "";
            if (request.FoodImageUrl != null)
            {
                imageUrl = await _cloudinaryHelper.UploadImageWithCloudDinary(request.FoodImageUrl);
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

            var imageUrl = "";
            if (request.FoodImageUrl != null)
            {
                imageUrl = await _cloudinaryHelper.UploadImageWithCloudDinary(request.FoodImageUrl);
            }

            await _unitOfWork.FoodRepository.UpdateAsync(food);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
