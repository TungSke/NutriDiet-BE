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
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using NutriDiet.Service.ModelDTOs.Response;
using NutriDiet.Service.Utilities;
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

            await _unitOfWork.BeginTransaction();
            try
            {
                await _unitOfWork.MealPlanRepository.AddAsync(mealPlan);
                await _unitOfWork.SaveChangesAsync();
                if (mealPlanRequest.MealPlanDetails != null && mealPlanRequest.MealPlanDetails.Any())
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

                var mealplanResponse = mealPlan.Adapt<MealPlanResponse>();

                return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_CREATE_MSG, mealplanResponse);
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
                .Include(x => x.MealPlanDetails)
                .ThenInclude(x => x.Food)
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
            await _unitOfWork.BeginTransaction();
            try
            {
                mealPlan.PlanName = mealPlanRequest.PlanName;
                mealPlan.HealthGoal = mealPlanRequest.HealthGoal;
                mealPlan.UpdatedBy = _userIdClaim;
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



        public async Task<IBusinessResult> CreateSuitableMealPlanByAI()
        {
            var userid = int.Parse(_userIdClaim);

            var userInfo = await _unitOfWork.UserRepository.GetByWhere(x => x.UserId == userid)
                                                       .Include(x => x.GeneralHealthProfiles)
                                                       .Include(x => x.HealthcareIndicators)
                                                       .Include(x => x.PersonalGoals)
                                                       .Include(x => x.UserFoodPreferences)
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

            var allergyNames = userInfo?.Allergies.Select(x => x.AllergyName) ?? new List<string>();
            var diseaseNames = userInfo?.Diseases.Select(x => x.DiseaseName) ?? new List<string>();

            var formattedAllergies = allergyNames.Any() ? string.Join(", ", allergyNames) : "không có";
            var formattedDiseases = diseaseNames.Any() ? string.Join(", ", diseaseNames) : "không có";

            var foodListText = JsonSerializer.Serialize(foods);

            var airecommendationResponse = await _unitOfWork.AIRecommendationRepository
                        .GetByWhere(x => x.Status.ToLower() == AIRecommendStatus.Pending.ToString().ToLower())
                        .FirstOrDefaultAsync();

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

            var mealPlanRequesttest = new MealPlanRequest
            {
                PlanName = "kế hoạch ăn (bệnh lý user) của (userEmail)",
                HealthGoal = "string",
                MealPlanDetails = new List<MealPlanDetailRequest>
                {
                    new MealPlanDetailRequest
                    {
                        FoodId = 0,
                        Quantity = 1,
                        MealType = "string",
                        DayNumber = 0
                    }
                }
            };

            string jsonOutputSample = JsonSerializer.Serialize(mealPlanRequesttest);

            //var input = "Bạn là một chuyên gia dinh dưỡng, nhiệm vụ của bạn là tạo một kế hoạch bữa ăn (MealPlan) phù hợp với mục tiêu và điều kiện sức khỏe của người dùng \n" +
            //    "Tôi cần một kế hoạch bữa ăn cá nhân hóa theo các thông tin sau: \n" +
            //    $"- **Người dùng**:{userInfo.FullName}, {userInfo.Email}, giới tính: {userInfo.Gender}, {userInfo.Age} tuổi, cao {userInfo.GeneralHealthProfiles.Select(x => x.Height).FirstOrDefault()}cm, nặng {userInfo.GeneralHealthProfiles.Select(x => x.Weight).FirstOrDefault()}kg \n" +
            //    $"- **Mức độ vận động**: {userInfo.GeneralHealthProfiles.Select(x => x.ActivityLevel).FirstOrDefault()} \n" +
            //    $"- **Mục tiêu**: ({userInfo.PersonalGoals.Select(x => x.GoalType).FirstOrDefault()}) từ {userInfo.PersonalGoals.Select(x => x.StartDate.Value.Date).FirstOrDefault()} đền {userInfo.PersonalGoals.Select(x => x.TargetDate.Date).FirstOrDefault()} \n" +
            //    "Yêu cầu cụ thể:\n" +
            //    "- Tạo thực đơn 7 ngày" +
            //    "- Mỗi ngày bao gồm **3 bữa chính (Breakfast, Lunch, Dinner).\n" +
            //    $"- Chỉ chọn thực phẩm từ danh sách sau: {foodListText} \n" +
            //    $"- Tôi bị các loại dị ứng: {formattedAllergies}\n" +
            //    $"- Thực đơn phù hợp với các bệnh lý: {formattedDiseases} \n"+
            //    "- **Giá trị dinh dưỡng mỗi ngày:** "+
            //    $"- DailyCalories: {userInfo.PersonalGoals.Select(x => x.DailyCalories).FirstOrDefault()},DailyCarb: {userInfo.PersonalGoals.Select(x => x.DailyCarb).FirstOrDefault()}, DailyFat: {userInfo.PersonalGoals.Select(x => x.DailyCalories).FirstOrDefault()}, DailyProtein: {userInfo.PersonalGoals.Select(x => x.DailyCalories).FirstOrDefault()}.\n" +
            //    "Lưu ý: Chỉ trả về JSON, không kèm theo giải thích.\r\nMỗi ngày có 3 bữa chính, không thêm bữa phụ.";
            //    //$"Lưu ý: trước đó tôi đã từ chối mealplan với lý do: {rejectMealplanReason.RejectionReason.IsNullOrEmpty() ?? "không có"}";

            var input = $@"
                        Bạn là một chuyên gia dinh dưỡng. Nhiệm vụ của bạn là tạo một Meal Plan phù hợp với mục tiêu và điều kiện sức khỏe của người dùng.

                        Thông tin người dùng:
                        - **Họ tên:** {userInfo.FullName}
                        - **Email:** {userInfo.Email}
                        - **Giới tính:** {userInfo.Gender}
                        - **Tuổi:** {userInfo.Age}
                        - **Chiều cao:** {height} cm
                        - **Cân nặng:** {weight} kg
                        - **Mức độ vận động:** {activityLevel}
                        - **Mục tiêu:** {goalType} ({startDate} - {targetDate})

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
                        - Trước đó tôi đã từ chối một Meal Plan với lý do: {rejectionText}
                        - Chỉ trả về **JSON thuần túy**, không kèm theo giải thích.
                        ";

            var airesponse = await _aIGeneratorService.AIResponseJson(input, jsonOutputSample);

            if (string.IsNullOrEmpty(airesponse))
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Invalid format");
            }
            try
            {
                var mealPlanRequest = JsonSerializer.Deserialize<MealPlanRequest>(airesponse);

                if (airecommendationResponse == null)
                {
                    await _unitOfWork.AIRecommendationRepository.AddAsync(new Airecommendation
                    {
                        AirecommendationResponse = airesponse,
                        RecommendedAt = DateTime.Now,
                        Status = AIRecommendStatus.Pending.ToString()
                    });
                }
                else
                {
                    airecommendationResponse.AirecommendationResponse = airesponse;
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
            var recommendResponse = await _unitOfWork.AIRecommendationRepository.GetByWhere(x => x.Status.ToLower() == AIRecommendStatus.Pending.ToString().ToLower()).FirstOrDefaultAsync();
            if (recommendResponse == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "not found");
            }
            recommendResponse.RejectionReason = rejectReason;
            await _unitOfWork.SaveChangesAsync();

            var response = await CreateSuitableMealPlanByAI();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_UPDATE_MSG, response.Data);
        }

        public async Task<IBusinessResult> SaveMealPlanAI()
        {
            int userid = int.Parse(_userIdClaim);
            var recommendResponse = await _unitOfWork.AIRecommendationRepository.GetByWhere(x=>x.Status.ToLower() == AIRecommendStatus.Pending.ToString().ToLower()).FirstOrDefaultAsync();
            if (recommendResponse == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "not found");
            }
            recommendResponse.Status = AIRecommendStatus.Accepted.ToString();
            recommendResponse.RejectionReason = null; 
            await _unitOfWork.SaveChangesAsync();

            var mealPlanRequest = JsonSerializer.Deserialize<MealPlanRequest>(recommendResponse.AirecommendationResponse.ToString());

            var response = await CreateMealPlan(mealPlanRequest);

            if (response.Data is MealPlanResponse createdMealPlan)
            {
                recommendResponse.MealPlanId = createdMealPlan.MealPlanId;
                await _unitOfWork.SaveChangesAsync();
            }

            return new BusinessResult(Const.HTTP_STATUS_OK, "Save mealplan by AI Success");
        }

        public async Task<IBusinessResult> GetMyMealPlan()
        {
            int userid = int.Parse(_userIdClaim);
            var mealPlans = await _unitOfWork.MealPlanRepository.GetByWhere(x=>x.UserId == userid).ToListAsync();
            if (mealPlans == null || !mealPlans.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
            }

            var response = mealPlans.Adapt<List<MealPlanResponse>>();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> ApplyMealPlan(int mealPlanId)
        {
            var mealPlan = await _unitOfWork.MealPlanRepository.GetByIdAsync(mealPlanId);
            if (mealPlan == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "not found");
            }
            if(mealPlan.Status == MealplanStatus.Inactive.ToString() && mealPlan.StartAt == null)
            {
            mealPlan.StartAt = DateTime.Now;
            mealPlan.Status = MealplanStatus.Active.ToString();
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
            x => x.CreatedBy.ToLower() == "admin" && x.Status.ToLower() == MealplanStatus.Active.ToString().ToLower() &&
                      (string.IsNullOrEmpty(search) || x.PlanName.ToLower().Contains(search)
                                                   || x.HealthGoal.ToLower().Contains(search)));

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
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "No active meal plan found.");
            }

            var response = currentMealPlan.Adapt<MealPlanResponse>();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }
    }
}
