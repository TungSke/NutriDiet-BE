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
                .Include(u => u.GeneralHealthProfiles)
                .FirstOrDefaultAsync();

            if (existingUser == null)
            {
                throw new Exception("User does not exist.");
            }

            if (request.Weight != null)
            {
                await UpdateGoalProgress(request.Weight, userId);
            }
            
            // Chuyển đổi request thành GeneralHealthProfile
            var healthProfile = request.Adapt<GeneralHealthProfile>();
            healthProfile.CreatedAt = DateTime.Now;
            healthProfile.UpdatedAt = DateTime.Now;
            healthProfile.UserId = userId;
            healthProfile.Evaluate = "";
            if (request.Weight.HasValue && request.Height.HasValue)
            {
                double percentile = CalculateGlobalWeightPercentile(request.Weight.Value);
                double weightChange = CalculateWeightChangeForNormalBMI(request.Weight.Value, request.Height.Value);
                string abnormal = ValidateHealthProfileData(request.Weight.Value, request.Height.Value);
                string weightChangeMessage = "";
                if (weightChange > 0)
                {
                    weightChangeMessage = $"Bạn cần tăng {weightChange}kg để đạt trạng thái cân đối";
                }
                else if (weightChange < 0)
                {
                    weightChangeMessage = $"Bạn cần giảm {Math.Abs(weightChange)}kg để đạt trạng thái cân đối";
                }
                else
                {
                    weightChangeMessage = "Bạn đã đạt trạng thái cân đối";
                }

                healthProfile.Evaluate = $"Bạn nặng hơn {percentile}% người trên thế giới. {weightChangeMessage}. {abnormal}";
            }
            await _unitOfWork.BeginTransaction();
            try 
            {
                if (request.ProfileOption.Equals(ProfileOption.REPLACE))
                {
                    var today = DateTime.Today.Date;
                    var existingRecord = await _unitOfWork.HealthProfileRepository
                        .GetByWhere(hp => hp.UserId == userId && hp.CreatedAt.Value.Date == today)
                        .FirstOrDefaultAsync();
                    if (existingRecord != null)
                    {
                        await _unitOfWork.HealthProfileRepository.DeleteAsync(existingRecord);
                    }
                }
                await _unitOfWork.HealthProfileRepository.AddAsync(healthProfile);
                if (IsValidHealthData(request))
                {
                    await SaveHealthIndicatorsAsync(healthProfile, request, existingUser);
                }

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
        public async Task<bool> HasCreatedHealthProfileToday()
        {
            var userId = int.Parse(_userIdClaim);
            var today = DateTime.Today.Date;

            var existingRecord = await _unitOfWork.HealthProfileRepository
                .GetByWhere(hp => hp.UserId == userId && hp.CreatedAt.HasValue && hp.CreatedAt.Value.Date == today)
                .FirstOrDefaultAsync();
            return existingRecord != null;
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
            }else if(percentage >= 100)
            {
                existgoal.GoalType = GoalType.Maintain.ToString();
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
        private bool IsValidHealthData(HealthProfileRequest request)
        {
            return request.Weight.HasValue && request.Height.HasValue &&
                   request.ActivityLevel.HasValue;
        }
        private async Task SaveHealthIndicatorsAsync(GeneralHealthProfile healthProfile, HealthProfileRequest request, User user)
        {
            // Tính toán các chỉ số sức khỏe
            var tdee = _unitOfWork.HealthcareIndicatorRepository.CalculateTDEE(
                request.Weight.Value, request.Height.Value, user.Age.Value,
                user.Gender.ToString().ToLower(), (double)request.ActivityLevel);

            var bmi = _unitOfWork.HealthcareIndicatorRepository.CalculateBMI(
                request.Weight.Value, request.Height.Value);

            // Phân loại BMI
            var bmiCategory = GetBMICategory(bmi);

            // Tạo các đối tượng HealthcareIndicator
            var bmiIndicator = CreateHealthIndicator(healthProfile.ProfileId, "Body mass index", bmiCategory, "BMI", bmi);
            var tdeeIndicator = CreateHealthIndicator(healthProfile.ProfileId, "Total daily energy expenditure", "Energy", "TDEE", tdee);

            if (healthProfile.HealthcareIndicators == null)
            {
                healthProfile.HealthcareIndicators = new List<HealthcareIndicator>();
            }
            healthProfile.HealthcareIndicators.Add(bmiIndicator);
            healthProfile.HealthcareIndicators.Add(tdeeIndicator);
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
        private HealthcareIndicator CreateHealthIndicator(int profileId, string name, string type, string code, double value)
        {
            return new HealthcareIndicatorRequest
            {
                ProfileID = profileId,
                Name = name,
                Type = type,
                Code = code,
                CurrentValue = value
            }.Adapt<HealthcareIndicator>();
        }
        public async Task<IBusinessResult> GetHealthProfile()
        {
            var userId = int.Parse(_userIdClaim);

            // Lấy hồ sơ sức khỏe mới nhất kèm theo danh sách HealthcareIndicator
            var healthProfile = await _unitOfWork.HealthProfileRepository
                .GetByWhere(hp => hp.UserId == userId)
                .Include(hp => hp.HealthcareIndicators) // giả sử có navigation property
                .OrderByDescending(hp => hp.CreatedAt)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (healthProfile == null)
            {
                throw new Exception("Health profile not exist.");
            }

            // Lọc chỉ số sức khỏe (BMI, TDEE)
            healthProfile.HealthcareIndicators = healthProfile.HealthcareIndicators
                .Where(hi => hi.Code == "BMI" || hi.Code == "TDEE")
                .OrderByDescending(hi => hi.CreatedAt)
                .Take(2)
                .ToList();

            // Lấy thông tin user để mapping sang response
            var user = await _unitOfWork.UserRepository
                .GetByWhere(u => u.UserId == userId)
                .Include(u => u.Allergies).ThenInclude(a => a.Ingredients)
                .Include(u => u.Diseases).ThenInclude(a => a.Ingredients)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new Exception("User not exist.");
            }

            var response = user.Adapt<HealthProfileResponse>();
            healthProfile.Adapt(response);

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }
        public async Task<IBusinessResult> DeleteHealthProfile(int userId)
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
                    hp.ProfileId,
                    hp.CreatedAt,
                    Value = EF.Property<object>(hp, field.ToString()),
                    hp.ImageUrl
                })
                .AsNoTracking()
                .ToListAsync();

            if (healthProfiles == null || !healthProfiles.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "No health history found for the requested field.");
            }

            var response = healthProfiles.Select(hp => new
            {
                ProfileId = hp.ProfileId,
                Date = hp.CreatedAt,
                Value = hp.Value,
                ImageUrl = hp.ImageUrl
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
        public async Task<IBusinessResult> GetHealthProfiles()
        {
            var userId = int.Parse(_userIdClaim);

            var profiles = await _unitOfWork.HealthProfileRepository
                                .GetByWhere(hp => hp.UserId == userId)
                                .Include(hp => hp.HealthcareIndicators)
                                .OrderByDescending(hp => hp.CreatedAt)
                                .AsNoTracking()
                                .ToListAsync();

            if (profiles == null || !profiles.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Không tìm thấy hồ sơ sức khỏe nào");
            }

            var response = profiles.Adapt<List<HealthProfileResponse>>();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }
        public async Task<IBusinessResult> DeleteProfileById(int profileId)
        {
            var profile = await _unitOfWork.HealthProfileRepository
                                .GetByWhere(p => p.ProfileId == profileId)
                                .Include(p => p.HealthcareIndicators)
                                .FirstOrDefaultAsync();

            if (profile == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Profile not found");
            }

            var firstProfile = await _unitOfWork.HealthProfileRepository
                                   .GetByWhere(p => p.UserId == profile.UserId)
                                   .OrderBy(p => p.CreatedAt)
                                   .FirstOrDefaultAsync();

            if (firstProfile != null && firstProfile.ProfileId == profile.ProfileId)
            {
                return new BusinessResult(Const.HTTP_STATUS_FORBIDDEN, "Không thể xóa hồ sơ đầu tiên của người dùng");
            }

            await _unitOfWork.HealthProfileRepository.DeleteAsync(profile);
            await _unitOfWork.SaveChangesAsync();

            var latestProfile = await _unitOfWork.HealthProfileRepository
                                        .GetByWhere(hp => hp.UserId == profile.UserId && hp.Weight.HasValue)
                                        .OrderByDescending(hp => hp.CreatedAt)
                                        .FirstOrDefaultAsync();

            if (latestProfile != null)
            {
                await UpdateGoalProgress(latestProfile.Weight, profile.UserId);
                await _unitOfWork.SaveChangesAsync();
            }

            return new BusinessResult(Const.HTTP_STATUS_OK, "Profile deleted successfully along with its healthcare indicators");
        }
        public async Task<IBusinessResult> AddImageToHealthProfile(int profileId, AddImageRequest request)
        {
            if (request.Image == null || request.Image.Length == 0)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Image file is required.", null);
            }

            int userId = int.Parse(_userIdClaim);

            var profile = await _unitOfWork.HealthProfileRepository
                .GetByWhere(p => p.ProfileId == profileId && p.UserId == userId)
                .FirstOrDefaultAsync();

            if (profile == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Health profile not found.", null);
            }

            try
            {
                var cloudinaryHelper = new CloudinaryHelper();

                if (!string.IsNullOrEmpty(profile.ImageUrl))
                {
                    var publicId = ExtractPublicIdFromUrl(profile.ImageUrl);
                    profile.ImageUrl = await cloudinaryHelper.UpdateImageWithCloudinary(publicId, request.Image);
                }
                else
                {
                    profile.ImageUrl = await cloudinaryHelper.UploadImageWithCloudDinary(request.Image);
                }

                profile.UpdatedAt = DateTime.Now;
            }
            catch (Exception)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, "Error uploading image.", null);
            }

            await _unitOfWork.HealthProfileRepository.UpdateAsync(profile);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, "Image added/replaced successfully.", profile.ImageUrl);
        }
        private string ExtractPublicIdFromUrl(string imageUrl)
        {
            var uri = new Uri(imageUrl);
            var segments = uri.AbsolutePath.Split('/');
            var folderIndex = Array.IndexOf(segments, "upload") + 1;
            var pathParts = segments.Skip(folderIndex).ToArray();

            var fullPath = string.Join("/", pathParts);
            return Path.Combine(Path.GetDirectoryName(fullPath) ?? "", Path.GetFileNameWithoutExtension(fullPath)).Replace("\\", "/");
        }
        public async Task<IBusinessResult> DeleteImageFromHealthProfile(int profileId)
        {
            int userId = int.Parse(_userIdClaim);

            var profile = await _unitOfWork.HealthProfileRepository
                .GetByWhere(p => p.ProfileId == profileId && p.UserId == userId)
                .FirstOrDefaultAsync();

            if (profile == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Health profile not found.", null);
            }

            if (string.IsNullOrEmpty(profile.ImageUrl))
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "No image to delete.", null);
            }

            try
            {
                var cloudinaryHelper = new CloudinaryHelper();

                // Xoá ảnh trên Cloudinary
                //var publicId = ExtractPublicIdFromUrl(profile.ImageUrl);
                //await cloudinaryHelper.DeleteImage(publicId);

                // Xoá ảnh trong hệ thống
                profile.ImageUrl = null;
                profile.UpdatedAt = DateTime.Now;

                await _unitOfWork.HealthProfileRepository.UpdateAsync(profile);
                await _unitOfWork.SaveChangesAsync();

                return new BusinessResult(Const.HTTP_STATUS_OK, "Image deleted successfully.");
            }
            catch (Exception)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, "Error deleting image.", null);
            }
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

            var input = $@"Bạn là một chuyên gia dinh dưỡng và sức khỏe. Nhiệm vụ của bạn là đưa ra lời khuyên chi tiết giúp cải thiện sức khỏe cho người dùng dựa trên hồ sơ sức khỏe và mục tiêu cá nhân của họ.

