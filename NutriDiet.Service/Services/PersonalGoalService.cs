using Microsoft.AspNetCore.Http;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Common;
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
using Mapster;
using Microsoft.EntityFrameworkCore;
using NutriDiet.Common.Enums;

namespace NutriDiet.Service.Services
{
    public class PersonalGoalService : IPersonalGoalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userIdClaim;

        public PersonalGoalService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
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

        public async Task CreatePersonalGoal(PersonalGoalRequest request)
        {
            var userId = int.Parse(_userIdClaim);
            var existingUser = await _unitOfWork.UserRepository
                .GetByWhere(u => u.UserId == userId)
                .Include(u => u.GeneralHealthProfiles)
                .ThenInclude(u => u.HealthcareIndicators)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (existingUser == null)
            {
                throw new Exception("User does not exist.");
            }

            var latestProfile = existingUser.GeneralHealthProfiles
                                            .OrderByDescending(h => h.CreatedAt)
                                            .FirstOrDefault();

            if (latestProfile == null)
            {
                throw new Exception("No health profile found.");
            }

            var currentWeight = latestProfile.Weight;
            var tdee = latestProfile.HealthcareIndicators
                .Where(h => h.Code.Equals("TDEE"))
                .OrderByDescending(h => h.CreatedAt)
                .FirstOrDefault()?.CurrentValue ?? 0;

            if (tdee == 0)
            {
                throw new Exception("TDEE data is missing.");
            }

            // Kiểm tra hợp lệ của mục tiêu
            ValidatePersonalGoal(request, currentWeight);

            // Tính toán lượng calo hàng ngày và thời gian cần đạt mục tiêu
            var (dailyCalories, targetDate) = CalculateDailyCaloriesAndTargetDate(request, tdee, currentWeight);

            // Tính toán tỷ lệ macronutrients (%) theo mục tiêu
            var macronutrients = CalculateMacronutrientRatios(request.GoalType);

            // Nếu user đã có personal goal đang Active thì xóa nó đi trước
            var existingGoal = await _unitOfWork.PersonalGoalRepository
                .GetByWhere(pg => pg.UserId == userId)
                .FirstOrDefaultAsync();
            if (existingGoal != null)
            {
                await _unitOfWork.PersonalGoalRepository.DeleteAsync(existingGoal);
            }

            var personalGoal = request.Adapt<PersonalGoal>();
            personalGoal.UserId = userId;
            personalGoal.StartDate = DateTime.Now;
            personalGoal.Status = "Active";
            personalGoal.ProgressPercentage = 0;
            personalGoal.ProgressRate = (int)(currentWeight - request.TargetWeight);
            personalGoal.DailyCalories = (int)dailyCalories;
            personalGoal.TargetDate = targetDate ?? DateTime.Now;
            personalGoal.DailyCarb = Math.Round(dailyCalories * (macronutrients.CarbRatio / 100.0) / 4.0, 2);
            personalGoal.DailyProtein = Math.Round(dailyCalories * (macronutrients.ProteinRatio / 100.0) / 4.0, 2);
            personalGoal.DailyFat = Math.Round(dailyCalories * (macronutrients.FatRatio / 100.0) / 9.0, 2);

            await _unitOfWork.PersonalGoalRepository.AddAsync(personalGoal);
            await _unitOfWork.SaveChangesAsync();
        }

