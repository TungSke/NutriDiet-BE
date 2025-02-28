using Mapster;
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
    public class MealPlanDetailService : IMealPlanDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        public MealPlanDetailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IBusinessResult> GetAllMealPlanDetail()
        {
            var detail = _unitOfWork.MealPlanDetailRepository.GetAll().ToList();
            if(detail == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
            }
            var response = detail.Adapt<List<MealPlanDetailResponse>>();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task DeleteMealPlanDetail(int id)
        {
            var mealPlanDetailExisted = await _unitOfWork.MealPlanDetailRepository.GetByIdAsync(id);
            await _unitOfWork.MealPlanDetailRepository.DeleteAsync(mealPlanDetailExisted);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IBusinessResult> CreateMealPlanDetail(int mealPlanId, MealPlanDetailRequest mealPlanDetailRequest)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                var mealPlan = await _unitOfWork.MealPlanRepository.GetByIdAsync(mealPlanId);
                if (mealPlan == null)
                {
                    return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "MealPlan not found.");
                }

                var dayNumberSet = new HashSet<int>();

                    var foodExist = await _unitOfWork.FoodRepository.GetByIdAsync(mealPlanDetailRequest.FoodId);
                    if (foodExist == null)
                    {
                        return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, $"Food not found.");
                    }

                    var mealPlanDetail = mealPlanDetailRequest.Adapt<MealPlanDetail>();

                    mealPlanDetail.FoodName = foodExist.FoodName;
                    mealPlanDetail.TotalCalories = (foodExist.Calories) * (mealPlanDetailRequest.Quantity ?? 1);
                    mealPlanDetail.TotalCarbs = (foodExist.Carbs) * (mealPlanDetailRequest.Quantity ?? 1);
                    mealPlanDetail.TotalFat = (foodExist.Fat) * (mealPlanDetailRequest.Quantity ?? 1);
                    mealPlanDetail.TotalProtein = (foodExist.Protein) * (mealPlanDetailRequest.Quantity ?? 1);

                    dayNumberSet.Add(mealPlanDetailRequest.DayNumber);

                await _unitOfWork.MealPlanDetailRepository.AddAsync(mealPlanDetail);

                mealPlan.Duration = dayNumberSet.Count;
                await _unitOfWork.MealPlanRepository.UpdateAsync(mealPlan);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

                return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_CREATE_MSG);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                return new BusinessResult(Const.HTTP_STATUS_INTERNAL_ERROR, $"Error: {ex.Message}");
            }
        }

        public async Task<IBusinessResult> UpdateMealPlanDetail(List<UpdateMealPlanDetailRequest> updateRequest)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                var mealPlanDetails = new List<MealPlanDetail>();
                foreach (var request in updateRequest)
                {
                    var mealPlanDetail = await _unitOfWork.MealPlanDetailRepository.GetByIdAsync(request.MealPlanDetailId);
                    if (mealPlanDetail == null)
                    {
                        return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "MealPlanDetail not found.");
                    }

                    var foodExist = await _unitOfWork.FoodRepository.GetByIdAsync(request.FoodId);
                    if (foodExist == null)
                    {
                        return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Food not found.");
                    }

                    mealPlanDetail.FoodId = request.FoodId;
                    mealPlanDetail.FoodName = foodExist.FoodName;
                    mealPlanDetail.Quantity = request.Quantity;
                    mealPlanDetail.MealType = request.MealType;
                    mealPlanDetail.DayNumber = request.DayNumber;
                    mealPlanDetail.TotalCalories = (foodExist.Calories) * (request.Quantity ?? 1);
                    mealPlanDetail.TotalCarbs = (foodExist.Carbs) * (request.Quantity ?? 1);
                    mealPlanDetail.TotalFat = (foodExist.Fat) * (request.Quantity ?? 1);
                    mealPlanDetail.TotalProtein = (foodExist.Protein) * (request.Quantity ?? 1);
                    mealPlanDetails.Add(mealPlanDetail);
                }
                await _unitOfWork.MealPlanDetailRepository.UpdateRangeAsync(mealPlanDetails);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

                return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_UPDATE_MSG);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                return new BusinessResult(Const.HTTP_STATUS_INTERNAL_ERROR, $"Error: {ex.Message}");
            }
        }

    }
}
