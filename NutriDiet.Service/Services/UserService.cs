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

namespace NutriDiet.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PasswordHasher<string> _passwordHasher;
        private readonly TokenHandlerHelper _tokenHandler;
        private readonly EmailService _emailService;
        private readonly string _UserIdClaim;

        public UserService(IUnitOfWork unitOfWork, EmailService emailService, TokenHandlerHelper tokenHandlerHelper)
        {
            _unitOfWork ??= unitOfWork;
            _passwordHasher = new PasswordHasher<string>();
            _tokenHandler = tokenHandlerHelper;
            _emailService = emailService;
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
            return await _unitOfWork.UserRepository.GetByWhere(x => x.UserId == id).Include(x => x.Role).FirstOrDefaultAsync();
        }

        public async Task<IBusinessResult> Register(RegisterRequest request)
        {

            var checkUser = await findUserByEmail(request.Email);
            if (checkUser != null)
            {
                return new BusinessResult(Const.FAILURE, "Email Existed");
            }
            request.Password = HashPassword(request.Password);
            var acc = request.Adapt<User>();
            acc.FullName = "New User";
            acc.RoleId = (int)RoleEnum.Customer;
            acc.Status = UserStatus.Inactive.ToString();
            acc.Avatar = "";
            await _unitOfWork.UserRepository.CreateAsync(acc);
            await _unitOfWork.SaveChangesAsync();

            await _emailService.SendEmailWithOTP(request.Email, "Verify your account");
            return new BusinessResult(Const.SUCCESS, "Check email to active account");
        }

        public async Task<IBusinessResult> VerifyAccount(VerifyAccountRequest request)
        {
            var isOtpValid = await _emailService.VerifyOtp(request.Email, request.OTP);

            if (!isOtpValid)
            {
                return new BusinessResult(Const.FAILURE, "OTP is incorrect");
            }

            var user = await findUserByEmail(request.Email);
            if (user == null)
            {
                return new BusinessResult(Const.SUCCESS, "User not found");
            }

            user.Status = UserStatus.Active.ToString();
            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.SUCCESS, "Verify success");
        }


        public async Task<IBusinessResult> Login(LoginRequest request)
        {
            var account = await findUserByEmail(request.Email);
            if (account == null)
            {
                return new BusinessResult(Const.FAILURE, "Email not Existed");
            }
            else if (account.Status == UserStatus.Inactive.ToString())
            {
                return new BusinessResult(Const.FAILURE, "Account is not verified");
            }
            else if (account.Status == UserStatus.Deleted.ToString())
            {
                return new BusinessResult(Const.FAILURE, "Account is Deleted");
            }

            var result = _passwordHasher.VerifyHashedPassword(null, account.Password, request.Password);

            if (result == PasswordVerificationResult.Success)
            {
                var res = new LoginResponse
                {
                    Role = account.Role.RoleName,
                    Token = _tokenHandler.GenerateJwtToken(account).Result
                };
                return new BusinessResult(Const.SUCCESS, Const.SUCCESS_READ_MSG, res);
            }
            else
            {
                return new BusinessResult(Const.FAILURE, "Password is incorrect");
            }
        }



    }
}