        private void ValidatePersonalGoal(PersonalGoalRequest request, double? currentWeight)
        {
            if (request.TargetWeight == null || request.WeightChangeRate == null)
            {
                throw new Exception("Target weight and weight change rate must be provided.");
            }

            if (!Enum.IsDefined(typeof(WeightChangeRate), request.WeightChangeRate))
            {
                throw new Exception("Tốc độ thay đổi cân nặng không hợp lệ.");
            }

            switch (request.GoalType)
            {
                case GoalType.GainWeight:
                    if (request.TargetWeight <= currentWeight)
                    {
                        throw new Exception($"Mục tiêu tăng cân không hợp lệ. Cân nặng hiện tại: {currentWeight} kg, mục tiêu: {request.TargetWeight} kg.");
                    }
                    if (request.WeightChangeRate <= 0)
                    {
                        throw new Exception("Tốc độ thay đổi cân nặng phải lớn hơn 0 khi mục tiêu là tăng cân.");
                    }
                    break;

                case GoalType.LoseWeight:
                    if (request.TargetWeight >= currentWeight)
                    {
                        throw new Exception($"Mục tiêu giảm cân không hợp lệ. Cân nặng hiện tại: {currentWeight} kg, mục tiêu: {request.TargetWeight} kg.");
                    }
                    if (request.WeightChangeRate >= 0)
                    {
                        throw new Exception("Tốc độ thay đổi cân nặng phải nhỏ hơn 0 khi mục tiêu là giảm cân.");
                    }
                    break;

                case GoalType.Maintain:
                    if (request.TargetWeight != currentWeight)
                    {
                        throw new Exception($"Mục tiêu duy trì cân nặng không hợp lệ. Cân nặng hiện tại: {currentWeight} kg, mục tiêu phải là {currentWeight} kg.");
                    }
                    if (request.WeightChangeRate != 0)
                    {
                        throw new Exception("Tốc độ thay đổi cân nặng phải là 0 khi duy trì cân nặng.");
                    }
                    break;

                default:
                    throw new Exception("Mục tiêu không hợp lệ.");
            }
        }

        private (double dailyCalories, DateTime? targetDate) CalculateDailyCaloriesAndTargetDate(PersonalGoalRequest request, double tdee, double? currentWeight)
        {
            double dailyCalories;
            DateTime? targetDate = null;

            switch (request.GoalType)
            {
                case GoalType.Maintain:
                    dailyCalories = tdee;
                    break;

                case GoalType.GainWeight:
                case GoalType.LoseWeight:
                    double weightChangePerWeek = (int)request.WeightChangeRate / 1000.0; // kg/tuần
                    double dailyCalorieAdjustment = (weightChangePerWeek * 7700) / 7;
                    dailyCalories = tdee + dailyCalorieAdjustment;

                    double weightDifference = Math.Abs((double)(currentWeight - request.TargetWeight.Value));
                    if (weightChangePerWeek == 0)
                    {
                        throw new Exception("Tốc độ thay đổi cân nặng không hợp lệ.");
                    }

                    int totalDays = (int)Math.Ceiling((weightDifference / Math.Abs(weightChangePerWeek)) * 7);
                    targetDate = DateTime.Now.AddDays(totalDays);
                    break;

                default:
                    throw new Exception("Mục tiêu không hợp lệ.");
            }

            return (dailyCalories, targetDate);
        }

        private (double CarbRatio, double ProteinRatio, double FatRatio) CalculateMacronutrientRatios(GoalType goalType)
        {
            double carbRatio, proteinRatio, fatRatio;

            switch (goalType)
            {
                case GoalType.GainWeight:
                    carbRatio = 50;   // 50% từ Carb
                    proteinRatio = 25; // 25% từ Protein
                    fatRatio = 25;    // 25% từ Fat
                    break;

                case GoalType.LoseWeight:
                    carbRatio = 40;   // 40% từ Carb
                    proteinRatio = 35; // 35% từ Protein
                    fatRatio = 25;    // 25% từ Fat
                    break;

                case GoalType.Maintain:
                default:
                    carbRatio = 50;   // 50% từ Carb
                    proteinRatio = 30; // 30% từ Protein
                    fatRatio = 20;    // 20% từ Fat
                    break;
            }

            return (carbRatio, proteinRatio, fatRatio);
        }

