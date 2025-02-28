using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NutriDiet.Common;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Common.Enums;
using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using NutriDiet.Service.ModelDTOs.Response;
using NutriDiet.Service.Utilities;
using System.Security.Claims;
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

                await _unitOfWork.MealPlanRepository.UpdateAsync(mealPlan);
                await _unitOfWork.SaveChangesAsync();
                return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_UPDATE_MSG);
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

            var likedFoodIds = userInfo.UserFoodPreferences
                            .Where(x => x.Preference.ToLower() == "like")
                            .Select(x => x.FoodId)
                            .ToList();

            var foods = await _unitOfWork.FoodRepository
            .GetByWhere(x =>
            !x.Allergies.Any(a => allergyIds.Contains(a.AllergyId)) &&
            !x.Diseases.Any(d => diseaseIds.Contains(d.DiseaseId)))
            .ToListAsync();

            var allergyNames = userInfo?.Allergies.Select(x => x.AllergyName) ?? new List<string>();
            var diseaseNames = userInfo?.Diseases.Select(x => x.DiseaseName) ?? new List<string>();

            var formattedAllergies = allergyNames.Any() ? string.Join(", ", allergyNames) : "không có";
            var formattedDiseases = diseaseNames.Any() ? string.Join(", ", diseaseNames) : "không có";

            var foodListText = JsonSerializer.Serialize(foods);
            var jsonOuputSample = "{\r\n  \"planName\": \"kế hoạch ăn (bệnh lý user) của (userEmail) \",\r\n  \"healthGoal\": \"string\",\r\n  \"mealPlanDetails\": [\r\n    {\r\n      \"foodId\": 0,\r\n      \"quantity\": 1,\r\n      \"mealType\": \"string\",\r\n      \"dayNumber\": 0\r\n    }\r\n  ]\r\n}";

            var input = "Bạn là một chuyên gia dinh dưỡng, nhiệm vụ của bạn là tạo một kế hoạch bữa ăn (MealPlan) phù hợp với mục tiêu và điều kiện sức khỏe của người dùng \n" +
                "Tôi cần một kế hoạch bữa ăn cá nhân hóa theo các thông tin sau: \n" +
                $"- **Người dùng**:{userInfo.FullName}, {userInfo.Email}, giới tính: {userInfo.Gender}, {userInfo.Age} tuổi, cao {userInfo.GeneralHealthProfiles.Select(x => x.Height).FirstOrDefault()}cm, nặng {userInfo.GeneralHealthProfiles.Select(x => x.Weight).FirstOrDefault()}kg \n" +
                $"- **Mức độ vận động**: {userInfo.GeneralHealthProfiles.Select(x => x.ActivityLevel).FirstOrDefault()} \n" +
                $"- **Mục tiêu**: ({userInfo.PersonalGoals.Select(x => x.GoalType).FirstOrDefault()}) từ {userInfo.PersonalGoals.Select(x => x.StartDate.Value.Date).FirstOrDefault()} đền {userInfo.PersonalGoals.Select(x => x.TargetDate.Date).FirstOrDefault()} \n" +
                "Yêu cầu cụ thể:\n" +
                "- Tạo thực đơn 7 ngày" +
                "- Mỗi ngày bao gồm **3 bữa chính (Breakfast, Lunch, Dinner).\n" +
                $"- Chỉ chọn thực phẩm từ danh sách sau: {foodListText} \n" +
                $"- Tôi bị các loại dị ứng: {formattedAllergies}\n" +
                $"- Thực đơn phù hợp với các bệnh lý: {formattedDiseases} \n"+
                "- **Giá trị dinh dưỡng mỗi ngày:** "+
                $"- DailyCalories: {userInfo.PersonalGoals.Select(x => x.DailyCalories).FirstOrDefault()},DailyCarb: {userInfo.PersonalGoals.Select(x => x.DailyCarb).FirstOrDefault()}, DailyFat: {userInfo.PersonalGoals.Select(x => x.DailyCalories).FirstOrDefault()}, DailyProtein: {userInfo.PersonalGoals.Select(x => x.DailyCalories).FirstOrDefault()}.\n" +
                "Lưu ý: Chỉ trả về JSON, không kèm theo giải thích.\r\nMỗi ngày có 3 bữa chính, không thêm bữa phụ.";

            var airesponse = await _aIGeneratorService.AIResponseJson(input, jsonOuputSample);

            var AIjsonSampleOuput = "```json\n{\n  \"planName\": \"Kế hoạch ăn kiêng giảm cân cho người tiểu đường\",\n  \"healthGoal\": \"LoseWeight\",\n  \"mealPlanDetails\": [\n    {\n      \"foodId\": 22,\n      \"quantity\": 1,\n      \"mealType\": \"Breakfast\",\n      \"dayNumber\": 1\n    },\n    {\n      \"foodId\": 30,\n      \"quantity\": 1,\n      \"mealType\": \"Lunch\",\n      \"dayNumber\": 1\n    },\n    {\n      \"foodId\": 24,\n      \"quantity\": 1,\n      \"mealType\": \"Dinner\",\n      \"dayNumber\": 1\n    },\n    {\n      \"foodId\": 22,\n      \"quantity\": 1,\n      \"mealType\": \"Breakfast\",\n      \"dayNumber\": 2\n    },\n    {\n      \"foodId\": 21,\n      \"quantity\": 1,\n      \"mealType\": \"Lunch\",\n      \"dayNumber\": 2\n    },\n    {\n      \"foodId\": 10,\n      \"quantity\": 1,\n      \"mealType\": \"Dinner\",\n      \"dayNumber\": 2\n    },\n    {\n      \"foodId\": 26,\n      \"quantity\": 2,\n      \"mealType\": \"Breakfast\",\n      \"dayNumber\": 3\n    },\n    {\n      \"foodId\": 30,\n      \"quantity\": 1,\n      \"mealType\": \"Lunch\",\n      \"dayNumber\": 3\n    },\n    {\n      \"foodId\": 24,\n      \"quantity\": 1,\n      \"mealType\": \"Dinner\",\n      \"dayNumber\": 3\n    },\n    {\n      \"foodId\": 22,\n      \"quantity\": 1,\n      \"mealType\": \"Breakfast\",\n      \"dayNumber\": 4\n    },\n    {\n      \"foodId\": 21,\n      \"quantity\": 1,\n      \"mealType\": \"Lunch\",\n      \"dayNumber\": 4\n    },\n    {\n      \"foodId\": 10,\n      \"quantity\": 1,\n      \"mealType\": \"Dinner\",\n      \"dayNumber\": 4\n    },\n    {\n      \"foodId\": 26,\n      \"quantity\": 2,\n      \"mealType\": \"Breakfast\",\n      \"dayNumber\": 5\n    },\n    {\n      \"foodId\": 30,\n      \"quantity\": 1,\n      \"mealType\": \"Lunch\",\n      \"dayNumber\": 5\n    },\n    {\n      \"foodId\": 24,\n      \"quantity\": 1,\n      \"mealType\": \"Dinner\",\n      \"dayNumber\": 5\n    },\n    {\n      \"foodId\": 22,\n      \"quantity\": 1,\n      \"mealType\": \"Breakfast\",\n      \"dayNumber\": 6\n    },\n    {\n      \"foodId\": 21,\n      \"quantity\": 1,\n      \"mealType\": \"Lunch\",\n      \"dayNumber\": 6\n    },\n    {\n      \"foodId\": 10,\n      \"quantity\": 1,\n      \"mealType\": \"Dinner\",\n      \"dayNumber\": 6\n    },\n    {\n      \"foodId\": 26,\n      \"quantity\": 2,\n      \"mealType\": \"Breakfast\",\n      \"dayNumber\": 7\n    },\n    {\n      \"foodId\": 30,\n      \"quantity\": 1,\n      \"mealType\": \"Lunch\",\n      \"dayNumber\": 7\n    },\n    {\n      \"foodId\": 24,\n      \"quantity\": 1,\n      \"mealType\": \"Dinner\",\n      \"dayNumber\": 7\n    }\n  ]\n}\n```";

            if (string.IsNullOrEmpty(airesponse))
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Invalid format");
            }
            try
            {
                // Deserialize JSON thành đối tượng MealPlanRequest
                var mealPlanRequest = JsonSerializer.Deserialize<MealPlanRequest>(airesponse);

                await CreateMealPlan(mealPlanRequest);

                return new BusinessResult(Const.HTTP_STATUS_CREATED, Const.SUCCESS_CREATE_MSG, mealPlanRequest);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Failed: " + ex.Message);
            }
        }
    }
}