Thông tin người dùng:
- Họ tên: {userInfo.FullName}
- Giới tính: {userInfo.Gender}
- Tuổi: {userInfo.Age}
- Chiều cao: {height} cm
- Cân nặng: {weight} kg
- Mức độ vận động: {activityLevel}
- Mục tiêu: {goalType}

Yêu cầu:
- Cung cấp lời khuyên chi tiết để cải thiện sức khỏe, bao gồm các khuyến nghị về dinh dưỡng, luyện tập và lối sống.
- Lưu ý các dị ứng thực phẩm: {formattedAllergies}
- Lưu ý các bệnh lý: {formattedDiseases}

Lưu ý:
- Chỉ trả về **text thuần túy**, không dưới dạng JSON và không kèm theo giải thích thêm.
";

            // Gọi dịch vụ AI để nhận phản hồi dạng text (giả sử bạn có phương thức AIResponseText)
            var airesponse = await _aiGeneratorService.AIResponseText(input);

            // Nếu bạn không có phương thức AIResponseText, bạn có thể sử dụng AIResponseJson nhưng cần thay đổi prompt như trên
            // var airesponse = await _aiGeneratorService.AIResponseJson(input, "");

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
        public double CalculateGlobalWeightPercentile(double weight)
        {
            double GlobalMeanWeight = 62.0;
            double GlobalStdDev = 15.0;
            double z = (weight - GlobalMeanWeight) / GlobalStdDev;
            double percentile = NormalCdf(z) * 100.0;
            return Math.Round(percentile, 2);
        }
        private double NormalCdf(double z)
        {
            return 0.5 * (1.0 + Erf(z / Math.Sqrt(2)));
        }
        private double Erf(double x)
        {
            int sign = Math.Sign(x);
            x = Math.Abs(x);
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;
            double t = 1.0 / (1.0 + p * x);
            double y = 1.0 - ((((a5 * t + a4) * t + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);
            return sign * y;
        }
        public double CalculateWeightChangeForNormalBMI(double currentWeight, double heightCm)
        {
            double heightM = heightCm / 100.0;

            double currentBMI = currentWeight / (heightM * heightM);
            if (currentBMI >= 18.5 && currentBMI <= 24.9)
            {
                return 0;
            }
            else if (currentBMI < 18.5)
            {
                double targetWeight = 18.5 * (heightM * heightM);
                double weightToGain = targetWeight - currentWeight;
                return Math.Round(weightToGain, 2);
            }
            else
            {
                double targetWeight = 24.9 * (heightM * heightM);
                double weightToLose = targetWeight - currentWeight;
                return Math.Round(weightToLose, 2);
            }
        }
        public string ValidateHealthProfileData(double currentWeight, double heightCm)
        {
            string message = "";

            double heightM = heightCm / 100.0;
            double currentBMI = currentWeight / (heightM * heightM);
            if (currentBMI < 10 || currentBMI > 50)
            {
                message = ("Chỉ số cơ thể của bạn hiện tại rất bất thường.");
            }
            return message; 
        }
    }
}
