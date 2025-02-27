using Azure;
using Mapster;
using Microsoft.AspNetCore.Http;
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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Services
{
    public class MealLogService : IMealLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userIdClaim;

        public MealLogService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
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
                    Quantity = quantity,
                    ServingSize = request.ServingSize,
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

        public async Task<IBusinessResult> RemoveMealLogDetail(int mealLogId,int detailId)
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
                    FoodName = d.Food.FoodName, 
                    MealType = d.MealType,
                    Quantity = d.Quantity,
                    ServingSize = d.ServingSize,
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
                    FoodName = d.Food?.FoodName ?? "Unknown",
                    MealType = d.MealType,
                    Quantity = d.Quantity,
                    ServingSize = d.ServingSize,
                    Calories = d.Calories ?? 0,
                    Protein = d.Protein ?? 0,
                    Carbs = d.Carbs ?? 0,
                    Fat = d.Fat ?? 0
                }).ToList()
            }).ToList();

            return new BusinessResult(Const.HTTP_STATUS_OK, "Meal logs retrieved successfully.", response);
        }

    }
}
