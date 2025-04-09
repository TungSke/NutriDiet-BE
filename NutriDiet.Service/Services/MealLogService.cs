using Azure;
using Azure.Core;
using Google.Apis.Drive.v3.Data;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NutriDiet.Common;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Common.Enums;
using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using NutriDiet.Service.ModelDTOs.Response;
using NutriDiet.Service.Utilities;
using Sprache;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NutriDiet.Service.Services
{
    public class MealLogService : IMealLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AIGeneratorService _aiGeneratorService;
        private readonly string _userIdClaim;

        public MealLogService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, AIGeneratorService aIGeneratorService)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _aiGeneratorService = aIGeneratorService;
            _userIdClaim = GetUserIdClaim();
        }
        private string GetUserIdClaim()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }
        public async Task<IBusinessResult> AddOrUpdateMealLog(MealLogRequest request)
        {
            var userId = int.Parse(_userIdClaim);
            var existingUser = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (existingUser == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "User does not exist.", null);
            }

            if (request.LogDate == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "LogDate is required.", null);
            }

            var logDate = request.LogDate.Value.Date;

            // Kiểm tra MealLog theo ngày
            var existingMealLog = await _unitOfWork.MealLogRepository
                .GetByWhere(m => m.UserId == userId && m.LogDate.Value.Date == logDate)
                .Include(m => m.MealLogDetails)
                .FirstOrDefaultAsync();

            if (existingMealLog == null)
            {
                // Nếu chưa có MealLog, tạo mới
                existingMealLog = new MealLog
                {
                    UserId = userId,
                    LogDate = logDate,
                    TotalCalories = 0,
                    TotalCarbs = 0,
                    TotalFat = 0,
                    TotalProtein = 0,
                    MealLogDetails = new List<MealLogDetail>()
                };
            }

            // Kiểm tra xem món ăn có tồn tại không
            var food = await _unitOfWork.FoodRepository.GetByIdAsync(request.FoodId);
            if (food == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, $"Food with ID {request.FoodId} not found.", null);
            }

            double quantity = request.Quantity ?? 1;

            // Kiểm tra xem món ăn đã tồn tại trong MealLogDetail chưa
            var existingMealDetail = existingMealLog.MealLogDetails.FirstOrDefault(d => d.FoodId == request.FoodId);

            if (existingMealDetail != null)
            {
                // Nếu món ăn đã tồn tại, chỉ cập nhật số lượng và dinh dưỡng
                existingMealDetail.Quantity += quantity;
                existingMealDetail.Calories += quantity * food.Calories;
                existingMealDetail.Protein += quantity * food.Protein;
                existingMealDetail.Carbs += quantity * food.Carbs;
                existingMealDetail.Fat += quantity * food.Fat;
            }
            else
            {
                // Nếu món ăn chưa có, thêm mới vào MealLogDetail
                var mealLogDetail = new MealLogDetail
                {
                    FoodId = request.FoodId,
                    MealType = request.MealType.ToString(),
                    ServingSize = food.ServingSize,
                    Quantity = quantity,
                    Calories = quantity * food.Calories,
                    Protein = quantity * food.Protein,
                    Carbs = quantity * food.Carbs,
                    Fat = quantity * food.Fat
                };

                existingMealLog.MealLogDetails.Add(mealLogDetail);
            }

            // Cập nhật tổng dinh dưỡng cho MealLog
            existingMealLog.TotalCalories = existingMealLog.MealLogDetails.Sum(d => d.Calories);
            existingMealLog.TotalProtein = existingMealLog.MealLogDetails.Sum(d => d.Protein);
            existingMealLog.TotalCarbs = existingMealLog.MealLogDetails.Sum(d => d.Carbs);
            existingMealLog.TotalFat = existingMealLog.MealLogDetails.Sum(d => d.Fat);

            // Thêm mới hoặc cập nhật MealLog
            if (existingMealLog.MealLogId == 0)
            {
                await _unitOfWork.MealLogRepository.AddAsync(existingMealLog);
            }
            else
            {
                await _unitOfWork.MealLogRepository.UpdateAsync(existingMealLog);
            }

            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, "Meal log updated successfully.");
        }
        public async Task<IBusinessResult> RemoveMealLog(int mealLogId)
        {
            var userId = int.Parse(_userIdClaim);

            // Lấy MealLog theo ID và kiểm tra xem có thuộc về người dùng hiện tại không
            var mealLog = await _unitOfWork.MealLogRepository
                .GetByWhere(m => m.MealLogId == mealLogId && m.UserId == userId)
                .Include(m => m.MealLogDetails)
                .FirstOrDefaultAsync();

            if (mealLog == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Meal log not found.", null);
            }

            // Xóa toàn bộ meal log (bao gồm các chi tiết nếu có)
            await _unitOfWork.MealLogRepository.DeleteAsync(mealLog);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, "Meal log removed successfully.");
        }
        public async Task<bool> IsDailyCaloriesExceeded(DateTime? logDate, double additionalCalories)
        {
            var userId = int.Parse(_userIdClaim);
            var existingUser = await _unitOfWork.UserRepository.GetByWhere( u => u.UserId == userId).Include(u => u.PersonalGoals).FirstOrDefaultAsync();
            var personalgoal = existingUser?.PersonalGoals.OrderByDescending(p => p.CreatedAt).First();
            var meallog = await _unitOfWork.MealLogRepository.GetByWhere(u => u.UserId == userId && u.LogDate.Value.Date == logDate.Value.Date).FirstOrDefaultAsync();
            if (meallog == null) { return false; }
            if(meallog.TotalCalories > personalgoal?.DailyCalories)
            {
                return false;
            }
            if (meallog.TotalCalories + additionalCalories > personalgoal?.DailyCalories)
            {
                return true;
            } 
            return false;
        }
        public async Task<IBusinessResult> RemoveMealLogDetail(int mealLogId, int detailId)
        {
            var userId = int.Parse(_userIdClaim);

            // Lấy MealLog có chứa MealLogDetail cần xóa
            var mealLog = await _unitOfWork.MealLogRepository
                .GetByWhere(m => m.MealLogId == mealLogId && m.UserId == userId)
                .Include(m => m.MealLogDetails)
                .FirstOrDefaultAsync();

            if (mealLog == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Meal log not found.", null);
            }

            var mealLogDetail = mealLog.MealLogDetails.FirstOrDefault(md => md.DetailId == detailId);

            if (mealLogDetail == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, $"Meal Detail with ID {mealLogId} not found in meal log.", null);
            }

            // Trừ đi giá trị dinh dưỡng của món ăn sắp bị xóa
            mealLog.TotalCalories -= mealLogDetail.Calories;
            mealLog.TotalProtein -= mealLogDetail.Protein;
            mealLog.TotalCarbs -= mealLogDetail.Carbs;
            mealLog.TotalFat -= mealLogDetail.Fat;

            // Xóa MealLogDetail khỏi danh sách
            mealLog.MealLogDetails.Remove(mealLogDetail);

            // Nếu MealLog không còn chi tiết nào thì xóa luôn
            if (!mealLog.MealLogDetails.Any())
            {
                await _unitOfWork.MealLogRepository.DeleteAsync(mealLog);
            }
            else
            {
                // Cập nhật lại MealLog
                await _unitOfWork.MealLogRepository.UpdateAsync(mealLog);
            }

            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, "Meal log detail removed successfully.");
        }

        public async Task<IBusinessResult> GetMealLogsByDateRange(DateTime? logDate, DateTime? fromDate, DateTime? toDate)
        {
            var userId = int.Parse(_userIdClaim);
            var existingUser = await _unitOfWork.UserRepository
                .GetByWhere(u => u.UserId == userId)
                .Include(u => u.PersonalGoals.OrderByDescending(pg => pg.CreatedAt))
                .FirstOrDefaultAsync();

            if (existingUser == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "User not found.", null);
            }

            var personalGoal = existingUser.PersonalGoals.FirstOrDefault();
            double dailycalories = personalGoal?.DailyCalories ?? 2000;
            IQueryable<MealLog> query = _unitOfWork.MealLogRepository
                .GetByWhere(m => m.UserId == userId)
                .Include(m => m.MealLogDetails)
                .ThenInclude(d => d.Food); 

            if (logDate.HasValue)
            {
                query = query.Where(m => m.LogDate.Value.Date == logDate.Value.Date);
            }
            else if (fromDate.HasValue && toDate.HasValue && fromDate < toDate)
            {
                query = query.Where(m => m.LogDate.Value.Date >= fromDate.Value.Date && m.LogDate.Value.Date <= toDate.Value.Date);
            }
            else
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Invalid date parameters.", null);
            }

            var mealLogs = await query.ToListAsync();

            if (!mealLogs.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "No meal logs found.", null);
            }

            // 🔹 Map dữ liệu sang DTO (giống như `GetMealLogById`)
            var response = mealLogs.Select(mealLog => new MealLogResponse
            {
                MealLogId = mealLog.MealLogId,
                LogDate = mealLog.LogDate.Value,
                TotalCalories = mealLog.TotalCalories ?? 0,
                TotalProtein = mealLog.TotalProtein ?? 0,
                TotalCarbs = mealLog.TotalCarbs ?? 0,
                TotalFat = mealLog.TotalFat ?? 0,
                MealLogDetails = mealLog.MealLogDetails.Select(d => new MealLogDetailResponse
                {
                    DetailId = d.DetailId,
                    FoodName = d.Food?.FoodName ?? "Quick Add",
                    MealType = d.MealType,
                    ServingSize = d.ServingSize,
                    Quantity = d.Quantity,
                    Calories = d.Calories ?? 0,
                    Protein = d.Protein ?? 0,
                    Carbs = d.Carbs ?? 0,
                    Fat = d.Fat ?? 0,
                    ImageUrl = d.ImageUrl ?? "",
                }).ToList()
            }).ToList();

            return new BusinessResult(Const.HTTP_STATUS_OK, "Meal logs retrieved successfully.", response);
        }

        public async Task<IBusinessResult> QuickAddMealLogDetail(QuickMealLogRequest request)
        {
            var userId = int.Parse(_userIdClaim);

            if (request.LogDate == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "LogDate is required.", null);
            }

            var logDate = request.LogDate.Value.Date;

            var mealLog = await _unitOfWork.MealLogRepository
                .GetByWhere(m => m.UserId == userId && m.LogDate.Value.Date == logDate)
                .Include(m => m.MealLogDetails)
                .FirstOrDefaultAsync();

            if (mealLog == null)
            {
                mealLog = new MealLog
                {
                    UserId = userId,
                    LogDate = logDate,
                    TotalCalories = 0,
                    TotalCarbs = 0,
                    TotalFat = 0,
                    TotalProtein = 0,
                    MealLogDetails = new List<MealLogDetail>()
                };
                await _unitOfWork.MealLogRepository.AddAsync(mealLog);
                await _unitOfWork.SaveChangesAsync();
            }

            // Thêm nhanh một MealLogDetail mà không cần chọn món ăn
            var mealLogDetail = new MealLogDetail
            {
                FoodId = null,
                MealType = request.MealType.ToString(),
                Quantity = 1,
                Calories = request.Calories ?? 0,
                Protein = request.Protein ?? 0,
                Carbs = request.Carbohydrates ?? 0,
                Fat = request.Fats ?? 0
            };

            mealLog.MealLogDetails.Add(mealLogDetail);

            mealLog.TotalCalories += mealLogDetail.Calories;
            mealLog.TotalProtein += mealLogDetail.Protein;
            mealLog.TotalCarbs += mealLogDetail.Carbs;
            mealLog.TotalFat += mealLogDetail.Fat;

            await _unitOfWork.MealLogRepository.UpdateAsync(mealLog);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, "Quick meal log added successfully.");
        }

        public async Task<IBusinessResult> CopyMealLogDetails(CopyMealLogRequest request)
        {
            var userId = int.Parse(_userIdClaim);

            if (request.SourceDate == null || request.TargetDate == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "SourceDate and TargetDate are required.", null);
            }

            var sourceDate = request.SourceDate.Value.Date;
            var targetDate = request.TargetDate.Value.Date;

            if (sourceDate == targetDate)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Cannot copy to the same date.", null);
            }

            // Lấy MealLog của ngày nguồn
            var sourceMealLog = await _unitOfWork.MealLogRepository
                .GetByWhere(m => m.UserId == userId && m.LogDate == sourceDate)
                .Include(m => m.MealLogDetails)
                .FirstOrDefaultAsync();

            if (sourceMealLog == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "No meal log found for the source date.", null);
            }

            var sourceDetails = sourceMealLog.MealLogDetails.ToList();

            if (!sourceDetails.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "No meal log details found for the source date.", null);
            }

            var targetMealLog = await _unitOfWork.MealLogRepository
                .GetByWhere(m => m.UserId == userId && m.LogDate.Value.Date == targetDate)
                .Include(m => m.MealLogDetails)
                .FirstOrDefaultAsync();

            bool isNewMealLog = false;

            if (targetMealLog == null)
            {
                if (targetDate < DateTime.UtcNow.Date)
                {
                    return new BusinessResult(Const.HTTP_STATUS_OK, "No existing meal log found on the target date. Nothing to copy.");
                }

                targetMealLog = new MealLog
                {
                    UserId = userId,
                    LogDate = targetDate,
                    TotalCalories = 0,
                    TotalProtein = 0,
                    TotalCarbs = 0,
                    TotalFat = 0,
                    MealLogDetails = new List<MealLogDetail>()
                };

                await _unitOfWork.MealLogRepository.AddAsync(targetMealLog);
                await _unitOfWork.SaveChangesAsync(); // Lưu ngay để lấy MealLogId
                isNewMealLog = true;
            }

            // Copy toàn bộ các chi tiết từ nguồn
            foreach (var detail in sourceDetails)
            {
                var copiedDetail = new MealLogDetail
                {
                    MealLogId = targetMealLog.MealLogId,
                    FoodId = detail.FoodId,
                    MealType = detail.MealType,  // Giữ nguyên thông tin MealType của bữa ăn gốc
                    ServingSize = detail.ServingSize,
                    Quantity = detail.Quantity,
                    Calories = detail.Calories,
                    Protein = detail.Protein,
                    Carbs = detail.Carbs,
                    Fat = detail.Fat
                };

                targetMealLog.MealLogDetails.Add(copiedDetail);
            }

            targetMealLog.TotalCalories += sourceDetails.Sum(d => d.Calories);
            targetMealLog.TotalProtein += sourceDetails.Sum(d => d.Protein);
            targetMealLog.TotalCarbs += sourceDetails.Sum(d => d.Carbs);
            targetMealLog.TotalFat += sourceDetails.Sum(d => d.Fat);

            if (!isNewMealLog) 
            {
                await _unitOfWork.MealLogRepository.Attach(targetMealLog);
                await _unitOfWork.MealLogRepository.UpdateAsync(targetMealLog);
            }

            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, "Meal log details copied successfully.");
        }

        private double CalculateRecommendedValue(
            List<MealLog> mealLogsList,
            double targetValue,
            Func<MealLog, double> selector,
            double clampMin,
            double clampMax)
        {
            // Nhóm các MealLog theo ngày và tính tổng giá trị theo selector (ví dụ: TotalCalories) cho mỗi ngày
            var dailyValueDictionary = mealLogsList
                .GroupBy(m => m.LogDate.Value.Date)
                .ToDictionary(g => g.Key, g => g.Sum(selector));

            int computedDays = dailyValueDictionary.Count;           // Số ngày có dữ liệu meal log
            double sumValue = dailyValueDictionary.Values.Sum();       // Tổng giá trị thực tế của các ngày đó
            double recommendedValue = targetValue;                     // Mặc định nếu không có dữ liệu thì giữ nguyên mục tiêu

            if (computedDays > 0)
            {
                double totalTarget = targetValue * computedDays;
                double totalDifference = totalTarget - sumValue;
                double adjustment = totalDifference / computedDays;
                adjustment = Math.Clamp(adjustment, clampMin, clampMax);
                recommendedValue = targetValue + adjustment;
            }
            return recommendedValue;
        }

        public async Task<IBusinessResult> CreateMealLogAI()
        {
            var userId = int.Parse(_userIdClaim);
            var isPremiumResult = await _unitOfWork.UserPackageRepository.IsUserPremiumAsync(userId);
            if (!isPremiumResult)
            {
                return new BusinessResult(Const.HTTP_STATUS_FORBIDDEN, "Chỉ tài khoản Premium mới sử dụng được tính năng này");
            }

            var userInfo = await _unitOfWork.UserRepository.GetByWhere(x => x.UserId == userId)
                                                           .Include(x => x.GeneralHealthProfiles)
                                                           .Include(x => x.UserFoodPreferences)
                                                           .Include(x => x.UserIngreDientPreferences).ThenInclude(x => x.Ingredient)
                                                           .Include(x => x.PersonalGoals)
                                                           .Include(x => x.Allergies)
                                                           .Include(x => x.Diseases)
                                                           .FirstOrDefaultAsync();

            if (userInfo == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "User not found");
            }

            var allergyNames = userInfo.Allergies?.Select(x => x.AllergyName).ToList() ?? new List<string>();
            var diseaseNames = userInfo.Diseases?.Select(x => x.DiseaseName).ToList() ?? new List<string>();

            var formattedAllergies = allergyNames.Any() ? string.Join(", ", allergyNames) : "không có";
            var formattedDiseases = diseaseNames.Any() ? string.Join(", ", diseaseNames) : "không có";

            var userProfile = userInfo.GeneralHealthProfiles.FirstOrDefault();
            var personalGoal = userInfo.PersonalGoals.FirstOrDefault();

            var height = userProfile?.Height ?? 0;
            var weight = userProfile?.Weight ?? 0;
            var activityLevel = userProfile?.ActivityLevel ?? "Không xác định";
            var goalType = personalGoal?.GoalType ?? "Không có mục tiêu";
            // Các chỉ số mục tiêu được chuyển đổi sang kiểu double
            double targetDailyCalories = personalGoal?.DailyCalories ?? 0;
            double dailyCarb = personalGoal?.DailyCarb ?? 0;
            double dailyFat = personalGoal?.DailyFat ?? 0;
            double dailyProtein = personalGoal?.DailyProtein ?? 0;
            var dietStyle = userProfile?.DietStyle ?? "Không xác định";
            var today = DateTime.Now.Date;
            var userIngredientsReference = userInfo.UserIngreDientPreferences
                .Select(x => new
                {
                    x.Ingredient.IngredientName,
                    x.Level,
                })
                .ToList();

            string favoriteIngredientsFormatted = userIngredientsReference.Any()
                ? string.Join(", ", userIngredientsReference.Select(x => $"{x.IngredientName} ({x.Level})"))
                : "không có";

            // Lấy dữ liệu Meal Log 7 ngày gần nhất
            var mealLogs = await GetMealLogsByDateRange(null, DateTime.Now.AddDays(-7), DateTime.Now);
            var formattedMealLogs = JsonSerializer.Serialize(mealLogs.Data);

            var mealLogsList = mealLogs.Data as List<MealLog> ?? new List<MealLog>();

            // Sử dụng helper để tính toán các chỉ số dinh dưỡng cho hôm nay dựa vào dữ liệu các ngày có meal log
            double recommendedTodayCalories = CalculateRecommendedValue(
                mealLogsList,
                targetDailyCalories,
                m => (double)m.TotalCalories,
                -500, 500);

            double recommendedTodayProtein = CalculateRecommendedValue(
                mealLogsList,
                dailyProtein,
                m => (double)m.TotalProtein,
                -20, 20);

            double recommendedTodayCarb = CalculateRecommendedValue(
                mealLogsList,
                dailyCarb,
                m => (double)m.TotalCarbs,
                -50, 50);

            double recommendedTodayFat = CalculateRecommendedValue(
                mealLogsList,
                dailyFat,
                m => (double)m.TotalFat,
                -20, 20);

            // Lấy danh sách thực phẩm có thể ăn
            var foods = await _unitOfWork.FoodRepository.GetAll().ToListAsync();
            var foodResponse = foods.Adapt<List<FoodResponse>>();
            var foodListText = JsonSerializer.Serialize(foodResponse);

            var mealogrequestSample = new List<MealLogRequest>
    {
        new MealLogRequest
        {
            LogDate = DateTime.Now.Date,
            FoodId = 1,
            MealType = MealType.Breakfast,
            Quantity = 1,
            ServingSize = "1 tô"
        },
        new MealLogRequest
        {
            LogDate = DateTime.Now.Date,
            FoodId = 2,
            MealType = MealType.Lunch,
            Quantity = 1,
            ServingSize = "1 bát"
        },
        new MealLogRequest
        {
            LogDate = DateTime.Now.Date,
            FoodId = 3,
            MealType = MealType.Dinner,
            Quantity = 1,
            ServingSize = "1 phần"
        },
        new MealLogRequest
        {
            LogDate = DateTime.Now.Date,
            FoodId = 4,
            MealType = MealType.Snacks,
            Quantity = 1,
            ServingSize = "1 phần"
        }
    };

            var jsonSampleOutput = JsonSerializer.Serialize(mealogrequestSample);

            // Cập nhật chuỗi prompt cho AI, thay các giá trị mục tiêu ban đầu bởi giá trị đã điều chỉnh
            var input = $@"Bạn là một chuyên gia dinh dưỡng. Nhiệm vụ của bạn là tạo một Meal Log phù hợp với mục tiêu và điều kiện sức khỏe của người dùng cho ngày hôm nay {today}.

