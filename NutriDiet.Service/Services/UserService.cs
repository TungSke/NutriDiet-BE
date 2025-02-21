using NutriDiet.Repository.Interface;
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
using System.Net.Http;
using CloudinaryDotNet;
using System.Runtime.CompilerServices;

namespace NutriDiet.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PasswordHasher<string> _passwordHasher;
        private readonly TokenHandlerHelper _tokenHandler;
        private readonly GoogleService _googleService;
        private readonly HttpClient _httpClient = new HttpClient();

        public UserService(IUnitOfWork unitOfWork, GoogleService googleService, TokenHandlerHelper tokenHandlerHelper)
        {
            _unitOfWork ??= unitOfWork;
            _passwordHasher = new PasswordHasher<string>();
            _tokenHandler = tokenHandlerHelper;
            _googleService = googleService;
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
            return await _unitOfWork.UserRepository.GetByWhere(x => x.UserId == id).Include(x => x.Role).FirstOrDefaultAsync();
        }

        public async Task<IBusinessResult> Register(RegisterRequest request)
        {

            var checkUser = await findUserByEmail(request.Email);
            if (checkUser != null)
            {
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
            await _googleService.SendEmailWithOTP(request.Email,"Xác nhận lại OTP NutriDiet");
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

            if (result == PasswordVerificationResult.Success)
            {
                var res = new LoginResponse
                {
                    Role = account.Role.RoleName,
                    Token = _tokenHandler.GenerateJwtToken(account).Result
                };
                return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, res);
            }
            else
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Password is incorrect");
            }
        }

        public async Task<IBusinessResult> LoginWithGoogle(string idToken)
        {
            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
            }
            catch (Exception)
            {
                throw new Exception("Invalid Google token.");
            }

            var account = await findUserByEmail(payload.Email);
            
            if (account == null)
            {
                account = new User
                {
                    FullName = payload.Name ?? "User",
                    Email = payload.Email,
                    Password = HashPassword("12345"),
                    Avatar = payload.Picture,
                    Status = "ACTIVE",
                    RoleId = (int)RoleEnum.Customer
                };
                await _unitOfWork.UserRepository.AddAsync(account);
                await _unitOfWork.SaveChangesAsync();

                //get again
                account = await findUserByEmail(payload.Email);
                var res = new LoginResponse
                {
                    Role = account.Role.RoleName,
                    Token = _tokenHandler.GenerateJwtToken(account).Result
                };
                return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, res);
            }
            else if (account.Status == "INACTIVE")
            {
                throw new Exception("Account is deleted, please contact admin to restore");
            }

            return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, Const.FAIL_READ_MSG);
        }

        public async Task<IBusinessResult> LoginWithFacebook(string accessToken)
        {
            try
            {
                // Kiểm tra token với Facebook
                var urlConnect = $"https://graph.facebook.com/v21.0/me?fields=id,name,email,phone&access_token={accessToken}";
                var userAvatar = $"https://graph.facebook.com/v21.0/me/picture?type=large&access_token={accessToken}"; // Dùng link này là xem luôn dc avatar 
                var response = await _httpClient.GetAsync(urlConnect);
                var response2 = await _httpClient.GetAsync(userAvatar);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, $"Invalid Facebook access token. Details: {errorContent}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var userData = JsonConvert.DeserializeObject<JObject>(content);

                // Kiểm tra sự tồn tại của các trường trước khi truy cập
                var name = userData?["name"]?.ToString();
                var email = userData?["email"]?.ToString();

                var account = await findUserByEmail(email);
                if (account == null)
                {
                    account = new User
                    {
                        FullName = name ?? "User",
                        Email = email,
                        Password = HashPassword("12345"),
                        Avatar = userAvatar,
                        Status = "ACTIVE",
                        RoleId = (int)RoleEnum.Customer
                    };
                    await _unitOfWork.UserRepository.AddAsync(account);
                    await _unitOfWork.SaveChangesAsync();

                    //get again
                    account = await findUserByEmail(email);
                    var res = new LoginResponse
                    {
                        Role = account.Role.RoleName,
                        Token = _tokenHandler.GenerateJwtToken(account).Result
                    };
                    return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, res);
                }
                else if (account.Status == "INACTIVE")
                {
                    throw new Exception("Account is deleted, please contact admin to restore");
                }

                return null;
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST,ex.Message);
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
                                                   x.RoleId != 1
            );

            if (users == null || !users.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
            }

            var response = users.Adapt<List<UserResponse>>();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }
        public async Task<IBusinessResult> GetUserById(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if(user == null || user.RoleId == 1)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
            }
            var response = user.Adapt<UserResponse>();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }
    }
}
