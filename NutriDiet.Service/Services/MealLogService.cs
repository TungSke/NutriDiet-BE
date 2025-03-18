using Azure;
using Google.Apis.Drive.v3.Data;
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

            // Kiểm tra người dùng tồn tại
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
                .GetByWhere(m => m.UserId == userId && m.LogDate == logDate)
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
                    ServingSize = request.ServingSize,
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

        public async Task<IBusinessResult> GetMealLogById(int mealLogId)
        {
            var userId = int.Parse(_userIdClaim);

            var mealLog = await _unitOfWork.MealLogRepository
                .GetByWhere(m => m.MealLogId == mealLogId && m.UserId == userId)
                .Include(m => m.MealLogDetails)
                .ThenInclude(d => d.Food)
                .FirstOrDefaultAsync();

            if (mealLog == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Meal log not found.", null);
            }

            // Map dữ liệu sang DTO
            var response = new MealLogResponse
            {
                MealLogId = mealLog.MealLogId,
                LogDate = mealLog.LogDate.Value,
                TotalCalories = mealLog.TotalCalories.Value,
                TotalProtein = mealLog.TotalProtein.Value,
                TotalCarbs = mealLog.TotalCarbs.Value,
                TotalFat = mealLog.TotalFat.Value,
                MealLogDetails = mealLog.MealLogDetails.Select(d => new MealLogDetailResponse
                {
                    DetailId = d.DetailId,
                    FoodName = d.Food.FoodName ?? "Quick Add",
                    MealType = d.MealType,
                    ServingSize = d.ServingSize,
                    Quantity = d.Quantity,
                    Calories = d.Calories.Value,
                    Protein = d.Protein.Value,
                    Carbs = d.Carbs.Value,
                    Fat = d.Fat.Value
                }).ToList()
            };

            return new BusinessResult(Const.HTTP_STATUS_OK, "Meal log retrieved successfully.", response);
        }
        public async Task<IBusinessResult> GetMealLogsByDateRange(DateTime? logDate, DateTime? fromDate, DateTime? toDate)
        {
            var userId = int.Parse(_userIdClaim);

            // Truy vấn MealLog theo điều kiện
            IQueryable<MealLog> query = _unitOfWork.MealLogRepository
                .GetByWhere(m => m.UserId == userId)
                .Include(m => m.MealLogDetails)
                .ThenInclude(d => d.Food); // ✅ Include để lấy thông tin món ăn

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
                    Fat = d.Fat ?? 0
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

            // Lấy MealLog theo ngày, nếu chưa có thì tạo mới
            var mealLog = await _unitOfWork.MealLogRepository
                .GetByWhere(m => m.UserId == userId && m.LogDate == logDate)
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

            // Cập nhật tổng giá trị dinh dưỡng của MealLog
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

            // Kiểm tra ngày hợp lệ
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

            // Lọc danh sách MealLogDetails theo bữa ăn
            var sourceDetails = sourceMealLog.MealLogDetails
                .Where(d => d.MealType == request.MealType.ToString())
                .ToList();

            if (!sourceDetails.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, $"No meal log details found for {request.MealType} on source date.", null);
            }

            // Lấy hoặc tạo MealLog của ngày đích
            var targetMealLog = await _unitOfWork.MealLogRepository
                .GetByWhere(m => m.UserId == userId && m.LogDate == targetDate)
                .Include(m => m.MealLogDetails)
                .FirstOrDefaultAsync();

            bool isNewMealLog = false;

            if (targetMealLog == null)
            {
                // Không tạo mới MealLog trong quá khứ nếu không tồn tại
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
                await _unitOfWork.SaveChangesAsync(); // Cần lưu ngay để lấy `MealLogId`
                isNewMealLog = true;
            }

            // Sao chép các chi tiết bữa ăn từ ngày nguồn sang ngày đích
            foreach (var detail in sourceDetails)
            {
                var copiedDetail = new MealLogDetail
                {
                    MealLogId = targetMealLog.MealLogId, // Đảm bảo có MealLogId hợp lệ
                    FoodId = detail.FoodId,
                    MealType = detail.MealType,
                    ServingSize = detail.ServingSize,
                    Quantity = detail.Quantity,
                    Calories = detail.Calories,
                    Protein = detail.Protein,
                    Carbs = detail.Carbs,
                    Fat = detail.Fat
                };

                targetMealLog.MealLogDetails.Add(copiedDetail);
            }

            // Cập nhật lại tổng giá trị dinh dưỡng cho ngày đích
            targetMealLog.TotalCalories += sourceDetails.Sum(d => d.Calories);
            targetMealLog.TotalProtein += sourceDetails.Sum(d => d.Protein);
            targetMealLog.TotalCarbs += sourceDetails.Sum(d => d.Carbs);
            targetMealLog.TotalFat += sourceDetails.Sum(d => d.Fat);

            if (!isNewMealLog) // Nếu MealLog đã tồn tại, chỉ cần cập nhật
            {
                await _unitOfWork.MealLogRepository.Attach(targetMealLog);
                await _unitOfWork.MealLogRepository.UpdateAsync(targetMealLog);
            }

            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, "Meal log details copied successfully.");
        }

        public async Task<IBusinessResult> CreateMealLogAI()
        {
            var userId = int.Parse(_userIdClaim);

            var userInfo = await _unitOfWork.UserRepository.GetByWhere(x => x.UserId == userId)
                                                           .Include(x => x.GeneralHealthProfiles)
                                                           .Include(x => x.UserFoodPreferences)
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
            var dailyCalories = personalGoal?.DailyCalories ?? 0;
            var dailyCarb = personalGoal?.DailyCarb ?? 0;
            var dailyFat = personalGoal?.DailyFat ?? 0;
            var dailyProtein = personalGoal?.DailyProtein ?? 0;

            // Lấy Meal Log 7 ngày gần nhất
            var mealLogs = await GetMealLogsByDateRange(null, DateTime.Now.AddDays(-7), DateTime.Now);

            var formattedMealLogs = JsonSerializer.Serialize(mealLogs.Data);

            // Lấy danh sách thực phẩm có thể ăn
            var foods = await _unitOfWork.FoodRepository.GetAll().ToListAsync();
            var foodListText = JsonSerializer.Serialize(foods);

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
                }
            };

            var jsonSampleOutput = JsonSerializer.Serialize(mealogrequestSample);

            var input = $@"Bạn là một chuyên gia dinh dưỡng. Nhiệm vụ của bạn là tạo một Meal Log phù hợp với mục tiêu và điều kiện sức khỏe của người dùng.

                        Thông tin người dùng:
                        - **Họ tên:** {userInfo.FullName}
                        - **Giới tính:** {userInfo.Gender}
                        - **Tuổi:** {userInfo.Age}
                        - **Chiều cao:** {height} cm
                        - **Cân nặng:** {weight} kg
                        - **Mức độ vận động:** {activityLevel}
                        - **Mục tiêu:** {goalType}

                        Dữ liệu Meal Log 7 ngày gần nhất:
                        {formattedMealLogs}

                        Yêu cầu cho Meallog ngày hôm nay:
                        - **Meallog 1 ngày** với 3 bữa chính (Breakfast, Lunch, Dinner)
                        - **Chỉ chọn thực phẩm từ danh sách:** {foodListText}
                        - **Dị ứng thực phẩm:** {formattedAllergies}
                        - **Bệnh lý cần lưu ý:** {formattedDiseases}

                        Giá trị dinh dưỡng đề xuất cho user nạp:
                        - **Calories:** {dailyCalories}
                        - **Carb:** {dailyCarb}
                        - **Fat:** {dailyFat}
                        - **Protein:** {dailyProtein}

                        Lưu ý:
                        - Hạn chế chọn các món đã ăn quá nhiều trong tuần
                        - Chỉ trả về **JSON thuần túy**, không kèm theo giải thích.";

            var airesponse = await _aiGeneratorService.AIResponseJson(input, jsonSampleOutput);

            var airecommendmealog = new AirecommendMealLog
            {
                MealLogId = null,
                Status = "Pending",
                
            };

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
                .Where(m => m != null) // Loại bỏ food null
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

        public async Task<IBusinessResult> SaveMeallogAI()
        {
            return new BusinessResult(Const.HTTP_STATUS_OK, "Meal log AI saved successfully.");
        }
    }
}