Thông tin người dùng:
- **Họ tên:** {userInfo.FullName}
- **Giới tính:** {userInfo.Gender}
- **Tuổi:** {userInfo.Age}
- **Chiều cao:** {height} cm
- **Cân nặng:** {weight} kg
- **Mức độ vận động:** {activityLevel}
- **Mục tiêu:** {goalType}
- Lưu ý quan trọng Phong cách ăn uống : **{dietStyle}**

Dữ liệu Meal Log 7 ngày gần nhất:
{formattedMealLogs}

Yêu cầu cho Meal Log ngày hôm nay:
- **Meal Log 1 ngày** với 3 bữa chính (Breakfast, Lunch, Dinner) và 1 bữa phụ (Snacks). Mỗi bữa có 1-2 món.
- **Chỉ chọn thực phẩm từ danh sách:** {foodListText}
- **Dị ứng thực phẩm:** {formattedAllergies}
- **Bệnh lý cần lưu ý:** {formattedDiseases}

Giá trị dinh dưỡng đề xuất cho user nạp đủ trong ngày hôm nay:
- **Calories:** {recommendedTodayCalories}
- **Carb:** {recommendedTodayCarb}g
- **Fat:** {recommendedTodayFat}g
- **Protein:** {recommendedTodayProtein}g

