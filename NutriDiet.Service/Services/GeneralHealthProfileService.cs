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
using NutriDiet.Service.Utilities;
using System;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace NutriDiet.Service.Services
{
    public class GeneralHealthProfileService : IGeneralHealthProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userIdClaim;
        private readonly AIGeneratorService _aiGeneratorService;

        public GeneralHealthProfileService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, AIGeneratorService aIGeneratorService)
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
            // Cập nhật tiến trình giảm cân
            if (request.Weight != null)
            {
                await UpdateGoalProgress(request.Weight, userId);
            }

            if (IsValidHealthData(request))
            {
                await SaveHealthIndicatorsAsync(userId, request, existingUser);
            }

            await _unitOfWork.BeginTransaction();
            try
            {
                // Kiểm tra nếu đã tồn tại record HealthProfile với CreatedAt là ngày hôm nay thì xóa trước
                var today = DateTime.Today.Date;
                var existingRecord = await _unitOfWork.HealthProfileRepository
                    .GetByWhere(hp => hp.UserId == userId && hp.CreatedAt.Value.Date == today)
                    .FirstOrDefaultAsync();

                if (existingRecord != null)
                {
                    await _unitOfWork.HealthProfileRepository.DeleteAsync(existingRecord);
                }
                // Lưu hồ sơ sức khỏe mới
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
                .Include(u => u.Allergies).ThenInclude(a => a.Ingredients)
                .Include(u => u.Diseases).ThenInclude(a => a.Ingredients)
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

        public async Task<IBusinessResult> GetSuggestionImproveFromAI()
        {
            var userId = int.Parse(_userIdClaim);

            var healthProfiles = await _unitOfWork.HealthProfileRepository
                .GetByWhere(hp => hp.UserId == userId)
                .OrderBy(hp => hp.CreatedAt)
                .Select(hp => new
                {
                    hp.CreatedAt,
                    
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG);
        }

        public async Task<IBusinessResult> CreateAISuggestion()
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

            // Định nghĩa JSON mẫu cho lời khuyên cải thiện sức khỏe
            var jsonSampleOutput = JsonSerializer.Serialize(new
            {
                advice = "Ví dụ: Hãy duy trì chế độ ăn cân bằng, tập thể dục đều đặn và ngủ đủ giấc.",
                tips = new string[] {
            "Tăng cường rau xanh",
            "Giảm đường và chất béo bão hòa",
            "Tham gia hoạt động thể chất hàng ngày từ 30 phút đến 1 tiếng"
        }
            });

            // Xây dựng input cho AI với thông tin người dùng và yêu cầu cải thiện sức khỏe
            var input = $@"Bạn là một chuyên gia dinh dưỡng và sức khỏe. Nhiệm vụ của bạn là đưa ra lời khuyên để cải thiện sức khỏe cho người dùng dựa trên hồ sơ sức khỏe và mục tiêu cá nhân của họ.

Thông tin người dùng:
- **Họ tên:** {userInfo.FullName}
- **Giới tính:** {userInfo.Gender}
- **Tuổi:** {userInfo.Age}
- **Chiều cao:** {height} cm
- **Cân nặng:** {weight} kg
- **Mức độ vận động:** {activityLevel}
- **Mục tiêu:** {goalType}

Yêu cầu:
- Cung cấp lời khuyên chi tiết để cải thiện sức khỏe, bao gồm các khuyến nghị về dinh dưỡng, luyện tập và lối sống.
- Lưu ý các dị ứng thực phẩm: {formattedAllergies}
- Lưu ý các bệnh lý: {formattedDiseases}

Lưu ý:
- Chỉ trả về **JSON thuần túy**, không kèm theo giải thích.
";

            // Gọi dịch vụ AI để nhận phản hồi dưới dạng JSON
            var airesponse = await _aiGeneratorService.AIResponseJson(input, jsonSampleOutput);

            // Tìm bản ghi GeneralHealthProfile gần nhất của người dùng
            var healthProfileRecord = await _unitOfWork.HealthProfileRepository
                                          .GetByWhere(hp => hp.UserId == userId)
                                          .OrderByDescending(hp => hp.CreatedAt)
                                          .FirstOrDefaultAsync();

            if (healthProfileRecord == null)
            {
                // Nếu không tồn tại, tạo mới bản ghi với Aisuggestion
                healthProfileRecord = new GeneralHealthProfile
                {
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Aisuggestion = airesponse
                };

                await _unitOfWork.HealthProfileRepository.AddAsync(healthProfileRecord);
            }
            else
            {
                // Cập nhật trường Aisuggestion của bản ghi hiện có
                healthProfileRecord.Aisuggestion = airesponse;
                healthProfileRecord.UpdatedAt = DateTime.Now;
                await _unitOfWork.HealthProfileRepository.UpdateAsync(healthProfileRecord);
            }

            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, "Lời khuyên tư vấn đã được tạo và lưu thành công", airesponse);
        }


    }
}
