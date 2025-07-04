﻿using Azure.Core;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                        .OrderByDescending(hp => hp.CreatedAt)
                        .FirstOrDefaultAsync();
                    var oldestRecord = await _unitOfWork.HealthProfileRepository
                        .GetByWhere(hp => hp.UserId == userId && hp.CreatedAt.HasValue && hp.CreatedAt.Value.Date == today)
                        .OrderBy(hp => hp.CreatedAt) 
                        .FirstOrDefaultAsync();
                    if(oldestRecord != null && oldestRecord.ProfileId == existingRecord?.ProfileId)
                    {
                        await UpdateGoalProgress(request.Weight, userId, true);
                    }else
                    {
                        await UpdateGoalProgress(request.Weight, userId, false);
                    }
                    if (existingRecord != null)
                    {
                        await _unitOfWork.HealthProfileRepository.DeleteAsync(existingRecord);
                    }
                }else
                {
                    await UpdateGoalProgress(request.Weight, userId, false);
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
        private async Task UpdateGoalProgress(double? weight, int userId, bool isfirst)
        {
            var existgoal = await _unitOfWork.PersonalGoalRepository.GetByWhere(pg => pg.UserId == userId).OrderByDescending(pg => pg.CreatedAt).FirstOrDefaultAsync();
            if (existgoal == null)
            {
                return;
            }
            if (isfirst)
            {
                existgoal.ProgressRate = (int)(weight - existgoal.TargetWeight);
                return;
            }
            var newrate = weight - existgoal.TargetWeight.Value;
            var percentage = 100 - (int)((newrate / existgoal.ProgressRate) * 100);
            if(percentage <= 0)
            {
                percentage = 0;
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
                >= 18.5 and < 25 => "Bình thường",
                >= 25 and < 30 => "Thừa cân",
                >= 30 and < 35 => "Béo phì độ 1",
                >= 35 and < 40 => "Béo phì độ 2",
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
                .Include(hp => hp.Aisuggestions)
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
            healthProfile.Aisuggestions = healthProfile.Aisuggestions
                .Where(hi => hi.Type == "DinhDuong" || hi.Type == "LuyenTap" || hi.Type == "LoiSong" || hi.Type == "All")
                .OrderByDescending(hi => hi.CreatedAt)
                .Take(4)
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
                                .Include(hp => hp.Aisuggestions)
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
                await UpdateGoalProgress(latestProfile.Weight, profile.UserId,false);
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

        public async Task<IBusinessResult> CreateAISuggestion(CategoryAdvice adviceCategory = CategoryAdvice.All)
        {
            var userId = int.Parse(_userIdClaim);
            var isPremiumResult = await _unitOfWork.UserPackageRepository.IsUserPremiumAsync(userId);
            if (!isPremiumResult)
                return new BusinessResult(Const.HTTP_STATUS_FORBIDDEN, "Chỉ tài khoản Premium mới sử dụng được tính năng này");

            var userInfo = await _unitOfWork.UserRepository.GetByWhere(x => x.UserId == userId)
                .Include(x => x.GeneralHealthProfiles)
                /* ... các Include khác ... */
                .FirstOrDefaultAsync();

            if (userInfo == null)
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "User not found");

            var userProfile = userInfo.GeneralHealthProfiles.OrderByDescending(p => p.CreatedAt).FirstOrDefault();
            var personalGoal = userInfo.PersonalGoals.OrderByDescending(g => g.CreatedAt).FirstOrDefault();

            var height = userProfile?.Height ?? 0;
            var weight = userProfile?.Weight ?? 0;
            var activityLevel = userProfile?.ActivityLevel ?? "Không xác định";
            var goalType = personalGoal?.GoalType ?? "Không có mục tiêu";

            // Chuẩn bị prompt cho AI
            var categoryAdviceText = adviceCategory == CategoryAdvice.All
                ? "dinh dưỡng, luyện tập và lối sống"
                : adviceCategory switch
                {
                    CategoryAdvice.DinhDuong => "dinh dưỡng",
                    CategoryAdvice.LuyenTap => "luyện tập",
                    CategoryAdvice.LoiSong => "lối sống",
                    _ => "dinh dưỡng, luyện tập và lối sống"
                };

            string input = adviceCategory switch
            {
                CategoryAdvice.LuyenTap => $@"
Bạn là một chuyên gia về luyện tập thể chất. Dựa trên thông tin dưới đây, hãy đưa ra lời khuyên tập luyện phù hợp:
- Chiều cao: {height} cm
- Cân nặng: {weight} kg
- Mức độ vận động: {activityLevel}
- Mục tiêu cá nhân: {goalType}

Hãy đề xuất cụ thể:
- Nên tập những bài nào (ví dụ: đi bộ nhanh, plank, HIIT...),
- Mỗi buổi tập nên kéo dài bao nhiêu phút,
- Nên tập bao nhiêu buổi mỗi tuần.

Giới hạn trong 300–350 từ, chỉ trả lời bằng văn bản thuần.",

                CategoryAdvice.DinhDuong => $@"
Bạn là một chuyên gia về dinh dưỡng. Dựa trên thông tin dưới đây, hãy đưa ra lời khuyên ăn uống phù hợp:
- Chiều cao: {height} cm
- Cân nặng: {weight} kg
- Mức độ vận động: {activityLevel}
- Mục tiêu cá nhân: {goalType}

Hãy gợi ý cụ thể:
- Nên ăn bao nhiêu bữa trong một ngày,
- Mỗi bữa nên gồm những nhóm thực phẩm nào (ví dụ: tinh bột, protein, rau xanh...),
- Đưa ra khẩu phần mẫu cho một ngày phù hợp.

Giới hạn trong 300–350 từ, chỉ trả lời bằng văn bản thuần.",

                CategoryAdvice.LoiSong => $@"
Bạn là một chuyên gia về lối sống lành mạnh. Dựa trên thông tin dưới đây, hãy đưa ra lời khuyên phù hợp:
- Chiều cao: {height} cm
- Cân nặng: {weight} kg
- Mức độ vận động: {activityLevel}
- Mục tiêu cá nhân: {goalType}

Hãy tư vấn cụ thể:
- Các thói quen tốt nên duy trì (giấc ngủ, uống nước, giảm stress...),
- Thời gian biểu khuyến nghị trong ngày,
- Các hành vi nên tránh hoặc điều chỉnh.

Giới hạn trong 300–350 từ, chỉ trả lời bằng văn bản thuần.",

                _ => $@"
Bạn là một chuyên gia toàn diện về dinh dưỡng, luyện tập và lối sống. Dựa trên thông tin dưới đây, hãy đưa ra lời khuyên tổng quát phù hợp:
- Chiều cao: {height} cm
- Cân nặng: {weight} kg
- Mức độ vận động: {activityLevel}
- Mục tiêu cá nhân: {goalType}

Hãy chia thành 3 phần rõ ràng:
1. Dinh dưỡng: số bữa, loại thực phẩm, khẩu phần mẫu.
2. Luyện tập: bài tập cụ thể, thời lượng, tần suất/tuần.
3. Lối sống: thói quen tốt, thời gian biểu, điều chỉnh cần thiết.

Giới hạn trong 300 từ, chỉ trả lời bằng văn bản thuần."
            };

            var airesponse = await _aiGeneratorService.AIResponseText(input);

            var healthProfileRecord = await _unitOfWork.HealthProfileRepository
                .GetByWhere(hp => hp.UserId == userId)
                .Include(hp => hp.Aisuggestions)
                .OrderByDescending(hp => hp.CreatedAt)
                .FirstOrDefaultAsync();

            if (healthProfileRecord == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Profile not found");
            }
            else
            {
                healthProfileRecord.UpdatedAt = DateTime.Now;
                await _unitOfWork.HealthProfileRepository.UpdateAsync(healthProfileRecord);
            }
            var typeString = adviceCategory.ToString();
            var old = healthProfileRecord.Aisuggestions.FirstOrDefault(s => s.Type == typeString);
            if (old != null)
            {
                healthProfileRecord.Aisuggestions.Remove(old);
            }
            var aiSuggestion = new Aisuggestion
            {
                ProfileId = healthProfileRecord.ProfileId,
                Content = airesponse,
                Type = adviceCategory.ToString(),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            healthProfileRecord.Aisuggestions.Add(aiSuggestion);
            await _unitOfWork.SaveChangesAsync();
            return new BusinessResult(
                Const.HTTP_STATUS_OK,
                "Lời khuyên tư vấn đã được tạo và lưu thành công",
                airesponse
            );
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
