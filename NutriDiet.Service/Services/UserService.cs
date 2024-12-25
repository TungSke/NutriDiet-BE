using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using NutriDiet.Service.Enums;
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

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = new PasswordHasher<string>();
            _tokenHandler = new TokenHandlerHelper();
            _emailService = new EmailService();
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

        public async Task Register(RegisterRequest request)
        {
            
            var checkUser = await findUserByEmail(request.Email);
            if (checkUser != null)
            {
                throw new Exception("Email already exists");
            }
            request.Password = HashPassword(request.Password);
            var acc = request.Adapt<User>();
            acc.FullName = "New User";
            acc.RoleId = (int)RoleEnum.Customer;
            acc.Status = "INACTIVE";
            acc.Avatar = "";
            await _unitOfWork.UserRepository.CreateAsync(acc);
            await _unitOfWork.SaveChangesAsync();

            await _emailService.SendEmailWithOTP(request.Email, "Verify your account");
        }

        public async Task VerifyAccount(VerifyAccountRequest request)
        {
            
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var account = await findUserByEmail(request.Email);
            if (account == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(null, request.Password, request.Password);
            if(result == PasswordVerificationResult.Success)
            {
                return new LoginResponse
                {
                    Role = account.Role.RoleName,
                    Token = _tokenHandler.GenerateJwtToken(account).Result
                };
            }
            else
            {
                throw new Exception("Password is incorrect");
            }
        }



    }
}
