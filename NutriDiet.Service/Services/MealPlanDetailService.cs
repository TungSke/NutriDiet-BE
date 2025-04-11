using Mapster;
using Microsoft.EntityFrameworkCore;
using NutriDiet.Common;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Common.Enums;
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

        public async Task DeleteMealPlanDetail(int id)
        {
            var mealPlanDetailExisted = await _unitOfWork.MealPlanDetailRepository.GetByIdAsync(id);
            if (mealPlanDetailExisted == null)
            {
                throw new Exception("MealPlanDetail not found");
            }
            await _unitOfWork.BeginTransaction();
            try
            {

                var mealPlanId = mealPlanDetailExisted.MealPlanId;

                await _unitOfWork.MealPlanDetailRepository.DeleteAsync(mealPlanDetailExisted);
                await _unitOfWork.SaveChangesAsync();

                var remainingDetails = await _unitOfWork.MealPlanDetailRepository
                                    .GetAll()
                                    .Where(d => d.MealPlanId == mealPlanId)
                                    .OrderBy(d => d.DayNumber)
                                    .ToListAsync();

                var dayToDelete = mealPlanDetailExisted.DayNumber;
                bool dayIsEmpty = !remainingDetails.Any(x => x.DayNumber == dayToDelete);
                if (dayIsEmpty)
                {
                    //dồn daynumber sau ngày bị xóa
                    int newDayNumber = 1;
                    var groupByDay = remainingDetails.GroupBy(x => x.DayNumber).OrderBy(x=>x.Key);
                    foreach(var group in groupByDay)
                    {
                        foreach(var detail in group)
                        {
                            if(detail.DayNumber != newDayNumber)
                            {
                                detail.DayNumber = newDayNumber;
                                await _unitOfWork.MealPlanDetailRepository.UpdateAsync(detail);
                            }
                        }
                        newDayNumber++;
                    }
                }

                var mealPlan = await _unitOfWork.MealPlanRepository.GetByIdAsync(mealPlanId);
                if (mealPlan != null)
                {
                    mealPlan.Duration = remainingDetails.Select(d => d.DayNumber).Distinct().Count();
                    await _unitOfWork.MealPlanRepository.UpdateAsync(mealPlan);

                    // Đồng bộ MealLog nếu MealPlan đang Active
                    await SyncActiveMealPlanWithMealLog(mealPlanId);

                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransaction();
                }
            }catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                throw ex;
            }
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

                    var foodExistedInMeal = _unitOfWork.MealPlanDetailRepository
                    .GetByWhere(x => x.MealPlanId == mealPlanId
                     && x.FoodId == mealPlanDetailRequest.FoodId
                     && x.MealType == mealPlanDetailRequest.MealType
                     && x.DayNumber == mealPlanDetailRequest.DayNumber)
                    .FirstOrDefault();
                    if (foodExistedInMeal != null)
                    {
                        foodExistedInMeal.Quantity += mealPlanDetailRequest.Quantity;
                        
                        //foodExistedInMeal.TotalCalories = foodExist.Calories * (foodExistedInMeal.Quantity ?? 1);
                        //foodExistedInMeal.TotalCarbs = foodExist.Carbs * (foodExistedInMeal.Quantity ?? 1);
                        //foodExistedInMeal.TotalFat = foodExist.Fat * (foodExistedInMeal.Quantity ?? 1);
                        //foodExistedInMeal.TotalProtein = foodExist.Protein * (foodExistedInMeal.Quantity ?? 1);

                        await _unitOfWork.MealPlanDetailRepository.UpdateAsync(foodExistedInMeal);
                    }
                    else
                    {
                        var mealPlanDetail = mealPlanDetailRequest.Adapt<MealPlanDetail>();
                        mealPlanDetail.MealPlanId = mealPlan.MealPlanId;
                        mealPlanDetail.FoodName = foodExist.FoodName;
                        //mealPlanDetail.TotalCalories = (foodExist.Calories) * (mealPlanDetailRequest.Quantity ?? 1);
                        //mealPlanDetail.TotalCarbs = (foodExist.Carbs) * (mealPlanDetailRequest.Quantity ?? 1);
                        //mealPlanDetail.TotalFat = (foodExist.Fat) * (mealPlanDetailRequest.Quantity ?? 1);
                        //mealPlanDetail.TotalProtein = (foodExist.Protein) * (mealPlanDetailRequest.Quantity ?? 1);
                
                        await _unitOfWork.MealPlanDetailRepository.AddAsync(mealPlanDetail);
                    }

                //update meal plan duration
                var existedDetail = _unitOfWork.MealPlanDetailRepository.GetAll().Where(x=>x.MealPlanId == mealPlan.MealPlanId);

                var uniqueDays = existedDetail.Select(x=>x.DayNumber).ToHashSet();
                uniqueDays.Add(mealPlanDetailRequest.DayNumber);

                mealPlan.Duration = uniqueDays.Count();
                await _unitOfWork.MealPlanRepository.UpdateAsync(mealPlan);

                // Đồng bộ MealLog nếu MealPlan đang Active
                await SyncActiveMealPlanWithMealLog(mealPlanId);

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

        public async Task<IBusinessResult> UpdateMealPlanDetail(UpdateMealPlanDetailRequest updateRequest)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                    var mealPlanDetail = await _unitOfWork.MealPlanDetailRepository.GetByIdAsync(updateRequest.MealPlanDetailId);
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
                    //mealPlanDetail.TotalCalories = (foodExist.Calories) * (updateRequest.Quantity ?? 1);
                    //mealPlanDetail.TotalCarbs = (foodExist.Carbs) * (updateRequest.Quantity ?? 1);
                    //mealPlanDetail.TotalFat = (foodExist.Fat) * (updateRequest.Quantity ?? 1);
                    //mealPlanDetail.TotalProtein = (foodExist.Protein) * (updateRequest.Quantity ?? 1);
                
                await _unitOfWork.MealPlanDetailRepository.UpdateAsync(mealPlanDetail);

                //update meal plan duration

                var mealPlanId = mealPlanDetail.MealPlanId;
                var mealPlan = await _unitOfWork.MealPlanRepository.GetByIdAsync(mealPlanId);
                if (mealPlan != null)
                {
                    var existedDetail = _unitOfWork.MealPlanDetailRepository.GetAll().Where(x => x.MealPlanId == mealPlan.MealPlanId);

                    var uniqueDays = existedDetail.Select(x => x.DayNumber).ToHashSet();
                    
                    mealPlan.Duration = uniqueDays.Count();
                    await _unitOfWork.MealPlanRepository.UpdateAsync(mealPlan);
                }

                // Đồng bộ MealLog nếu MealPlan đang Active
                await SyncActiveMealPlanWithMealLog(mealPlanId);

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
                    .GroupBy(x => new { x.DayNumber, x.MealType })
                    .Select(g => new TotalByMealType
                    {
                        DayNumber = g.Key.DayNumber, 
                        MealType = g.Key.MealType,
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

        public async Task SyncActiveMealPlanWithMealLog(int mealPlanId)
        {
            var mealPlan = await _unitOfWork.MealPlanRepository
                .GetByWhere(x=>x.MealPlanId == mealPlanId)
                .Include(x=>x.MealPlanDetails)
                .FirstOrDefaultAsync();

            if (mealPlan == null || mealPlan.Status != MealplanStatus.Active.ToString() || !mealPlan.StartAt.HasValue)
            {
                return;
            }

            var userId = mealPlan.UserId;
            
            var startDay = mealPlan.StartAt.Value.Date;
            var currentDay = DateTime.Now.Date;

            // only sync từ ngày hiện tại trở đi
            foreach(var meaplandetail in mealPlan.MealPlanDetails.GroupBy(x => x.DayNumber))
            {
                var dayNumber = meaplandetail.Key;
                var logDate = startDay.AddDays(dayNumber - 1);
                if(logDate < currentDay) continue; // bỏ qua ngày trong quá khứ

                var existingMealLog = await _unitOfWork.MealLogRepository
                    .GetByWhere(x=>x.UserId == userId && x.LogDate.Value.Date == logDate.Date)
                    .Include(x=>x.MealLogDetails)
                    .FirstOrDefaultAsync();

                if(existingMealLog != null)
                {
                    await _unitOfWork.MealLogRepository.DeleteAsync(existingMealLog);
                }

                //tạo meallogdetail
                var mealLogDetails = meaplandetail.Select( detail => new MealLogDetail
                {
                    FoodId = detail.FoodId,
                    Quantity = detail.Quantity,
                    MealType = detail.MealType,
                    Calories = detail.TotalCalories,
                    Carbs = detail.TotalCarbs,
                    Fat = detail.TotalFat,
                    Protein = detail.TotalProtein
                }).ToList();

                //tạo meallog
                var mealLog = new MealLog
                {
                    UserId = userId,
                    LogDate = logDate,
                    TotalCalories = mealLogDetails.Sum(x => x.Calories ?? 0),
                    TotalProtein = mealLogDetails.Sum(x => x.Protein ?? 0),
                    TotalCarbs = mealLogDetails.Sum(x => x.Carbs ?? 0),
                    TotalFat = mealLogDetails.Sum(x => x.Fat ?? 0),
                    MealLogDetails = mealLogDetails
                };
                await _unitOfWork.MealLogRepository.AddAsync(mealLog);
            }
        }
    }
}