Yêu cầu bắt buộc:
- Tổng giá trị dinh dưỡng của các món ăn trong ngày **phải đạt tối thiểu**:
    - Calories >= {recommendedTodayCalories}
    - Carbs >= {recommendedTodayCarb}g
    - Fat >= {recommendedTodayFat}g
    - Protein >= {recommendedTodayProtein}g
- Nếu không thể đạt đúng, hãy chọn món khác từ danh sách để đảm bảo đủ chỉ tiêu.
- Không được gửi kết quả nếu tổng Calories dưới {recommendedTodayCalories}.

Quy định phản hồi:
- Trả về theo đúng định dạng JSON mẫu như sau: {jsonSampleOutput}
- Chỉ trả về **JSON thuần túy**, không kèm theo giải thích, chú thích, markdown hoặc mô tả.";

            var airesponse = await _aiGeneratorService.AIResponseJson(input, jsonSampleOutput);

            var airecommendMeallogExisted = await _unitOfWork.AIRecommendationMeallogRepository
                .GetByWhere(x => x.UserId == userId && x.Status.ToLower() == "pending")
                .FirstOrDefaultAsync();
            if (airecommendMeallogExisted == null)
            {
                var airecommendmealog = new AirecommendMealLog
                {
                    MealLogId = null,
                    Status = "Pending",
                    UserId = userId,
                    AirecommendMealLogResponse = airesponse
                };

                await _unitOfWork.AIRecommendationMeallogRepository.AddAsync(airecommendmealog);
            }
            else
            {
                airecommendMeallogExisted.AirecommendMealLogResponse = airesponse;
                await _unitOfWork.AIRecommendationMeallogRepository.UpdateAsync(airecommendMeallogExisted);
            }
            await _unitOfWork.SaveChangesAsync();

            var mealogresquestToadd = JsonSerializer.Deserialize<List<MealLogRequest>>(airesponse);

            var foodIds = mealogresquestToadd.Select(m => m.FoodId).Distinct().ToList();

            var foodList = await _unitOfWork.FoodRepository
                .GetByWhere(f => foodIds.Contains(f.FoodId))
                .ToListAsync();

            var mealLogDetails = mealogresquestToadd
                .Select(m =>
                {
                    var food = foodList.FirstOrDefault(f => f.FoodId == m.FoodId);
                    if (food == null) return null; // Nếu không tìm thấy food, bỏ qua

                    return new MealLogDetailResponse
                    {
                        FoodName = food.FoodName,
                        MealType = m.MealType.ToString(),
                        ServingSize = m.ServingSize,
                        Quantity = m.Quantity,
                        Calories = (m.Quantity ?? 1) * (food.Calories ?? 0),
                        Protein = (m.Quantity ?? 1) * (food.Protein ?? 0),
                        Carbs = (m.Quantity ?? 1) * (food.Carbs ?? 0),
                        Fat = (m.Quantity ?? 1) * (food.Fat ?? 0)
                    };
                })
                .Where(m => m != null)
                .ToList();

            return new BusinessResult(Const.HTTP_STATUS_OK, "Input đã tạo thành công", mealLogDetails);
        }





        private async Task SaveMeallogOneDay(List<MealLogRequest> requests)
        {

            if (requests == null || !requests.Any())
            {
                throw new ArgumentException("Meal log request list cannot be empty.");
            }

            foreach (var mealLogRequest in requests)
            {
                await AddOrUpdateMealLog(mealLogRequest);
            }
        }

        public async Task<IBusinessResult> SaveMeallogAI(string? feedback)
        {
            var userId = int.Parse(_userIdClaim);
            var isPremiumResult = await _unitOfWork.UserPackageRepository.IsUserPremiumAsync(userId);
            if (!isPremiumResult)
            {
                return new BusinessResult(Const.HTTP_STATUS_FORBIDDEN, "Chỉ tài khoản Premium mới sử dụng được tính năng này");
            }

            var airecommendMeallogExisted = await _unitOfWork.AIRecommendationMeallogRepository.GetByWhere(x => x.UserId == userId && x.Status.ToLower() == "pending").FirstOrDefaultAsync();
            if(airecommendMeallogExisted == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "No pending AI recommendation found.");
            }

            airecommendMeallogExisted.Status = "Accepted";
            if (!string.IsNullOrWhiteSpace(feedback))
            {
                airecommendMeallogExisted.Feedback = feedback;
            }
            var aiResponse = JsonSerializer.Deserialize<List<MealLogRequest>>(airecommendMeallogExisted.AirecommendMealLogResponse);
            var today = DateTime.Now.Date;
            var existingMealLog = await _unitOfWork.MealLogRepository
                .GetByWhere(m => m.UserId == userId && m.LogDate.Value.Date == today)
                .Include(m => m.MealLogDetails)
                .FirstOrDefaultAsync();
            if (existingMealLog != null)
            {
                await _unitOfWork.MealLogRepository.DeleteAsync(existingMealLog);
                await _unitOfWork.SaveChangesAsync();
            }
            await SaveMeallogOneDay(aiResponse);

            return new BusinessResult(Const.HTTP_STATUS_OK, "Meal log AI saved successfully.");
        }

        public async Task<IBusinessResult> TransferMealLogDetail(int detailId, MealType targetMealType)
        {
            var userId = int.Parse(_userIdClaim);

            // Tìm MealLog chứa MealLogDetail có detailId cho người dùng hiện tại
            var mealLog = await _unitOfWork.MealLogRepository
                .GetByWhere(m => m.UserId == userId && m.MealLogDetails.Any(d => d.DetailId == detailId))
                .Include(m => m.MealLogDetails)
                .FirstOrDefaultAsync();

            if (mealLog == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Meal log detail not found.", null);
            }

            // Tìm chi tiết bữa ăn cần chuyển
            var mealLogDetail = mealLog.MealLogDetails.FirstOrDefault(d => d.DetailId == detailId);
            if (mealLogDetail == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Meal log detail not found.", null);
            }

            // Nếu bữa hiện tại đã trùng với bữa chuyển đến thì trả về thông báo
            if (mealLogDetail.MealType.Equals(targetMealType.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return new BusinessResult(Const.HTTP_STATUS_OK, "Meal log detail is already in the target meal type.", null);
            }

            // Cập nhật MealType cho MealLogDetail
            mealLogDetail.MealType = targetMealType.ToString();

            // Nếu cần thiết, bạn có thể cập nhật lại các thông tin tổng hợp của MealLog nếu chuyển bữa cần điều chỉnh tổng dinh dưỡng.
            // Trong ví dụ này, vì chỉ chuyển bữa mà không thay đổi số lượng hay giá trị dinh dưỡng, nên ta không cần tính lại tổng.

            await _unitOfWork.MealLogRepository.UpdateAsync(mealLog);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, "Meal log detail transferred successfully.");
        }

        public async Task<IBusinessResult> AddMealToMultipleDays(AddMultipleDaysMealLogRequest request)
        {
            var userId = int.Parse(_userIdClaim);

            var existingUser = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (existingUser == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "User does not exist.", null);
            }

            if (request.Dates == null || !request.Dates.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "No dates specified.", null);
            }

            foreach (var date in request.Dates)
            {
                // Tạo request cho từng ngày
                var singleDayRequest = new MealLogRequest
                {
                    LogDate = date,               
                    FoodId = request.FoodId,      
                    Quantity = request.Quantity,  
                    MealType = request.MealType,  
                };

                var result = await AddOrUpdateMealLog(singleDayRequest);

                if (result.StatusCode != Const.HTTP_STATUS_OK)
                {
                    return result;
                }

            }
            return new BusinessResult(Const.HTTP_STATUS_OK, "Added meal to multiple days successfully.");
        }

        public async Task<IBusinessResult> GetNutritionSummary(DateTime date)
        {
            var userId = int.Parse(_userIdClaim);

            // Lấy user + personal goals
            var existingUser = await _unitOfWork.UserRepository
                .GetByWhere(u => u.UserId == userId)
                .Include(u => u.PersonalGoals)
                .FirstOrDefaultAsync();

            if (existingUser == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "User not found.", null);
            }

            var personalGoal = existingUser.PersonalGoals.FirstOrDefault();
            double dailyGoal = personalGoal?.DailyCalories ?? 2000;
            double carbsGoalRatio = personalGoal?.DailyCarb ?? 50;
            double fatGoalRatio = personalGoal?.DailyFat ?? 20;
            double proteinGoalRatio = personalGoal?.DailyProtein ?? 30;

            // Lấy MealLog
            var mealLog = await _unitOfWork.MealLogRepository
                .GetByWhere(m => m.UserId == userId && m.LogDate.Value.Date == date.Date)
                .Include(m => m.MealLogDetails)
                .ThenInclude(d => d.Food)
                .FirstOrDefaultAsync();

            // Nếu không có mealLog => trả về rỗng
            if (mealLog == null)
            {
                var emptyResp = new NutritionSummaryResponse
                {
                    Goal = dailyGoal,
                    MacroGoal = new MacroGoalResponse
                    {
                        CarbsRatio = carbsGoalRatio,
                        FatRatio = fatGoalRatio,
                        ProteinRatio = proteinGoalRatio
                    }
                };
                return new BusinessResult(Const.HTTP_STATUS_OK, "No meal log found for this date.", emptyResp);
            }

            // Tính totalCalories, netCalories
            double totalCalories = mealLog.MealLogDetails.Sum(d => d.Calories) ?? 0;
            double netCalories = totalCalories; // tuỳ logic

            // Phân bổ calories theo bữa
            var groupByMeal = mealLog.MealLogDetails
                .GroupBy(d => d.MealType)
                .Select(g => new {
                    MealType = g.Key,
                    TotalCals = g.Sum(x => x.Calories ?? 0)
                })
                .ToList();

            var mealBreakdown = new List<MealTypeBreakdownResponse>();
            foreach (var group in groupByMeal)
            {
                double mealCals = group.TotalCals;
                double percentage = totalCalories > 0 ? (mealCals / totalCalories * 100) : 0;
                mealBreakdown.Add(new MealTypeBreakdownResponse
                {
                    MealType = group.MealType,
                    Calories = mealCals,
                    Percentage = Math.Round(percentage, 2)
                });
            }

            // Tính macros thực tế
            double totalCarbs = mealLog.MealLogDetails.Sum(d => d.Carbs) ?? 0;
            double totalFat = mealLog.MealLogDetails.Sum(d => d.Fat) ?? 0;
            double totalProtein = mealLog.MealLogDetails.Sum(d => d.Protein) ?? 0;

            // Top 5 highest in Calories
            var highestInCalories = mealLog.MealLogDetails
                .OrderByDescending(d => d.Calories)
                .Take(5)
                .Select(d => new FoodHighResponse
                {
                    FoodName = d.Food?.FoodName ?? "Quick Add",
                    Value = d.Calories ?? 0
                })
                .ToList();

            // Highest in Carbs
            var highestInCarbs = mealLog.MealLogDetails
                .OrderByDescending(d => d.Carbs)
                .Take(5)
                .Select(d => new FoodHighResponse
                {
                    FoodName = d.Food?.FoodName ?? "Quick Add",
                    Value = d.Carbs ?? 0
                })
                .ToList();

            // Highest in Fat
            var highestInFat = mealLog.MealLogDetails
                .OrderByDescending(d => d.Fat)
                .Take(5)
                .Select(d => new FoodHighResponse
                {
                    FoodName = d.Food?.FoodName ?? "Quick Add",
                    Value = d.Fat ?? 0
                })
                .ToList();

            // Highest in Protein
            var highestInProtein = mealLog.MealLogDetails
                .OrderByDescending(d => d.Protein)
                .Take(5)
                .Select(d => new FoodHighResponse
                {
                    FoodName = d.Food?.FoodName ?? "Quick Add",
                    Value = d.Protein ?? 0
                })
                .ToList();

            // Tạo response
            var response = new NutritionSummaryResponse
            {
                TotalCalories = totalCalories,
                NetCalories = netCalories,
                Goal = dailyGoal,
                MealBreakdown = mealBreakdown,
                HighestInCalories = highestInCalories,
                Macros = new MacrosResponse
                {
                    Carbs = totalCarbs,
                    Fat = totalFat,
                    Protein = totalProtein
                },
                MacroGoal = new MacroGoalResponse
                {
                    CarbsRatio = carbsGoalRatio,
                    FatRatio = fatGoalRatio,
                    ProteinRatio = proteinGoalRatio
                },
                HighestInCarbs = highestInCarbs,
                HighestInFat = highestInFat,
                HighestInProtein = highestInProtein
            };

            return new BusinessResult(Const.HTTP_STATUS_OK, "Nutrition summary retrieved successfully.", response);
        }

        public async Task<IBusinessResult> AddImageToMealLogDetail(int detailId, AddImageRequest request)
        {
            if (request.Image == null || request.Image.Length == 0)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Image file is required.", null);
            }
            int userId = int.Parse(_userIdClaim);

            var mealLog = await _unitOfWork.MealLogRepository
                .GetByWhere(m => m.UserId == userId && m.MealLogDetails.Any(d => d.DetailId == detailId))
                .Include(m => m.MealLogDetails)
                .FirstOrDefaultAsync();

            if (mealLog == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Meal log not found.", null);
            }

            var mealLogDetail = mealLog.MealLogDetails.FirstOrDefault(d => d.DetailId == detailId);
            try
            {
                var cloudinaryHelper = new CloudinaryHelper();
                mealLogDetail.ImageUrl = await cloudinaryHelper.UploadImageWithCloudDinary(request.Image);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, "Error uploading image.", null);
            }
            await _unitOfWork.MealLogRepository.UpdateAsync(mealLog);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, "Image added to meal log detail successfully.");
        }
        public async Task<IBusinessResult> GetMealLogDetail(int detailId)
        {
            int userId = int.Parse(_userIdClaim);
            var mealLog = await _unitOfWork.MealLogRepository
                .GetByWhere(m => m.UserId == userId && m.MealLogDetails.Any(d => d.DetailId == detailId))
                .Include(m => m.MealLogDetails)
                .ThenInclude(d => d.Food)
                .FirstOrDefaultAsync();

            if (mealLog == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Meal log not found.", null);
            }
            var mealLogDetail = mealLog.MealLogDetails.FirstOrDefault(d => d.DetailId == detailId);
            if (mealLogDetail == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Meal log detail not found.", null);
            }
            var response = new MealLogDetailResponse
            {
                DetailId = mealLogDetail.DetailId,
                FoodName = mealLogDetail.Food?.FoodName ?? "Quick Add",
                MealType = mealLogDetail.MealType,
                ServingSize = mealLogDetail.ServingSize,
                Quantity = mealLogDetail.Quantity,
                Calories = mealLogDetail.Calories ?? 0,
                Protein = mealLogDetail.Protein ?? 0,
                Carbs = mealLogDetail.Carbs ?? 0,
                Fat = mealLogDetail.Fat ?? 0,
                ImageUrl = mealLogDetail.ImageUrl ?? ""
            };
            return new BusinessResult(Const.HTTP_STATUS_OK, "Meal log detail retrieved successfully.", response);
        }

        public async Task<IBusinessResult> UpdateMealLogDetailNutrition(int detailId, UpdateMealLogNutritionRequest request)
        {
            int userId = int.Parse(_userIdClaim);

            var mealLog = await _unitOfWork.MealLogRepository
                .GetByWhere(m => m.UserId == userId && m.MealLogDetails.Any(d => d.DetailId == detailId))
                .Include(m => m.MealLogDetails)
                .FirstOrDefaultAsync();

            if (mealLog == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Meal log not found.", null);
            }

            var mealLogDetail = mealLog.MealLogDetails.FirstOrDefault(d => d.DetailId == detailId);
            if (mealLogDetail == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Meal log detail not found.", null);
            }
            mealLogDetail.Calories = request.Calorie;
            mealLogDetail.Protein = request.Protein;
            mealLogDetail.Carbs = request.Carbs;
            mealLogDetail.Fat = request.Fat;
            mealLog.TotalCalories = mealLog.MealLogDetails.Sum(d => d.Calories ?? 0);
            mealLog.TotalProtein = mealLog.MealLogDetails.Sum(d => d.Protein ?? 0);
            mealLog.TotalCarbs = mealLog.MealLogDetails.Sum(d => d.Carbs ?? 0);
            mealLog.TotalFat = mealLog.MealLogDetails.Sum(d => d.Fat ?? 0);

            await _unitOfWork.MealLogRepository.UpdateAsync(mealLog);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, "Meal log detail and nutrition values updated successfully.");
        }

        public async Task<IBusinessResult> AnalyzeAndPredictMealImprovements(DateTime logDate)
        {
            var userId = int.Parse(_userIdClaim);
            var isPremiumResult = await _unitOfWork.UserPackageRepository.IsUserPremiumAsync(userId);
            if (!isPremiumResult)
            {
                return new BusinessResult(Const.HTTP_STATUS_FORBIDDEN, "Chỉ tài khoản Premium mới sử dụng được tính năng này");
            }

            // Lấy thông tin người dùng
            var userInfo = await _unitOfWork.UserRepository.GetByWhere(x => x.UserId == userId)
                                                           .Include(x => x.GeneralHealthProfiles)
                                                           .Include(x => x.PersonalGoals)
                                                           .FirstOrDefaultAsync();
            if (userInfo == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "User not found", null);
            }

            var userProfile = userInfo.GeneralHealthProfiles.FirstOrDefault();
            var personalGoal = userInfo.PersonalGoals.FirstOrDefault();

            var height = userProfile?.Height ?? 0;
            var weight = userProfile?.Weight ?? 0;
            var activityLevel = userProfile?.ActivityLevel ?? "Không xác định";
            var goalType = personalGoal?.GoalType ?? "Không có mục tiêu";
            var goalcalories = personalGoal?.DailyCalories;
            var dietStyle = userProfile?.DietStyle ?? "Không xác định";
            var goalfat = personalGoal?.DailyFat;
            var goalcarb = personalGoal?.DailyCarb;
            var goalprotein = personalGoal?.DailyProtein;
            var mealLogsResult = await GetMealLogsByDateRange(logDate, null, null);
            if (mealLogsResult.StatusCode != Const.HTTP_STATUS_OK)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Không đủ dữ liệu để phân tích.", null);
            }

            var mealLogsList = mealLogsResult.Data as List<MealLogResponse>;
            if (mealLogsList == null || mealLogsList.Count == 0)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Không đủ dữ liệu để phân tích.", null);
            }

            double totalCalories = mealLogsList.Sum(m => m.TotalCalories);
            const double MIN_DAILY_CALORIES = 1200;
            if (totalCalories < MIN_DAILY_CALORIES)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Lượng calories tối thiểu 1 ngày cho 1 người trưởng thành không được thấp hơn chuẩn quốc tế quy định" + MIN_DAILY_CALORIES, null);
            }

            var serializedMealLogs = JsonSerializer.Serialize(mealLogsList);

            // Xây dựng prompt cho AI
            var prompt = $@"Bạn là một chuyên gia dinh dưỡng với nhiều năm kinh nghiệm. Dưới đây là thông tin người dùng và nhật ký ăn uống của ngày {logDate:dd/MM/yyyy}:

