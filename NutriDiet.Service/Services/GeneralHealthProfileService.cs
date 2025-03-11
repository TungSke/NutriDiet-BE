using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NutriDiet.Common;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Common.Enums;
using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using NutriDiet.Service.ModelDTOs.Response;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NutriDiet.Service.Services
{
    public class GeneralHealthProfileService : IGeneralHealthProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userIdClaim;

        public GeneralHealthProfileService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
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

        public async Task CreateHealthProfileRecord(HealthProfileRequest request)
        {
            var userId = int.Parse(_userIdClaim);
            var existingUser = await _unitOfWork.UserRepository
                .GetByWhere(x => x.UserId == userId)
                .Include(u => u.Allergies)
                .Include(u => u.Diseases)
                .FirstOrDefaultAsync();

            if (existingUser == null)
            {
                throw new Exception("User does not exist.");
            }
            var healthProfile = request.Adapt<GeneralHealthProfile>();
            healthProfile.CreatedAt = DateTime.Now;
            healthProfile.UpdatedAt = DateTime.Now;

            // Tính toán và lưu các chỉ số sức khỏe nếu đủ dữ liệu
            if (IsValidHealthData(request))
            {
                await SaveHealthIndicatorsAsync(userId, request,existingUser);
            }

            await _unitOfWork.BeginTransaction();
            try
                {
                // Cập nhật tiến trình giảm cân
                if(request.Weight != null)
                {
                    await UpdateGoalProgress(request.Weight, userId);
                }
                // Lưu hồ sơ sức khỏe
                healthProfile.UserId = existingUser.UserId;
                await _unitOfWork.HealthProfileRepository.AddAsync(healthProfile);
                await UpdateUserAllergiesAsync(existingUser, request.AllergyIds);
                await UpdateUserDiseasesAsync(existingUser, request.DiseaseIds);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();
            }
            catch
            {
                await _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        private async Task UpdateGoalProgress(double? weight, int userId)
        {
            var existgoal = await _unitOfWork.PersonalGoalRepository.GetByWhere(pg => pg.UserId == userId).FirstOrDefaultAsync();
            if (existgoal == null)
            {
                return;
            }
            var newrate = weight - existgoal.TargetWeight.Value;
            var percentage = 100 - (int)((newrate / existgoal.ProgressRate) * 100);
            if(percentage < 0)
            {
                return;
            }else if(percentage > 100)
            {
                percentage = 100;
            }
            existgoal.ProgressPercentage = percentage;
        }

        private async Task UpdateUserAllergiesAsync(User existingUser, List<int> allergyIds)
        {
            // Nếu danh sách mới rỗng, xóa toàn bộ dị ứng cũ
            if (allergyIds == null || !allergyIds.Any())
            {
                existingUser.Allergies.Clear();
                return;
            }

            // Xóa toàn bộ danh sách dị ứng cũ
            existingUser.Allergies.Clear();

            // Lấy danh sách dị ứng mới từ database
            var allergies = await _unitOfWork.AllergyRepository
                .GetByWhere(a => allergyIds.Contains(a.AllergyId))
                .ToListAsync();

            // Thêm từng dị ứng vào danh sách
            foreach (var allergy in allergies)
            {
                existingUser.Allergies.Add(allergy);
            }
        }
        private async Task UpdateUserDiseasesAsync(User existingUser, List<int> diseaseIds)
        {
            if (diseaseIds == null || !diseaseIds.Any())
            {
                existingUser.Diseases.Clear();
                return;
            }

            existingUser.Diseases.Clear();

            var diseases = await _unitOfWork.DiseaseRepository
                .GetByWhere(d => diseaseIds.Contains(d.DiseaseId))
                .ToListAsync();

            foreach (var disease in diseases)
            {
                existingUser.Diseases.Add(disease);
            }
        }


        /// <summary>
        /// Kiểm tra xem request có đủ dữ liệu để tính toán chỉ số sức khỏe hay không.
        /// </summary>
        private bool IsValidHealthData(HealthProfileRequest request)
        {
            return request.Weight.HasValue && request.Height.HasValue &&
                   request.ActivityLevel.HasValue;
        }

        /// <summary>
        /// Tính toán và lưu các chỉ số sức khỏe (BMI, TDEE)
        /// </summary>
        private async Task SaveHealthIndicatorsAsync(int userId, HealthProfileRequest request, User user)
        {
            var tdee = _unitOfWork.HealthcareIndicatorRepository.CalculateTDEE(
                request.Weight.Value, request.Height.Value, user.Age.Value,
                user.Gender.ToString().ToLower(), (double)request.ActivityLevel);

            var bmi = _unitOfWork.HealthcareIndicatorRepository.CalculateBMI(
                request.Weight.Value, request.Height.Value);

            // Phân loại BMI
            var bmiCategory = GetBMICategory(bmi);

            var bmiIndicator = CreateHealthIndicator(userId, "Body mass index", bmiCategory, "BMI", bmi);
            var tdeeIndicator = CreateHealthIndicator(userId, "Total daily energy expenditure", "Energy", "TDEE", tdee);

            await _unitOfWork.HealthcareIndicatorRepository.AddAsync(bmiIndicator);
            await _unitOfWork.HealthcareIndicatorRepository.AddAsync(tdeeIndicator);
        }

        private string GetBMICategory(double bmi)
        {
            return bmi switch
            {
                < 18.5 => "Gầy",
                >= 18.5 and < 24.9 => "Bình thường",
                >= 25 and < 29.9 => "Thừa cân",
                >= 30 and < 34.9 => "Béo phì độ 1",
                >= 35 and < 39.9 => "Béo phì độ 2",
                _ => "Béo phì độ 3"
            };
        }

        private HealthcareIndicator CreateHealthIndicator(int userId, string name, string type, string code, double value)
        {
            return new HealthcareIndicatorRequest
            {
                UserId = userId,
                Name = name,
                Type = type,
                Code = code,
                CurrentValue = value
            }.Adapt<HealthcareIndicator>();
        }


        public async Task<IBusinessResult> GetHealthProfile()
        {
            var userid = int.Parse(_userIdClaim);

            var existingUser = await _unitOfWork.UserRepository
                .GetByWhere(u => u.UserId == userid)
                .Include(u => u.Allergies)
                .Include(u => u.Diseases)
                .Include(u => u.HealthcareIndicators.OrderByDescending(hi => hi.CreatedAt))
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (existingUser == null)
            {
                throw new Exception("User not exist.");
            }
            existingUser.HealthcareIndicators = existingUser.HealthcareIndicators
                                                .Where(hi => hi.Code == "BMI" || hi.Code == "TDEE")
                                                .OrderByDescending(hi => hi.CreatedAt)
                                                .Take(2)
                                                .ToList();

            var healthProfile = await _unitOfWork.HealthProfileRepository
                .GetByWhere(hp => hp.UserId == userid)
                .OrderByDescending(hp => hp.CreatedAt)
                .AsNoTracking() 
                .FirstOrDefaultAsync();

            HealthProfileResponse response;
            try
            {
                response = existingUser.Adapt<HealthProfileResponse>();
            }
            catch (Exception ex)
            {
                throw new Exception("Error mapping User to HealthProfileResponse: " + ex.Message, ex);
            }

            if (healthProfile != null)
            {
                try
                {
                    healthProfile.Adapt(response);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error mapping HealthProfile to HealthProfileResponse: " + ex.Message, ex);
                }
            }

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public  async Task<IBusinessResult> DeleteHealthProfile(int userId)
        {
            var profile = _unitOfWork.HealthProfileRepository.GetByWhere(p => p.UserId == userId);
            if (profile == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "profile not found");
            }

            await _unitOfWork.HealthProfileRepository.RemoveRange(profile);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_DELETE_MSG);
        }

        public async Task<IBusinessResult> TrackingHealthProfile(HealProfileFields field)
        {
            var userId = int.Parse(_userIdClaim);

            var healthProfiles = await _unitOfWork.HealthProfileRepository
                .GetByWhere(hp => hp.UserId == userId)
                .OrderBy(hp => hp.CreatedAt)
                .Select(hp => new
                {
                    hp.CreatedAt,
                    Value = EF.Property<object>(hp, field.ToString())
                })
                .AsNoTracking()
                .ToListAsync();

            if (healthProfiles == null || !healthProfiles.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "No health history found for the requested field.");
            }

            var response = healthProfiles.Select(hp => new
            {
                Date = hp.CreatedAt,
                Value = hp.Value
            }).ToList();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

    }
}
