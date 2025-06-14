﻿using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using NutriDiet.Service.Helpers;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using NutriDiet.Service.ModelDTOs.Response;
using NutriDiet.Service.Utilities;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using NutriDiet.Repository;
using NutriDiet.Service.Enums;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Common;
using Google.Apis.Auth;
using Azure.Core;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace NutriDiet.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PasswordHasher<string> _passwordHasher;
        private readonly TokenHandlerHelper _tokenHandler;
        private readonly GoogleService _googleService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userIdClaim;
        private readonly HttpClient _httpClient = new HttpClient();

        public UserService(IUnitOfWork unitOfWork, GoogleService googleService, TokenHandlerHelper tokenHandlerHelper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork ??= unitOfWork;
            _passwordHasher = new PasswordHasher<string>();
            _tokenHandler = tokenHandlerHelper;
            _googleService = googleService;
            _httpContextAccessor = httpContextAccessor;
            _userIdClaim = GetUserIdClaim();
        }
        private string GetUserIdClaim()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }
        private string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(null, password);
        }

        public async Task<User> findUserByEmail(string email)
        {
            return await _unitOfWork.UserRepository.GetByWhere(x => x.Email == email).Include(x => x.Role).FirstOrDefaultAsync();
        }

        public async Task<User> findUserById(int id)
        {
            return await _unitOfWork.UserRepository.GetByWhere(x => x.UserId == id).Include(x => x.Role).Include(x => x.UserPackages).ThenInclude(x => x.Package).FirstOrDefaultAsync();
        }

        public async Task<IBusinessResult> Register(RegisterRequest request)
        {

            var checkUser = await findUserByEmail(request.Email);
            if (checkUser != null)
            {
                if (checkUser.Status == UserStatus.Inactive.ToString())
                {
                    await _googleService.SendEmailWithOTP(request.Email, "Mã OTP xác thực tài khoản NutriDiet");
                    return new BusinessResult(Const.HTTP_STATUS_OK, "Tài khoản đã tồn tại nhưng chưa xác thực. OTP mới đã được gửi.");
                }
                return new BusinessResult(Const.HTTP_STATUS_CONFLICT, "Email Existed");
            }
            request.Password = HashPassword(request.Password);
            var acc = request.Adapt<User>();
            acc.FullName = "New User";
            acc.RoleId = (int)RoleEnum.Customer;
            acc.Status = UserStatus.Inactive.ToString();
            acc.Avatar = "";
            await _unitOfWork.UserRepository.AddAsync(acc);
            await _unitOfWork.SaveChangesAsync();

            await _googleService.SendEmailWithOTP(request.Email, "Mã OTP xác thực tài khoản NutriDiet");
            return new BusinessResult(Const.HTTP_STATUS_OK, "Check email to active account");
        }

        public async Task<IBusinessResult> VerifyAccount(VerifyAccountRequest request)
        {
            var isOtpValid = await _googleService.VerifyOtp(request.Email, request.OTP);

            if (!isOtpValid)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "OTP is incorrect");
            }

            var user = await findUserByEmail(request.Email);
            if (user == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "User not found");
            }

            user.Status = UserStatus.Active.ToString();
            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, "Verify success");
        }

        public async Task<IBusinessResult> ResendOTP(ResendOtpRequest request)
        {
            var checkUser = await findUserByEmail(request.Email);
            if (checkUser == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Email not existed, please register first!");
            }
            await _googleService.SendEmailWithOTP(request.Email, "Xác nhận lại OTP NutriDiet");
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG);
        }


        public async Task<IBusinessResult> Login(LoginRequest request)
        {
            var account = await findUserByEmail(request.Email);
            if (account == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Email not Existed");
            }
            else if (account.Status == UserStatus.Inactive.ToString())
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Account is not verified");
            }
            else if (account.Status == UserStatus.Deleted.ToString())
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Account is Deleted");
            }

            var result = _passwordHasher.VerifyHashedPassword(null, account.Password, request.Password);
            if (result != PasswordVerificationResult.Success)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Email or password is incorrect");
            }

            var accessToken = await _tokenHandler.GenerateJwtToken(account);
            var refreshToken = await _tokenHandler.GenerateRefreshToken();

            account.RefreshToken = refreshToken;
            account.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            account.FcmToken = request.fcmToken;

            await _unitOfWork.UserRepository.UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();

            var res = new LoginResponse
            {
                Role = account.Role.RoleName,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, res);

        }

        public async Task<IBusinessResult> LoginWithGoogle(string idToken, string fcmToken)
        {
            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
            }
            catch (Exception)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Invalid Google token.");
            }

            var account = await findUserByEmail(payload.Email);

            if (account == null)
            {
                account = new User
                {
                    FullName = payload.Name ?? "User",
                    Email = payload.Email,
                    Password = HashPassword("tungdeptrai123142"),
                    Avatar = payload.Picture,
                    Status = UserStatus.Active.ToString(),
                    RoleId = (int)RoleEnum.Customer,
                    FcmToken = fcmToken,
                    RefreshToken = await _tokenHandler.GenerateRefreshToken(),
                    RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
                };

                await _unitOfWork.UserRepository.AddAsync(account);
                await _unitOfWork.SaveChangesAsync();

                account = await findUserByEmail(payload.Email);
            }
            else
            {
                if (account.Status == UserStatus.Inactive.ToString())
                {
                    return new BusinessResult(Const.HTTP_STATUS_FORBIDDEN, "Account is inactive, please contact support.");
                }

                account.FcmToken = fcmToken;
                account.RefreshToken = await _tokenHandler.GenerateRefreshToken();
                account.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                await _unitOfWork.UserRepository.UpdateAsync(account);
                await _unitOfWork.SaveChangesAsync();
            }

            var accessToken = await _tokenHandler.GenerateJwtToken(account);

            var response = new LoginResponse
            {
                Role = account.Role.RoleName,
                AccessToken = accessToken,
                RefreshToken = account.RefreshToken
            };

            return new BusinessResult(Const.HTTP_STATUS_OK, "Login successful", response);
        }

        public async Task<IBusinessResult> LoginWithFacebook(string accessToken, string fcmToken)
        {
            try
            {
                var urlConnect = $"https://graph.facebook.com/v21.0/me?fields=id,name,email,picture.width(200).height(200)&access_token={accessToken}";
                var response = await _httpClient.GetAsync(urlConnect);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, $"Invalid Facebook access token. Details: {errorContent}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var userData = JsonConvert.DeserializeObject<JObject>(content);

                var name = userData?["name"]?.ToString() ?? "User";
                var email = userData?["email"]?.ToString();
                var userAvatar = userData?["picture"]?["data"]?["url"]?.ToString();

                if (string.IsNullOrEmpty(email))
                {
                    return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Email not found in Facebook response.");
                }

                var account = await findUserByEmail(email);
                if (account == null)
                {
                    // Tạo tài khoản mới
                    account = new User
                    {
                        FullName = name,
                        Email = email,
                        Password = HashPassword("12345"),
                        Avatar = userAvatar,
                        Status = UserStatus.Active.ToString(),
                        RoleId = (int)RoleEnum.Customer,
                        FcmToken = fcmToken, // Lưu FCM Token khi tạo user mới
                        RefreshToken = await _tokenHandler.GenerateRefreshToken(),
                        RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
                    };

                    await _unitOfWork.UserRepository.AddAsync(account);
                    await _unitOfWork.SaveChangesAsync();
                    account = await findUserByEmail(email);
                }
                else
                {
                    if (account.Status == UserStatus.Inactive.ToString())
                    {
                        return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Account is deleted, please contact admin to restore.");
                    }

                    account.FcmToken = fcmToken;
                    account.RefreshToken = account.RefreshToken ?? await _tokenHandler.GenerateRefreshToken();
                    account.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                    await _unitOfWork.UserRepository.UpdateAsync(account);
                }

                await _unitOfWork.SaveChangesAsync();

                var accessTokenRes = await _tokenHandler.GenerateJwtToken(account);

                var loginResponse = new LoginResponse
                {
                    Role = account.Role.RoleName,
                    AccessToken = accessTokenRes,
                    RefreshToken = account.RefreshToken
                };

                return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, loginResponse);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, ex.Message);
            }
        }


        public async Task<IBusinessResult> ForgotPassword(string email)
        {
            var user = await findUserByEmail(email);
            if (user == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Email not existed");
            }

            await _googleService.SendEmailWithOTP(email, "Reset mật khẩu cho tài khoản NutriDiet");

            return new BusinessResult(Const.HTTP_STATUS_OK, "Check email to reset password");
        }

        public async Task<IBusinessResult> ResetPassword(ResetPasswordRequest request)
        {
            var user = await findUserByEmail(request.Email);
            if (user == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Email not existed");
            }

            var isOtpValid = await _googleService.VerifyOtp(request.Email, request.OTP);
            if (!isOtpValid)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "OTP is incorrect");
            }

            user.Password = HashPassword(request.NewPassword);
            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, "Reset password success");
        }
        public async Task<IBusinessResult> SearchUser(int pageIndex, int pageSize, string status, string search)
        {
            search = search?.ToLower() ?? string.Empty;

            var users = await _unitOfWork.UserRepository.GetPagedAsync(
                pageIndex,
                pageSize,
                x => (string.IsNullOrEmpty(status) || x.Status.ToLower() == status.ToLower()) &&
                      (string.IsNullOrEmpty(search) || x.FullName.ToLower().Contains(search)
                                                   || x.Email.ToLower().Contains(search)
                                                   || x.Phone.ToLower().Contains(search)) &&
                                                   x.RoleId != 1,
                include: i=>i.Include(x=>x.Role).Include(x=>x.UserPackages).ThenInclude(x=>x.Package)
            );

            if (users == null || !users.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
            }
            
            var response = users.Adapt<List<UserResponse>>().ToList();
            

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }
        public async Task<IBusinessResult> GetUserById(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByWhere(x=>x.UserId == id).Include(x=>x.UserPackages).ThenInclude(x=>x.Package).FirstOrDefaultAsync();
            if (user == null || user.RoleId == 1)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
            }
            var response = user.Adapt<UserResponse>();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> RefreshToken(RefreshTokenRequest request)
        {
            var user = await _unitOfWork.UserRepository.GetByWhere(x => x.RefreshToken.Equals(request.RefreshToken)).Include(x => x.Role).FirstOrDefaultAsync();

            if (user == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Refresh token is invalid");
            }
            if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _unitOfWork.UserRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return new BusinessResult(Const.HTTP_STATUS_UNAUTHORIZED, "Refresh token is expired, please login again");
            }

            var newAccessToken = await _tokenHandler.GenerateJwtToken(user);
            var newRefreshToken = await _tokenHandler.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var response = new LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Role = user.Role.RoleName
            };

            return new BusinessResult(Const.HTTP_STATUS_OK, "Refresh token success", response);
        }

        public async Task<IBusinessResult> UpdateUser(UpdateUserRequest request)
        {
            var userID = int.Parse(_userIdClaim);
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userID);
            if (user == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
            }
            user.FullName = request.FullName;
            user.Age = request.Age;
            user.Gender = request.Gender.ToString();
            user.Location = request.Location;
            if(request.Avatar != null)
            {
                CloudinaryHelper cloudinary = new CloudinaryHelper();
                user.Avatar = await cloudinary.UploadImageWithCloudDinary(request.Avatar);           
            }

            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_UPDATE_MSG);
        }

        public async Task<IBusinessResult> UpgradePackage(int packageId)
        {
            try
            {
                int userId = int.Parse(_userIdClaim);
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
                }
                var package = await _unitOfWork.PackageRepository.GetByIdAsync(packageId);
                if (package == null)
                {
                    return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
                }

                var existingPackage = await _unitOfWork.UserPackageRepository
                    .GetByWhere(x => x.UserId == userId && x.Status == "Active" && x.ExpiryDate > DateTime.UtcNow)
                    .FirstOrDefaultAsync();
                if (existingPackage != null)
                {
                    return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Bạn đã có gói Premium đang hoạt động");
                }

                var userPackage = new UserPackage
                {
                    UserId = userId,
                    PackageId = packageId,
                    StartDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(package.Duration ?? 0),
                    Status = "Active"
                };
                await _unitOfWork.UserPackageRepository.AddAsync(userPackage);
                await _unitOfWork.SaveChangesAsync();
                var response = userPackage.Adapt<UserPackageResponse>();
                response.FullName = user.FullName;
                response.PackageName = package.PackageName;
                return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_CREATE_MSG, response);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.HTTP_STATUS_INTERNAL_ERROR, "Lỗi server: " + ex.Message);
            }
        }

        public async Task<IBusinessResult> IsPremium()
        {
            int userId = int.Parse(_userIdClaim);

            var isPremium = await _unitOfWork.UserPackageRepository.IsUserPremiumAsync(userId);

            var isAdvancedPremium = await _unitOfWork.UserPackageRepository.IsUserAdvancedPremiumAsync(userId);

            // Xác định loại gói
            string packageType = "None";
            if (isAdvancedPremium)
            {
                packageType = "Advanced";
            }
            else if (isPremium)
            {
                packageType = "Basic";
            }

            // Cập nhật trạng thái gói hết hạn
            var expiredPackages = await _unitOfWork.UserPackageRepository
                .GetByWhere(up => up.UserId == userId && up.Status == "Active" && up.ExpiryDate <= DateTime.UtcNow)
                .ToListAsync();
            if (expiredPackages.Any())
            {
                foreach (var package in expiredPackages)
                {
                    package.Status = "Inactive";
                    await _unitOfWork.UserPackageRepository.UpdateAsync(package);
                }
                await _unitOfWork.SaveChangesAsync();
            }

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, isPremium);
        }
        public async Task<IBusinessResult> IsAdvancedPremium()
        {
            int userId = int.Parse(_userIdClaim);

            // Kiểm tra trạng thái Premium
            var isPremium = await _unitOfWork.UserPackageRepository.IsUserPremiumAsync(userId);
            var isAdvancedPremium = await _unitOfWork.UserPackageRepository.IsUserAdvancedPremiumAsync(userId);

            // Xác định loại gói
            string packageType = "None";
            if (isAdvancedPremium)
            {
                packageType = "Advanced";
            }
            else if (isPremium)
            {
                packageType = "Basic";
            }

            // Cập nhật trạng thái gói hết hạn
            var expiredPackages = await _unitOfWork.UserPackageRepository
                .GetByWhere(up => up.UserId == userId && up.Status == "Active" && up.ExpiryDate <= DateTime.UtcNow)
                .ToListAsync();
            if (expiredPackages.Any())
            {
                foreach (var package in expiredPackages)
                {
                    package.Status = "Inactive";
                    await _unitOfWork.UserPackageRepository.UpdateAsync(package);
                }
                await _unitOfWork.SaveChangesAsync();
                // Cập nhật lại trạng thái sau khi gói hết hạn
                isPremium = await _unitOfWork.UserPackageRepository.IsUserPremiumAsync(userId);
                isAdvancedPremium = await _unitOfWork.UserPackageRepository.IsUserAdvancedPremiumAsync(userId);
                packageType = isAdvancedPremium ? "Advanced" : (isPremium ? "Basic" : "None");
            }

            var result = new
            {
                IsPremium = isPremium,
                PackageType = packageType
            };

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, result);
        }

        public async Task<IBusinessResult> UpdateStatusUser(int userId, UserStatus status)
        {
            var userExisted = await _unitOfWork.UserRepository.GetByIdAsync(userId);

            if (userExisted == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
            }

            userExisted.Status = status.ToString();
            await _unitOfWork.UserRepository.UpdateAsync(userExisted);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, "Cập nhật trạng thái người dùng thành công", userExisted.Status);

        }

        public async Task<IBusinessResult> ChangeRole(int userId, RoleEnum role)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
            }

            if(role != RoleEnum.Customer && role != RoleEnum.Nutritionist)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Invalid role");
            }

            user.RoleId = (int)role;
            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, "User role updated successfully");
        }
    }
}
