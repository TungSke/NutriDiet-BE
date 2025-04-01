using Azure;
using CloudinaryDotNet;
using Google.Apis.Drive.v3.Data;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NutriDiet.Common;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Common.Enums;
using NutriDiet.Repository;
using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using NutriDiet.Service.Enums;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using NutriDiet.Service.ModelDTOs.Response;
using NutriDiet.Service.Utilities;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;

namespace NutriDiet.Service.Services
{
    public class MealPlanService : IMealPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AIGeneratorService _aIGeneratorService;
        private readonly string _userIdClaim;

        public MealPlanService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, AIGeneratorService aIGeneratorService)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _aIGeneratorService = aIGeneratorService;
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
                                                   || x.HealthGoal.ToLower().Contains(search)),
                q => q.OrderByDescending(x => x.CreatedAt)
                );


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
            if (existedUser == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "User is not found");
            }
            var mealPlan = new MealPlan
            {
                UserId = userid,
                PlanName = mealPlanRequest.PlanName,
                HealthGoal = mealPlanRequest.HealthGoal,
                Status = MealplanStatus.Inactive.ToString(),
                CreatedBy = existedUser.FullName,
                CreatedAt = DateTime.Now,
                UpdatedBy = existedUser.FullName,
                UpdatedAt = DateTime.Now
            };

            try
            {
                await _unitOfWork.MealPlanRepository.AddAsync(mealPlan);
                await _unitOfWork.SaveChangesAsync();
                if (mealPlanRequest.MealPlanDetails != null && mealPlanRequest.MealPlanDetails.Any())
                {
                    await _unitOfWork.BeginTransaction();
                    try
                    {

                        var mealPlanDetails = new List<MealPlanDetail>();
                        var dayNumberSet = new HashSet<int>();

                        foreach (var detail in mealPlanRequest.MealPlanDetails)
                        {
                            var foodExist = await _unitOfWork.FoodRepository.GetByIdAsync(detail.FoodId);
                            if (foodExist == null)
                            {
                                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Food is not found");
                            }

                            dayNumberSet.Add(detail.DayNumber);

                            var mealPlanDetail = detail.Adapt<MealPlanDetail>();
                            mealPlanDetail.TotalCalories = (foodExist.Calories) * (detail.Quantity ?? 1);
                            mealPlanDetail.TotalCarbs = (foodExist.Carbs) * (detail.Quantity ?? 1);
                            mealPlanDetail.TotalFat = (foodExist.Fat) * (detail.Quantity ?? 1);
                            mealPlanDetail.TotalProtein = (foodExist.Protein) * (detail.Quantity ?? 1);
                            mealPlanDetail.MealPlanId = mealPlan.MealPlanId;
                            mealPlanDetail.FoodName = foodExist.FoodName;
                            mealPlanDetails.Add(mealPlanDetail);
                        }

                        await _unitOfWork.MealPlanDetailRepository.AddRangeAsync(mealPlanDetails);

                        mealPlan.Duration = dayNumberSet.Count;
                        await _unitOfWork.MealPlanRepository.UpdateAsync(mealPlan);
                        await _unitOfWork.SaveChangesAsync();
                        await _unitOfWork.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        await _unitOfWork.RollbackTransaction();
                        throw ex;
                    }
                }
                var mealplanResponse = mealPlan.Adapt<MealPlanResponse>();

                return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_CREATE_MSG, mealplanResponse);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteMealPlan(int id)
        {
            var mealPlanExisted = await _unitOfWork.MealPlanRepository.GetByIdAsync(id);

            var userId = int.Parse(_userIdClaim);

            var mealPlanAI = _unitOfWork.AIRecommendationRepository.GetByWhere(x => x.MealPlanId == id).FirstOrDefault();
            if (mealPlanAI != null)
            {
                await _unitOfWork.AIRecommendationRepository.DeleteAsync(mealPlanAI);
            }

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
                .Include(x => x.MealPlanDetails)
                .ThenInclude(x => x.Food)
                .FirstOrDefault(x => x.MealPlanId == mealPlanId);

            var response = mealPlan.Adapt<MealPlanResponse>();
            response.MealPlanDetails = mealPlan.MealPlanDetails.Select(m => new MealPlanDetailResponse
            {
                MealPlanDetailId = m.MealPlanDetailId,
                FoodId = m.FoodId,
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
            var mealPlanDetail = await _unitOfWork.MealPlanDetailRepository.GetByWhere(x => x.MealPlanId == mealPlanId).ToListAsync();
            var response = mealPlanDetail.Adapt<List<MealPlanDetailResponse>>();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> CloneSampleMealPlan(int mealPlanID)
        {
            var userID = int.Parse(_userIdClaim);
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userID);
            var mealPlanExisted = await _unitOfWork.MealPlanRepository.GetByWhere(x => x.MealPlanId == mealPlanID).Include(x => x.MealPlanDetails).FirstOrDefaultAsync();

            var mealPlan = new MealPlan
            {
                UserId = userID,
                PlanName = mealPlanExisted.PlanName + $" - Clone by {user.FullName}",
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
            var userID = int.Parse(_userIdClaim);
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userID);

            var mealPlan = await _unitOfWork.MealPlanRepository.GetByIdAsync(mealPlanID);
            if (mealPlan == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Meal Plan not found.");
            }
            await _unitOfWork.BeginTransaction();
            try
            {
                mealPlan.PlanName = mealPlanRequest.PlanName;
                mealPlan.HealthGoal = mealPlanRequest.HealthGoal;
                mealPlan.UpdatedBy = user.FullName;
                mealPlan.UpdatedAt = DateTime.Now;

                var existingDetails = await _unitOfWork.MealPlanDetailRepository
                    .GetAll()
                    .Where(d => d.MealPlanId == mealPlanID)
                    .ToListAsync();

                var updatedDetails = new List<MealPlanDetail>();
                var newDetails = new List<MealPlanDetail>();

                foreach (var detail in mealPlanRequest.MealPlanDetails)
                {
                    var foodExist = await _unitOfWork.FoodRepository.GetByIdAsync(detail.FoodId);
                    if (foodExist == null)
                    {
                        return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Food is not found");
                    }

                    var existingDetail = existingDetails.FirstOrDefault(x => x.MealPlanDetailId == detail.MealPlanDetailId);
                    if (existingDetail != null)
                    {
                        existingDetail.FoodId = detail.FoodId;
                        existingDetail.FoodName = foodExist.FoodName;
                        existingDetail.Quantity = detail.Quantity;
                        existingDetail.DayNumber = detail.DayNumber;
                        existingDetail.MealType = detail.MealType;
                        existingDetail.TotalCalories = foodExist.Calories * (detail.Quantity ?? 1);
                        existingDetail.TotalCarbs = foodExist.Carbs * (detail.Quantity ?? 1);
                        existingDetail.TotalFat = foodExist.Fat * (detail.Quantity ?? 1);
                        existingDetail.TotalProtein = foodExist.Protein * (detail.Quantity ?? 1);
                        existingDetail.MealPlanId = mealPlan.MealPlanId;
                        updatedDetails.Add(existingDetail);
                    }
                    else
                    {
                        var newDetail = new MealPlanDetail
                        {
                            FoodId = detail.FoodId,
                            Quantity = detail.Quantity,
                            MealType = detail.MealType,
                            TotalCalories = foodExist.Calories * (detail.Quantity ?? 1),
                            TotalCarbs = foodExist.Carbs * (detail.Quantity ?? 1),
                            TotalFat = foodExist.Fat * (detail.Quantity ?? 1),
                            TotalProtein = foodExist.Protein * (detail.Quantity ?? 1),
                            MealPlanId = mealPlan.MealPlanId,
                            DayNumber = detail.DayNumber,
                            FoodName = foodExist.FoodName
                        };
                        newDetails.Add(newDetail);
                    }
                }

                var requestDetailIds = mealPlanRequest.MealPlanDetails
                    .Select(d => d.MealPlanDetailId)
                    .ToList();

                var detailsToDelete = existingDetails
                    .Where(d => !requestDetailIds.Contains(d.MealPlanDetailId))
                    .ToList();
                if (newDetails.Any())
                {
                    await _unitOfWork.MealPlanDetailRepository.AddRangeAsync(newDetails);
                }

                if (updatedDetails.Any())
                {
                    await _unitOfWork.MealPlanDetailRepository.UpdateRangeAsync(updatedDetails);
                }

                if (detailsToDelete.Any())
                {
                    await _unitOfWork.MealPlanDetailRepository.RemoveRange(detailsToDelete);
                }
                await _unitOfWork.SaveChangesAsync();

                var allDetails = await _unitOfWork.MealPlanDetailRepository
                    .GetAll()
                    .Where(d => d.MealPlanId == mealPlanID)
                    .ToListAsync();

                mealPlan.Duration = allDetails.Select(d => d.DayNumber).Distinct().Count();


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

        public async Task<IBusinessResult> UpdateMealPlanMobile(int mealPlanID, string planName, string healthGoal)
        {
            var userID = int.Parse(_userIdClaim);
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userID);

            var mealPlan = await _unitOfWork.MealPlanRepository.GetByIdAsync(mealPlanID);
            if (mealPlan == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Meal Plan not found.");
            }
            mealPlan.PlanName = planName;
            mealPlan.HealthGoal = healthGoal;
            mealPlan.UpdatedBy = user.FullName;
            mealPlan.UpdatedAt = DateTime.Now;
            await _unitOfWork.MealPlanRepository.UpdateAsync(mealPlan);
            await _unitOfWork.SaveChangesAsync();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_UPDATE_MSG);
        }

        public async Task<IBusinessResult> CreateSuitableMealPlanByAI()
        {

            var userid = int.Parse(_userIdClaim);

            var isPremium = await _unitOfWork.UserPackageRepository.IsUserPremiumAsync(userid);
            if (!isPremium)
            {
                return new BusinessResult(Const.HTTP_STATUS_FORBIDDEN, "Chỉ tài khoản Premium mới sử dụng được tính năng này");
            }

            // Lấy thông tin sức khỏe, mục tiêu cá nhân, thói quen ăn uống, dị ứng, bệnh lý của user
            var userInfo = await _unitOfWork.UserRepository.GetByWhere(x => x.UserId == userid)
                                                       .Include(x => x.GeneralHealthProfiles)
                                                       .ThenInclude(x => x.HealthcareIndicators)
                                                       .Include(x => x.PersonalGoals)
                                                       .Include(x => x.UserFoodPreferences)
                                                       .Include(x => x.UserIngreDientPreferences).ThenInclude(x => x.Ingredient)
                                                       .Include(x => x.Allergies)
                                                       .Include(x => x.Diseases)
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

            var foods = await _unitOfWork.FoodRepository.GetAll().ToListAsync();
            var foodResponse = foods.Adapt<List<FoodResponse>>();
            var foodListText = JsonSerializer.Serialize(foodResponse);

            //dị ứng và bệnh tật
            var allergyNames = userInfo?.Allergies.Select(x => x.AllergyName) ?? new List<string>();
            var diseaseNames = userInfo?.Diseases.Select(x => x.DiseaseName) ?? new List<string>();

            var formattedAllergies = allergyNames.Any() ? string.Join(", ", allergyNames) : "không có";
            var formattedDiseases = diseaseNames.Any() ? string.Join(", ", diseaseNames) : "không có";



            var airecommendationResponse = await _unitOfWork.AIRecommendationRepository
                        .GetByWhere(x => x.Status.ToLower() == AIRecommendStatus.Pending.ToString().ToLower() && x.UserId == userid)
                        .FirstOrDefaultAsync();

            //hồ sơ sức khỏe
            var userProfile = userInfo.GeneralHealthProfiles.FirstOrDefault();
            var personalGoal = userInfo.PersonalGoals.FirstOrDefault();
            var height = userProfile?.Height ?? 0;
            var weight = userProfile?.Weight ?? 0;
            var activityLevel = userProfile?.ActivityLevel ?? "Không xác định";
            var goalType = personalGoal?.GoalType ?? "Không có mục tiêu";
            var startDate = personalGoal?.StartDate?.ToString("yyyy-MM-dd") ?? "Chưa đặt";
            var targetDate = personalGoal?.TargetDate.ToString("yyyy-MM-dd") ?? "Chưa đặt";
            var dailyCalories = personalGoal?.DailyCalories ?? 0;
            var dailyCarb = personalGoal?.DailyCarb ?? 0;
            var dailyFat = personalGoal?.DailyFat ?? 0;
            var dailyProtein = personalGoal?.DailyProtein ?? 0;

            var rejectionReason = airecommendationResponse?.RejectionReason;
            var rejectionText = string.IsNullOrEmpty(rejectionReason) ? "không có" : rejectionReason;

            //thành phần user thích hay ghét
            var userIngredientsReference = userInfo.UserIngreDientPreferences.Select(x => new
            {
                x.Ingredient.IngredientName,
                x.Level,
            }).ToList();

            string favoriteIngredientsFormatted = userIngredientsReference.Any() ? string.Join(", ", userIngredientsReference.Select(x => $"{x.IngredientName} ({x.Level})")) : "không có";

            var mealPlanRequesttest = new MealPlanRequest
            {
                PlanName = $"kế hoạch ăn của {userInfo.FullName} cho người ghét hành",
                HealthGoal = "string",
                MealPlanDetails = new List<MealPlanDetailRequest>
                {
                    new MealPlanDetailRequest
                    {
                        FoodId = 1,
                        Quantity = 1,
                        MealType = "Breakfast",
                        DayNumber = 1
                    },
                    new MealPlanDetailRequest
                    {
                        FoodId = 2,
                        Quantity = 1,
                        MealType = "Lunch",
                        DayNumber = 1
                    },
                    new MealPlanDetailRequest
                    {
                        FoodId = 3,
                        Quantity = 1,
                        MealType = "Dinner",
                        DayNumber = 1
                    }
                }
            };

            // Sample JSON output
            string jsonOutputSample = JsonSerializer.Serialize(mealPlanRequesttest);

            var input = $@"Bạn là một chuyên gia dinh dưỡng. Nhiệm vụ của bạn là tạo một Meal Plan phù hợp với mục tiêu và điều kiện sức khỏe của người dùng.

                        Thông tin người dùng:
                        - **Họ tên:** {userInfo.FullName}
                        - **Email:** {userInfo.Email}
                        - **Giới tính:** {userInfo.Gender}
                        - **Tuổi:** {userInfo.Age}
                        - **Chiều cao:** {height} cm
                        - **Cân nặng:** {weight} kg
                        - **Mức độ vận động:** {activityLevel}
                        - **Mục tiêu:** {goalType} ({startDate} - {targetDate})
                        - **Thành phần yêu thích:** {favoriteIngredientsFormatted}      

                        Yêu cầu cho Meal Plan:
                        - **Thực đơn 7 ngày** với 3 bữa chính mỗi ngày (Breakfast, Lunch, Dinner)
                        - **Chỉ chọn thực phẩm từ danh sách:** {foodListText}
                        - **Dị ứng thực phẩm:** {formattedAllergies}
                        - **Bệnh lý cần lưu ý:** {formattedDiseases}

                        Giá trị dinh dưỡng mỗi ngày:
                        - **Calories:** {dailyCalories}
                        - **Carb:** {dailyCarb}
                        - **Fat:** {dailyFat}
                        - **Protein:** {dailyProtein}

                        Lưu ý:
                        - Mức độ yêu thích là -1(ghét) 0(bình thường) 1(thích)
                        - Trước đó tôi đã từ chối một Meal Plan với lý do: {rejectionText}
                        - Chỉ trả về **JSON thuần túy**, không kèm theo giải thích.";



            // Xử lý dữ liệu đầu vào và gửi yêu cầu tạo Meal Plan phù hợp
            var airesponse = await _aIGeneratorService.AIResponseJson(input, jsonOutputSample);
            //return new BusinessResult(Const.HTTP_STATUS_CREATED, Const.SUCCESS_CREATE_MSG, airesponse);
            if (string.IsNullOrEmpty(airesponse))
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Invalid format");
            }
            try
            {
                var mealPlanRequest = JsonSerializer.Deserialize<MealPlanRequest>(airesponse);

                if (airecommendationResponse == null)
                {
                    await _unitOfWork.AIRecommendationRepository.AddAsync(new AirecommendMealPlan
                    {
                        UserId = userid,
                        AirecommendMealPlanResponse = airesponse,
                        RecommendedAt = DateTime.Now,
                        Status = AIRecommendStatus.Pending.ToString()
                    });
                }
                else
                {
                    airecommendationResponse.AirecommendMealPlanResponse = airesponse;
                    airecommendationResponse.RecommendedAt = DateTime.Now;
                    airecommendationResponse.Status = AIRecommendStatus.Pending.ToString();
                    await _unitOfWork.AIRecommendationRepository.UpdateAsync(airecommendationResponse);
                }

                await _unitOfWork.SaveChangesAsync();

                return new BusinessResult(Const.HTTP_STATUS_CREATED, Const.SUCCESS_CREATE_MSG, mealPlanRequest);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Failed: " + ex.Message);
            }
        }

        public async Task<IBusinessResult> RejectMealplan(string rejectReason)
        {
            int userid = int.Parse(_userIdClaim);

            var isPremium = await _unitOfWork.UserPackageRepository.IsUserPremiumAsync(userid);
            if (!isPremium)
            {
                return new BusinessResult(Const.HTTP_STATUS_FORBIDDEN, "Chỉ tài khoản Premium mới sử dụng được tính năng này");
            }

            var recommendResponse = await _unitOfWork.AIRecommendationRepository.GetByWhere(x => x.Status.ToLower() == AIRecommendStatus.Pending.ToString().ToLower() && x.UserId == userid).FirstOrDefaultAsync();
            if (recommendResponse == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "not found");
            }
            recommendResponse.RejectionReason = rejectReason;
            await _unitOfWork.SaveChangesAsync();

            var response = await CreateSuitableMealPlanByAI();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_UPDATE_MSG, response.Data);
        }

        public async Task<IBusinessResult> SaveMealPlanAI(string feedback)
        {
            int userid = int.Parse(_userIdClaim);

            var isPremium = await _unitOfWork.UserPackageRepository.IsUserPremiumAsync(userid);
            if (!isPremium)
            {
                return new BusinessResult(Const.HTTP_STATUS_FORBIDDEN, "Chỉ tài khoản Premium mới sử dụng được tính năng này");
            }

            var recommendResponse = await _unitOfWork.AIRecommendationRepository.GetByWhere(x => x.Status.ToLower() == AIRecommendStatus.Pending.ToString().ToLower() && x.UserId == userid).FirstOrDefaultAsync();
            if (recommendResponse == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "not found");
            }
            recommendResponse.Status = AIRecommendStatus.Accepted.ToString();
            recommendResponse.RejectionReason = null;
            recommendResponse.Feedback = feedback;
            await _unitOfWork.SaveChangesAsync();

            var mealPlanRequest = JsonSerializer.Deserialize<MealPlanRequest>(recommendResponse.AirecommendMealPlanResponse.ToString());

            var response = await CreateMealPlan(mealPlanRequest);

            if (response.Data is MealPlanResponse createdMealPlan)
            {
                recommendResponse.MealPlanId = createdMealPlan.MealPlanId;
                await _unitOfWork.SaveChangesAsync();
            }

            return new BusinessResult(Const.HTTP_STATUS_OK, "Save mealplan by AI Success");
        }

        public async Task<IBusinessResult> GetMyMealPlan(int pageIndex, int pageSize, string? search)
        {

            int userid = int.Parse(_userIdClaim);

            search = search?.ToLower() ?? string.Empty;

            var mealPlans = await _unitOfWork.MealPlanRepository.GetPagedAsync(
                pageIndex,
                pageSize,
                x => x.UserId == userid && (string.IsNullOrEmpty(search) || x.PlanName.ToLower().Contains(search)
                                                   || x.HealthGoal.ToLower().Contains(search)),
                q => q.OrderByDescending(x => x.CreatedAt)
                );

            if (mealPlans == null || !mealPlans.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
            }

            var response = mealPlans.Adapt<List<MealPlanResponse>>();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> ApplyMealPlan(int mealPlanId)
        {
            var mealPlan = await _unitOfWork.MealPlanRepository
                .GetByWhere(x => x.MealPlanId == mealPlanId)
                .Include(x => x.MealPlanDetails)
                .ThenInclude(x=>x.Food)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (mealPlan == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "not found");
            }

            int userId = int.Parse(_userIdClaim);

            var existingActiveMealPlan = _unitOfWork.MealPlanRepository.GetAll()
                 .Any(x => x.UserId == userId // any: check true false
                     && x.Status == MealplanStatus.Active.ToString()
                     && x.StartAt != null);

            if (existingActiveMealPlan && mealPlan.Status == MealplanStatus.Inactive.ToString())
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Bạn đã có một thực đơn đang được áp dụng");
            }


            if (mealPlan.Status == MealplanStatus.Inactive.ToString() && mealPlan.StartAt == null)
            {
                mealPlan.StartAt = DateTime.Now;
                mealPlan.Status = MealplanStatus.Active.ToString();

                //add meallog day by days
                var mealLogs = new List<MealLog>();

                foreach (var dayGroup in mealPlan.MealPlanDetails.GroupBy(d => d.DayNumber))
                {
                    var dayNumber = dayGroup.Key;
                    var logDate = DateTime.Now.AddDays(dayNumber - 1);

                    //xóa mealog đã tồn tại và ghi đè lên nó
                    var existingMealLog = await _unitOfWork.MealLogRepository.GetByWhere(
                                x => x.UserId == userId && x.LogDate.Value.Date == logDate.Date).AsNoTracking().FirstOrDefaultAsync();

                    if (existingMealLog != null)
                    {
                        // Xóa MealLogDetail và MealLog hiện có
                        await _unitOfWork.MealLogRepository.DeleteAsync(existingMealLog);
                    }

                    var mealLogDetails = dayGroup.Select(detail => new MealLogDetail
                    {
                        FoodId = detail.FoodId,
                        Quantity = detail.Quantity,
                        MealType = detail.MealType,
                        ServingSize = detail.Food.ServingSize,
                        Calories = detail.TotalCalories,
                        Carbs = detail.TotalCarbs,
                        Fat = detail.TotalFat,
                        Protein = detail.TotalProtein
                    }).ToList();

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

                    mealLogs.Add(mealLog);
                }

                await _unitOfWork.MealLogRepository.AddRangeAsync(mealLogs);
            }
            else
            {
                mealPlan.StartAt = null;
                mealPlan.Status = MealplanStatus.Inactive.ToString();
            }
            await _unitOfWork.MealPlanRepository.UpdateAsync(mealPlan);
            await _unitOfWork.SaveChangesAsync();
            return new BusinessResult(Const.HTTP_STATUS_CREATED, Const.SUCCESS_UPDATE_MSG);
        }

        public async Task<IBusinessResult> GetSampleMealPlan(int pageIndex, int pageSize, string? search)
        {
            search = search?.ToLower() ?? string.Empty;

            var mealPlans = await _unitOfWork.MealPlanRepository.GetPagedAsync(
                pageIndex,
            pageSize,
            x => x.UserId == 2 && x.CreatedBy.ToLower() == "admin" &&
                      (string.IsNullOrEmpty(search) || x.PlanName.ToLower().Contains(search)
                                                   || x.HealthGoal.ToLower().Contains(search)),
            q => q.OrderByDescending(x => x.CreatedAt));

            if (mealPlans == null || !mealPlans.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
            }

            var response = mealPlans.Adapt<List<MealPlanResponse>>();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> GetMyCurrentMealPlan()
        {
            int userid = int.Parse(_userIdClaim);

            var currentMealPlan = await _unitOfWork.MealPlanRepository
                .GetByWhere(x => x.UserId == userid && x.Status == MealplanStatus.Active.ToString() && x.StartAt != null)
                .Include(x => x.MealPlanDetails)
                .ThenInclude(x => x.Food)
                .FirstOrDefaultAsync();

            if (currentMealPlan == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "No using meal plan found.");
            }

            var response = currentMealPlan.Adapt<MealPlanResponse>();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> GetFeedback(int pageIndex, int pageSize, string? search)
        {
            search = search?.ToLower() ?? string.Empty;

            var mealLogFeedback = await _unitOfWork.AIRecommendationMeallogRepository.GetPagedAsync(
                pageIndex,
                pageSize,
                predicate: log => log.Feedback != null && (string.IsNullOrEmpty(search) || log.User.FullName.ToLower().Contains(search)),
                x => x.OrderByDescending(x => x.RecommendedAt),
                include: log => log.Include(x => x.User)
                );

            var mealPlanFeedback = await _unitOfWork.AIRecommendationRepository.GetPagedAsync(
                pageIndex,
                pageSize,
                predicate: log => log.Feedback != null && (string.IsNullOrEmpty(search) || log.User.FullName.ToLower().Contains(search)),
                x => x.OrderByDescending(x => x.RecommendedAt),
                include: log => log.Include(x=>x.User)
                );

            var mealLogFeedbackResponse =  mealLogFeedback
                .Select(log => new FeedbackResponse
                {
                    Id = log.AirecommendMealLogId,
                    Type = "MealLog",
                    UserId = log.UserId,
                    FullName = log.User.FullName,
                    RecommendedAt = log.RecommendedAt,
                    Response = log.AirecommendMealLogResponse,
                    Status = log.Status,
                    RejectionReason = log.RejectionReason,
                    Feedback = log.Feedback
                })
                .ToList();

            var mealPlanFeedbackResponse = mealPlanFeedback
                .Select(plan => new FeedbackResponse
                {
                    Id = plan.AirecommendMealPlanId,
                    Type = "MealPlan",
                    UserId = plan.UserId,
                    FullName = plan.User.FullName,
                    RecommendedAt = plan.RecommendedAt,
                    Response = plan.AirecommendMealPlanResponse,
                    Status = plan.Status,
                    RejectionReason = plan.RejectionReason,
                    Feedback = plan.Feedback
                })
                .ToList();

            var allFeedback = mealLogFeedbackResponse
                .Concat(mealPlanFeedbackResponse)
                .OrderByDescending(f => f.RecommendedAt)
                .Take(pageSize)
                .ToList();

            if(allFeedback == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
            }

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, allFeedback);
        }
    }
}