        public async Task<IBusinessResult> GetPersonalGoal()
        {
            var userid = int.Parse(_userIdClaim);

            var existingUser = await _unitOfWork.UserRepository.GetByIdAsync(userid);

            if (existingUser == null)
            {
                throw new Exception("User not exist.");
            }

            var personalGoal = await _unitOfWork.PersonalGoalRepository
                .GetByWhere(pg => pg.UserId == userid)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var response = personalGoal.Adapt<PersonalGoalResponse>();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> UpdatePersonalGoal(PersonalGoalRequest request)
        {
            var userId = int.Parse(_userIdClaim);

            var existingGoal = await _unitOfWork.PersonalGoalRepository
                .GetByWhere(pg => pg.UserId == userId)
                .FirstOrDefaultAsync();

            if (existingGoal == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Personal goal not found.", null);
            }

            var existingUser = await _unitOfWork.UserRepository
                .GetByWhere(u => u.UserId == userId)
                .Include(u => u.GeneralHealthProfiles)
                .ThenInclude(u => u.HealthcareIndicators)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (existingUser == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "User does not exist.", null);
            }

            var latestProfile = existingUser.GeneralHealthProfiles
                                            .OrderByDescending(h => h.CreatedAt)
                                            .FirstOrDefault();

            if (latestProfile == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "No health profile found.", null);
            }

            var currentWeight = latestProfile.Weight;
            var tdee = latestProfile.HealthcareIndicators
                .Where(h => h.Code.Equals("TDEE"))
                .OrderByDescending(h => h.CreatedAt)
                .FirstOrDefault()?.CurrentValue ?? 0;

            if (tdee == 0)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "TDEE data is missing.", null);
            }

            try
            {
                // Kiểm tra hợp lệ của mục tiêu
                ValidatePersonalGoal(request, currentWeight);

                // Tính toán lượng calo hàng ngày và ngày đạt mục tiêu
                var (dailyCalories, targetDate) = CalculateDailyCaloriesAndTargetDate(request, tdee, currentWeight);

                // Tính toán tỷ lệ macronutrients (%) ban đầu
                var macronutrients = CalculateMacronutrientRatios(request.GoalType);

                // Cập nhật thông tin mục tiêu
                request.Adapt(existingGoal);
                existingGoal.DailyCalories = (int)dailyCalories;
                existingGoal.TargetDate = targetDate ?? DateTime.Now;
                // Chuyển đổi % sang gam và làm tròn 2 chữ số:
                existingGoal.DailyCarb = Math.Round(dailyCalories * (macronutrients.CarbRatio / 100.0) / 4.0, 2);
                existingGoal.DailyProtein = Math.Round(dailyCalories * (macronutrients.ProteinRatio / 100.0) / 4.0, 2);
                existingGoal.DailyFat = Math.Round(dailyCalories * (macronutrients.FatRatio / 100.0) / 9.0, 2);
                existingGoal.ProgressRate = (int)(currentWeight - request.TargetWeight);
                existingGoal.ProgressPercentage = 0;

                await _unitOfWork.PersonalGoalRepository.UpdateAsync(existingGoal);
                await _unitOfWork.SaveChangesAsync();

                var response = existingGoal.Adapt<PersonalGoalResponse>();
                return new BusinessResult(Const.HTTP_STATUS_OK, "Personal goal updated successfully.", response);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, ex.Message, null);
            }
        }

        public async Task<IBusinessResult> UpdateDailyMacronutrients(EditDailyMacronutrientsRequest request)
        {
            var userId = int.Parse(_userIdClaim);
            var existingGoal = await _unitOfWork.PersonalGoalRepository
                .GetByWhere(pg => pg.UserId == userId)
                .FirstOrDefaultAsync();

            if (existingGoal == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Personal goal not found.", null);
            }

            // Dựa vào DailyCalories đã tính, chuyển đổi % sang gam và làm tròn 2 chữ số:
            double dailyCalories = Convert.ToDouble(existingGoal.DailyCalories);
            existingGoal.DailyCarb = Math.Round(dailyCalories * (request.DailyCarb / 100.0) / 4.0, 2);
            existingGoal.DailyProtein = Math.Round(dailyCalories * (request.DailyProtein / 100.0) / 4.0, 2);
            existingGoal.DailyFat = Math.Round(dailyCalories * (request.DailyFat / 100.0) / 9.0, 2);

            await _unitOfWork.PersonalGoalRepository.UpdateAsync(existingGoal);
            await _unitOfWork.SaveChangesAsync();

            var response = existingGoal.Adapt<PersonalGoalResponse>();

            return new BusinessResult(Const.HTTP_STATUS_OK, "Daily macronutrients updated successfully.", response);
        }
    }
}
