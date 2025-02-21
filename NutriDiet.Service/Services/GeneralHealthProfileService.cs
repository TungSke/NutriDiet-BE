using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NutriDiet.Common;
using NutriDiet.Common.BusinessResult;
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
            var existingUser = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (existingUser == null)
            {
                throw new Exception("User does not exist.");
            }

            request.Adapt(existingUser);
            var healthProfile = request.Adapt<GeneralHealthProfile>();
            healthProfile.CreatedAt = DateTime.Now;
            healthProfile.UpdatedAt = DateTime.Now;

            // Tính toán và lưu các chỉ số sức khỏe nếu đủ dữ liệu
            if (IsValidHealthData(request))
            {
                await SaveHealthIndicatorsAsync(userId, request);
            }

            await _unitOfWork.BeginTransaction();
            try
            {
                // Cập nhật thông tin người dùng
                await _unitOfWork.UserRepository.UpdateAsync(existingUser);

                // Lưu hồ sơ sức khỏe
                healthProfile.UserId = existingUser.UserId;
                await _unitOfWork.HealthProfileRepository.AddAsync(healthProfile);

                // Thêm dị ứng và bệnh lý
                await AddUserAllergiesAsync(existingUser, request.AllergyNames);
                await AddUserDiseasesAsync(existingUser, request.DiseaseNames);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();
            }
            catch
            {
                await _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        /// <summary>
        /// Kiểm tra xem request có đủ dữ liệu để tính toán chỉ số sức khỏe hay không.
        /// </summary>
        private bool IsValidHealthData(HealthProfileRequest request)
        {
            return request.Weight.HasValue && request.Height.HasValue &&
                   request.Age.HasValue && request.Gender.HasValue &&
                   request.ActivityLevel.HasValue;
        }

        /// <summary>
        /// Tính toán và lưu các chỉ số sức khỏe (BMI, TDEE)
        /// </summary>
        private async Task SaveHealthIndicatorsAsync(int userId, HealthProfileRequest request)
        {
            var tdee = _unitOfWork.HealthcareIndicatorRepository.CalculateTDEE(
                request.Weight.Value, request.Height.Value, request.Age.Value,
                request.Gender.ToString().ToLower(), (double)request.ActivityLevel);

            var bmi = _unitOfWork.HealthcareIndicatorRepository.CalculateBMI(
                request.Weight.Value, request.Height.Value);

            var bmiIndicator = CreateHealthIndicator(userId, "Body mass index", "Mass", "BMI", bmi);
            var tdeeIndicator = CreateHealthIndicator(userId, "Total daily energy expenditure", "Energy", "TDEE", tdee);

            await _unitOfWork.HealthcareIndicatorRepository.AddAsync(bmiIndicator);
            await _unitOfWork.HealthcareIndicatorRepository.AddAsync(tdeeIndicator);
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

        private async Task AddUserAllergiesAsync(User existingUser, List<string> allergyNames)
        {
            if (allergyNames == null || !allergyNames.Any()) return;

            foreach (var allergyName in allergyNames)
            {
                var existingAllergy = await _unitOfWork.AllergyRepository
                    .GetByWhere(a => a.AllergyName.ToLower() == allergyName.ToLower())
                    .FirstOrDefaultAsync();

                if (existingAllergy == null)
                {
                    throw new Exception($"Allergy '{allergyName}' does not exist in the system.");
                }

                if (!existingUser.Allergies.Any(a => a.AllergyId == existingAllergy.AllergyId))
                {
                    existingUser.Allergies.Add(existingAllergy);
                }
            }
        }

        private async Task AddUserDiseasesAsync(User existingUser, List<string> diseaseNames)
        {
            if (diseaseNames == null || !diseaseNames.Any()) return;

            foreach (var diseaseName in diseaseNames)
            {
                var existingDisease = await _unitOfWork.DiseaseRepository
                    .GetByWhere(d => d.DiseaseName.ToLower() == diseaseName.ToLower())
                    .FirstOrDefaultAsync();

                if (existingDisease == null)
                {
                    throw new Exception($"Disease '{diseaseName}' does not exist in the system.");
                }

                if (!existingUser.Diseases.Any(d => d.DiseaseId == existingDisease.DiseaseId))
                {
                    existingUser.Diseases.Add(existingDisease);
                }
            }
        }


        public async Task<IBusinessResult> GetHealthProfile()
        {
            var userid = int.Parse(_userIdClaim);

            var existingUser = await _unitOfWork.UserRepository
                .GetByWhere(u => u.UserId == userid)
                .Include(u => u.Allergies)
                .Include(u => u.Diseases)
                .Include(u => u.HealthcareIndicators)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (existingUser == null)
            {
                throw new Exception("User not exist.");
            }

            var healthProfile = await _unitOfWork.HealthProfileRepository
                .GetByWhere(hp => hp.UserId == userid)
                .OrderBy(hp => hp.CreatedAt)
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

        public async Task<IBusinessResult> TrackingHealthProfile()
        {
            var userId = int.Parse(_userIdClaim);

            var existingUser = await _unitOfWork.UserRepository
                .GetByWhere(u => u.UserId == userId)
                .Include(u => u.Allergies)
                .Include(u => u.Diseases)
                .Include(u => u.HealthcareIndicators)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (existingUser == null)
            {
                throw new Exception("User does not exist.");
            }

            var healthProfiles = await _unitOfWork.HealthProfileRepository
                .GetByWhere(hp => hp.UserId == userId)
                .OrderBy(hp => hp.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

            if (healthProfiles == null || !healthProfiles.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "No health history found.");
            }

            List<HealthProfileResponse> responseList = new();

            foreach (var healthProfile in healthProfiles)
            {
                try
                {
                    var response = existingUser.Adapt<HealthProfileResponse>();
                    healthProfile.Adapt(response);
                    responseList.Add(response);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error mapping HealthProfile to HealthProfileResponse: " + ex.Message, ex);
                }
            }

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, responseList);
        }


    }
}
