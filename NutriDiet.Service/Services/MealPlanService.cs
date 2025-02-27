using Google.Apis.Drive.v3.Data;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
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

        public async Task<IBusinessResult> SearchMealPlan(int pageIndex, int pageSize, string? status, string? search)
        {
            search = search?.ToLower() ?? string.Empty;

            var mealPlans = await _unitOfWork.MealPlanRepository.GetPagedAsync(
                pageIndex,
                pageSize,
                x => (string.IsNullOrEmpty(status) || x.Status.ToLower() == status.ToLower()) &&
                      (string.IsNullOrEmpty(search) || x.PlanName.ToLower().Contains(search)
                                                   || x.HealthGoal.ToLower().Contains(search)));


            if (mealPlans == null || !mealPlans.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
            }

            var response = mealPlans.Adapt<List<MealPlanResponse>>();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
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
                    var carbsByDay = new Dictionary<int, double?>();
                    var fatByDay = new Dictionary<int, double?>();
                    var proteinByDay = new Dictionary<int, double?>();
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

                        if (!carbsByDay.ContainsKey(detail.DayNumber))
                        {
                            carbsByDay[detail.DayNumber] = 0;
                        }
                        carbsByDay[detail.DayNumber] += (foodExist.Carbs) * (detail.Quantity ?? 1);

                        if (!fatByDay.ContainsKey(detail.DayNumber))
                        {
                            fatByDay[detail.DayNumber] = 0;
                        }
                        fatByDay[detail.DayNumber] += (foodExist.Fat) * (detail.Quantity ?? 1);

                        if (!proteinByDay.ContainsKey(detail.DayNumber))
                        {
                            proteinByDay[detail.DayNumber] = 0;
                        }
                        proteinByDay[detail.DayNumber] += (foodExist.Protein) * (detail.Quantity ?? 1);

                        var mealPlanDetail = detail.Adapt<MealPlanDetail>();
                        mealPlanDetail.MealPlanId = mealPlan.MealPlanId;
                        mealPlanDetail.FoodName = foodExist.FoodName;
                        mealPlanDetails.Add(mealPlanDetail);
                    }
                    foreach(var detail in mealPlanDetails)
                    {
                        detail.TotalCalories = caloriesByDay[detail.DayNumber];
                        detail.TotalCarbs = carbsByDay[detail.DayNumber];
                        detail.TotalFat = fatByDay[detail.DayNumber];
                        detail.TotalProtein = proteinByDay[detail.DayNumber];
                    }

                    await _unitOfWork.MealPlanDetailRepository.AddRangeAsync(mealPlanDetails);

                    mealPlan.Duration = dayNumberSet.Count;
                    await _unitOfWork.MealPlanRepository.UpdateAsync(mealPlan);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransaction();
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
            await _unitOfWork.MealPlanRepository.DeleteAsync(mealPlanExisted);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ChangStatusMealPlan(int id, string status)
        {
            var mealPlanExisted = await _unitOfWork.MealPlanRepository.GetByIdAsync(id);
            mealPlanExisted.Status = status;
            await _unitOfWork.MealPlanRepository.UpdateAsync(mealPlanExisted);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IBusinessResult> GetMealPlanByID(int mealPlanId)
        {
            var mealPlan = _unitOfWork.MealPlanRepository
                .GetAll()
                .Include(x=>x.MealPlanDetails)
                .ThenInclude(x=>x.Food)
                .FirstOrDefault(x => x.MealPlanId == mealPlanId);
          
            var response = mealPlan.Adapt<MealPlanResponse>();
            response.MealPlanDetails = mealPlan.MealPlanDetails.Select(m => new MealPlanDetailResponse
            {
                MealPlanDetailId = m.MealPlanDetailId,
                FoodName = m.Food.FoodName,
                Quantity = m.Quantity,
                MealType = m.MealType,
                DayNumber = m.DayNumber,
                TotalCalories = m.TotalCalories,
                TotalCarbs = m.TotalCarbs,
                TotalFat = m.TotalFat,
                TotalProtein = m.TotalProtein
            }).ToList();
            
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> GetMealPlanDetailByMealPlanID(int mealPlanId)
        {
            var mealPlanDetail = await _unitOfWork.MealPlanDetailRepository.GetByWhere(x=>x.MealPlanId == mealPlanId).ToListAsync();
            var response = mealPlanDetail.Adapt<List<MealPlanDetailResponse>>();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> CloneSampleMealPlan(int mealPlanID)
        {
            var userID = int.Parse("1");
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userID);
            var mealPlanExisted = await _unitOfWork.MealPlanRepository.GetByWhere(x => x.MealPlanId == mealPlanID).Include(x => x.MealPlanDetails).FirstOrDefaultAsync();

            var mealPlan = new MealPlan
            {
                PlanName = mealPlanExisted.PlanName + $" - Clone by {user.Email}",
                HealthGoal = mealPlanExisted.HealthGoal,
                Duration = mealPlanExisted.Duration,
                Status = MealplanStatus.Inactive.ToString(),
                CreatedBy = user.FullName,
                CreatedAt = DateTime.Now,
                UpdatedBy = user.FullName,
                UpdatedAt = DateTime.Now,
                MealPlanDetails = new List<MealPlanDetail>()
            };

            foreach (var detail in mealPlanExisted.MealPlanDetails)
            {
                mealPlan.MealPlanDetails.Add(new MealPlanDetail
                {
                    FoodId = detail.FoodId,
                    FoodName = detail.FoodName,
                    Quantity = detail.Quantity,
                    MealType = detail.MealType,
                    DayNumber = detail.DayNumber,
                    TotalCalories = detail.TotalCalories,
                    TotalCarbs = detail.TotalCarbs,
                    TotalFat = detail.TotalFat,
                    TotalProtein = detail.TotalProtein
                });
            }

            await _unitOfWork.MealPlanRepository.AddAsync(mealPlan);
            await _unitOfWork.SaveChangesAsync();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_CREATE_MSG);
        }

        public async Task<IBusinessResult> UpdateMealPlan(int mealPlanID, UpdateMealPlanRequest mealPlanRequest)
        {
            var mealPlan = await _unitOfWork.MealPlanRepository.GetByIdAsync(mealPlanID);
            if (mealPlan == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Meal Plan not found.");
            }

            mealPlan.PlanName = mealPlanRequest.PlanName;
            mealPlan.HealthGoal = mealPlanRequest.HealthGoal;
            mealPlan.UpdatedBy = _userIdClaim;
            mealPlan.UpdatedAt = DateTime.Now;

            await _unitOfWork.BeginTransaction();
            try
            {
                var existingDetails = _unitOfWork.MealPlanDetailRepository
                    .GetAll()
                    .Where(x => x.MealPlanId == mealPlan.MealPlanId)
                    .ToList();

                var updatedDetails = new List<MealPlanDetail>();
                var existingDetailIds = existingDetails.Select(x => x.MealPlanDetailId).ToHashSet();
                var requestDetailIds = mealPlanRequest.MealPlanDetails
                    .Where(x => x.MealPlanDetailId > 0)
                    .Select(x => x.MealPlanDetailId)
                    .ToHashSet();

                var dayNumberSet = new HashSet<int>();
                var caloriesByDay = new Dictionary<int, double?>();
                var carbsByDay = new Dictionary<int, double?>();
                var fatByDay = new Dictionary<int, double?>();
                var proteinByDay = new Dictionary<int, double?>();

                foreach (var detail in mealPlanRequest.MealPlanDetails)
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

                    if (!carbsByDay.ContainsKey(detail.DayNumber))
                    {
                        carbsByDay[detail.DayNumber] = 0;
                    }
                    carbsByDay[detail.DayNumber] += (foodExist.Carbs) * (detail.Quantity ?? 1);

                    if (!fatByDay.ContainsKey(detail.DayNumber))
                    {
                        fatByDay[detail.DayNumber] = 0;
                    }
                    fatByDay[detail.DayNumber] += (foodExist.Fat) * (detail.Quantity ?? 1);

                    if (!proteinByDay.ContainsKey(detail.DayNumber))
                    {
                        proteinByDay[detail.DayNumber] = 0;
                    }
                    proteinByDay[detail.DayNumber] += (foodExist.Protein) * (detail.Quantity ?? 1);

                    var existingDetail = existingDetails.FirstOrDefault(x => x.MealPlanDetailId == detail.MealPlanDetailId);
                    if (existingDetail != null)
                    {
                        existingDetail.FoodId = detail.FoodId;
                        existingDetail.FoodName = foodExist.FoodName;
                        existingDetail.Quantity = detail.Quantity;
                        existingDetail.MealType = detail.MealType;
                        existingDetail.DayNumber = detail.DayNumber;
                        existingDetail.TotalCalories = caloriesByDay[detail.DayNumber];
                        existingDetail.TotalCarbs = carbsByDay[detail.DayNumber];
                        existingDetail.TotalFat = fatByDay[detail.DayNumber];
                        existingDetail.TotalProtein = proteinByDay[detail.DayNumber];
                        updatedDetails.Add(existingDetail);
                    }
                    if(detail.MealPlanDetailId <= 0 || existingDetail == null)
                    {
                        var newDetail = new MealPlanDetail
                        {
                            MealPlanId = mealPlan.MealPlanId,
                            FoodId = detail.FoodId,
                            FoodName = foodExist.FoodName,
                            Quantity = detail.Quantity,
                            MealType = detail.MealType,
                            DayNumber = detail.DayNumber,
                            TotalCalories = caloriesByDay[detail.DayNumber],
                            TotalCarbs = carbsByDay[detail.DayNumber],
                            TotalFat = fatByDay[detail.DayNumber],
                            TotalProtein = proteinByDay[detail.DayNumber]
                        };
                        await _unitOfWork.MealPlanDetailRepository.AddAsync(newDetail);
                    }
                }

                // xóa nếu k có trong request
                var detailsToRemove = existingDetails
                    .Where(x => !requestDetailIds.Contains(x.MealPlanDetailId))
                    .ToList();
                if (detailsToRemove.Any())
                {
                    foreach (var detail in detailsToRemove)
                    {
                        await _unitOfWork.MealPlanDetailRepository.DeleteAsync(detail);
                    }
                }
                // tính lại các total theo từng bữa

                if (updatedDetails.Any())
                {
                    foreach (var detail in updatedDetails)
                    {
                        detail.TotalCalories = caloriesByDay[detail.DayNumber];
                        detail.TotalCarbs = carbsByDay[detail.DayNumber];
                        detail.TotalFat = fatByDay[detail.DayNumber];
                        detail.TotalProtein = proteinByDay[detail.DayNumber];
                        await _unitOfWork.MealPlanDetailRepository.UpdateAsync(detail);
                    }
                }

                mealPlan.Duration = dayNumberSet.Count;

                await _unitOfWork.MealPlanRepository.UpdateAsync(mealPlan);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

                return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_UPDATE_MSG);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                throw ex;
            }
        }

        public async Task<IBusinessResult> CreateSuitableMealPlanByAI()
        {
            //var userid = int.Parse(_userIdClaim);
            var userid = int.Parse("1");

            var userInfo = await _unitOfWork.UserRepository.GetByWhere(x => x.UserId == userid)
                                                       .Include(x => x.GeneralHealthProfiles)
                                                       .Include(x => x.HealthcareIndicators)
                                                       .Include(x => x.PersonalGoals)
                                                       .Include(x => x.UserFoodPreferences)
                                                       .FirstOrDefaultAsync();

            var userAllergyDisease = new
            {
                AllergyIds = userInfo.Allergies?.Select(a => a.AllergyId).ToList() ?? new List<int>(),
                DiseaseIds = userInfo.Diseases?.Select(d => d.DiseaseId).ToList() ?? new List<int>()
            };

            if (userAllergyDisease == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "User not found");
            }

            var allergyIds = new List<int>(userAllergyDisease.AllergyIds);
            var diseaseIds = new List<int>(userAllergyDisease.DiseaseIds);

            var likedFoodIds = userInfo.UserFoodPreferences
                            .Where(x => x.Preference.ToLower() == "like")
                            .Select(x => x.FoodId)
                            .ToList();

            var foods = await _unitOfWork.FoodRepository
            .GetByWhere(x =>
            likedFoodIds.Contains(x.FoodId) && // Chỉ lấy món thích
            !x.Allergies.Any(a => allergyIds.Contains(a.AllergyId)) &&
            !x.Diseases.Any(d => diseaseIds.Contains(d.DiseaseId)))
            .ToListAsync();

            var foodResponse = foods.Adapt<List<FoodResponse>>();

            var input = "Tôi cần một kế hoạch bữa ăn cá nhân hóa theo các thông tin sau: \n" +
                $"- **Người dùng**: {userInfo.Gender}, {userInfo.Age} tuổi, cao {userInfo.GeneralHealthProfiles.Select(x => x.Height).FirstOrDefault()}cm, nặng {userInfo.GeneralHealthProfiles.Select(x => x.Weight).FirstOrDefault()}kg \n" +
                $"- **Mục tiêu**: {userInfo.PersonalGoals.Select(x => x.GoalType).FirstOrDefault()} ({userInfo.PersonalGoals.Select(x => x.GoalType).FirstOrDefault()} từ {userInfo.PersonalGoals.Select(x => x.StartDate).FirstOrDefault()} đền {userInfo.PersonalGoals.Select(x => x.TargetDate).FirstOrDefault()}) \n" +
                $"- **Mức độ vận động**: {userInfo.GeneralHealthProfiles.Select(x => x.ActivityLevel).FirstOrDefault()} \n"  
                ;


            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_CREATE_MSG, input);
        }
    }
}