Thông tin người dùng:
- Họ tên: {userInfo.FullName}
- Giới tính: {userInfo.Gender}
- Tuổi: {userInfo.Age}
- Chiều cao: {height} cm
- Cân nặng: {weight} kg
- Mức độ vận động: {activityLevel}
- Mục tiêu: {goalType}
- Mục tiêu Calories: {goalcalories}
- Mục tiêu Protein: {goalprotein}
- Mục tiêu Carb: {goalcarb}
- Mục tiêu Fat: {goalfat}
- DietStyle: {dietStyle}

Nhật ký ăn uống ngày {logDate:dd/MM/yyyy}:
{serializedMealLogs}

Hãy phân tích những điểm cần cải thiện trong chế độ ăn uống của người dùng trong ngày này, đưa ra các khuyến nghị cụ thể về cải thiện dinh dưỡng, luyện tập và thay đổi lối sống nếu cần. Đồng thời, hãy dự đoán khoảng thời gian cần thiết để người dùng đạt được mục tiêu sức khỏe của mình. Vui lòng trả lời chỉ dưới dạng văn bản thuần túy, không kèm theo bất kỳ giải thích thêm nào, và giới hạn kết quả trong khoảng 250 đến 300 từ.";

            var aiResponse = await _aiGeneratorService.AIResponseText(prompt);
            return new BusinessResult(Const.HTTP_STATUS_OK, "Phân tích và dự đoán thành công.", aiResponse);
        }
    }
}
