using Google.Apis.Drive.v3.Data;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Services
{
    public class MealPlanService : IMealPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userIdClaim;

        public MealPlanService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _userIdClaim = GetUserIdClaim();
        }

        private string GetUserIdClaim()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public async Task<IBusinessResult> SearchMealPlan(string? planName, string? healthGoal, int? userID)
        {
            var mealPlans = _unitOfWork.MealPlanRepository.GetAll().Include(x=>x.User).ToList();
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userID);
            if (!string.IsNullOrEmpty(planName))
            {
                mealPlans = mealPlans.Where(x => x.PlanName.ToLower().Contains(planName.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(healthGoal))
            {
                mealPlans = mealPlans.Where(x => x.HealthGoal.ToLower().Contains(healthGoal.ToLower())).ToList();
            }
            if (userID.HasValue)
            {
                mealPlans = mealPlans.Where(x => x.UserId == userID.Value).ToList();
            }
            var mealPlanResponse = mealPlans.Select(x=> new MealPlanResponse
            {
                PlanName = x.PlanName,
                HealthGoal = x.HealthGoal,
                Duration = x.Duration,
                Status = x.Status,
                UserName = x.User?.FullName,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt
            }).ToList();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, mealPlanResponse);
        }

        public async Task<IBusinessResult> CreateMealPlan(MealPlanRequest mealPlanRequest)
        {
            var userid = int.Parse(_userIdClaim);

            var existedUser = await _unitOfWork.UserRepository.GetByIdAsync(userid);
            if(existedUser == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND,"User is not found");
            }
            var mealPlan = new MealPlan
            {
                UserId = existedUser.UserId,
                PlanName = mealPlanRequest.PlanName,
                HealthGoal = mealPlanRequest.HealthGoal,
                Status = "Chưa sử dụng",
                CreatedBy = existedUser.FullName,
                CreatedAt = DateTime.Now,
                UpdatedBy = existedUser.FullName,
                UpdatedAt = DateTime.Now
            };
            
            await _unitOfWork.BeginTransaction();
            try
            {
                await _unitOfWork.MealPlanRepository.AddAsync(mealPlan);
                await _unitOfWork.SaveChangesAsync();
                var mealDetail = _unitOfWork.MealPlanDetailRepository.GetAll();
                if(mealPlanRequest.MealPlanDetails != null && mealPlanRequest.MealPlanDetails.Any())
                {
                    var mealPlanDetails = new List<MealPlanDetail>();
                    var dayNumberSet = new HashSet<int>();
                    var caloriesByDay = new Dictionary<int, double?>();
                    foreach (var detail in  mealPlanRequest.MealPlanDetails)
                    {
                        var foodExist = await _unitOfWork.FoodRepository.GetByIdAsync(detail.FoodId);
                        if (foodExist == null)
                        {
                            return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Food is not found");
                        }

                        dayNumberSet.Add(detail.DayNumber);

                        if (!caloriesByDay.ContainsKey(detail.DayNumber))
                        {
                            caloriesByDay[detail.DayNumber] = 0;
                        }
                        caloriesByDay[detail.DayNumber] += (foodExist.Calories) * (detail.Quantity ?? 1);

                        var mealPlanDetail = detail.Adapt<MealPlanDetail>();
                        mealPlanDetail.MealPlanId = mealPlan.MealPlanId;
                        mealPlanDetail.FoodName = foodExist.FoodName;
                        mealPlanDetails.Add(mealPlanDetail);
                    }
                    foreach(var detail in mealPlanDetails)
                    {
                        detail.TotalCalories = caloriesByDay[detail.DayNumber];
                    }

                    await _unitOfWork.MealPlanDetailRepository.AddRangeAsync(mealPlanDetails);

                    mealPlan.Duration = dayNumberSet.Count;
                    await _unitOfWork.MealPlanRepository.UpdateAsync(mealPlan);
                    await _unitOfWork.SaveChangesAsync();
                    _unitOfWork.CommitTransaction();
                }
                    return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_CREATE_MSG);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                throw ex;
            }
        }

        public async Task DeleteMealPlan(int id)
        {
            var mealPlanExisted = await _unitOfWork.MealPlanRepository.GetByIdAsync(id);
            _unitOfWork.MealPlanRepository.DeleteAsync(mealPlanExisted);
        }

        public async Task ChangStatusMealPlan(int id, string status)
        {
            var mealPlanExisted = await _unitOfWork.MealPlanRepository.GetByIdAsync(id);
            mealPlanExisted.Status = status;
            await _unitOfWork.MealPlanRepository.UpdateAsync(mealPlanExisted);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
