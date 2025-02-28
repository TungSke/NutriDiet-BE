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

                    var foodExist = await _unitOfWork.FoodRepository.GetByIdAsync(mealPlanDetailRequest.FoodId);
                    if (foodExist == null)
                    {
                        return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, $"Food not found.");
                    }

                    var mealPlanDetail = mealPlanDetailRequest.Adapt<MealPlanDetail>();
                    mealPlanDetail.MealPlanId = mealPlan.MealPlanId;
                    mealPlanDetail.FoodName = foodExist.FoodName;
                    mealPlanDetail.TotalCalories = (foodExist.Calories) * (mealPlanDetailRequest.Quantity ?? 1);
                    mealPlanDetail.TotalCarbs = (foodExist.Carbs) * (mealPlanDetailRequest.Quantity ?? 1);
                    mealPlanDetail.TotalFat = (foodExist.Fat) * (mealPlanDetailRequest.Quantity ?? 1);
                    mealPlanDetail.TotalProtein = (foodExist.Protein) * (mealPlanDetailRequest.Quantity ?? 1);

                await _unitOfWork.MealPlanDetailRepository.AddAsync(mealPlanDetail);


                //update meal plan duration
                var existedDetail = _unitOfWork.MealPlanDetailRepository.GetAll().Where(x=>x.MealPlanId == mealPlan.MealPlanId);

                var uniqueDays = existedDetail.Select(x=>x.DayNumber).ToHashSet();
                uniqueDays.Add(mealPlanDetailRequest.DayNumber);

                mealPlan.Duration = uniqueDays.Count();
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

        public async Task<IBusinessResult> UpdateMealPlanDetail(int mealPlanDetailId, UpdateMealPlanDetailRequest updateRequest)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                    var mealPlanDetail = await _unitOfWork.MealPlanDetailRepository.GetByIdAsync(mealPlanDetailId);
                    if (mealPlanDetail == null)
                    {
                        return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "MealPlanDetail not found.");
                    }

                    var foodExist = await _unitOfWork.FoodRepository.GetByIdAsync(updateRequest.FoodId);
                    if (foodExist == null)
                    {
                        return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Food not found.");
                    }

                    mealPlanDetail.FoodId = updateRequest.FoodId;
                    mealPlanDetail.FoodName = foodExist.FoodName;
                    mealPlanDetail.Quantity = updateRequest.Quantity;
                    mealPlanDetail.MealType = updateRequest.MealType;
                    mealPlanDetail.DayNumber = updateRequest.DayNumber;
                    mealPlanDetail.TotalCalories = (foodExist.Calories) * (updateRequest.Quantity ?? 1);
                    mealPlanDetail.TotalCarbs = (foodExist.Carbs) * (updateRequest.Quantity ?? 1);
                    mealPlanDetail.TotalFat = (foodExist.Fat) * (updateRequest.Quantity ?? 1);
                    mealPlanDetail.TotalProtein = (foodExist.Protein) * (updateRequest.Quantity ?? 1);
                
                await _unitOfWork.MealPlanDetailRepository.UpdateAsync(mealPlanDetail);

                //update meal plan duration

                var mealplanId = mealPlanDetail.MealPlanId;
                var mealPlan = await _unitOfWork.MealPlanRepository.GetByIdAsync(mealplanId);
                if (mealPlan != null)
                {
                    var existedDetail = _unitOfWork.MealPlanDetailRepository.GetAll().Where(x => x.MealPlanId == mealPlan.MealPlanId);

                    var uniqueDays = existedDetail.Select(x => x.DayNumber).ToHashSet();
                    
                    mealPlan.Duration = uniqueDays.Count();
                    await _unitOfWork.MealPlanRepository.UpdateAsync(mealPlan);
                }
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

        public async Task<IBusinessResult> GetMealPlanDetailTotals(int mealPlanId)
        {
            var mealPlanDetails = await _unitOfWork.MealPlanDetailRepository
                .GetAll()
                .Where(x => x.MealPlanId == mealPlanId)
                .ToListAsync();

            if (!mealPlanDetails.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "MealPlan details not found.");
            }

            var response = new MealPlanTotalResponse
            {
                TotalByMealType = mealPlanDetails
                    .GroupBy(x => x.MealType)
                    .Select(g => new TotalByMealType
                    {
                        MealType = g.Key,
                        TotalCalories = g.Sum(x => x.TotalCalories),
                        TotalCarbs = g.Sum(x => x.TotalCarbs),
                        TotalFat = g.Sum(x => x.TotalFat),
                        TotalProtein = g.Sum(x => x.TotalProtein)
                    }).ToList(),

                TotalByDayNumber = mealPlanDetails
                    .GroupBy(x => x.DayNumber)
                    .Select(g => new TotalByDayNumber
                    {
                        DayNumber = g.Key,
                        TotalCalories = g.Sum(x => x.TotalCalories),
                        TotalCarbs = g.Sum(x => x.TotalCarbs),
                        TotalFat = g.Sum(x => x.TotalFat),
                        TotalProtein = g.Sum(x => x.TotalProtein)
                    }).ToList()
            };

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

    }
}
