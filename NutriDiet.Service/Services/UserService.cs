using LogiConnect.Repository.Interface;
using LogiConnect.Repository.Models;
using LogiConnect.Service.Interface;
using LogiConnect.Service.ModelDTOs.Request;
using LogiConnect.Service.ModelDTOs.Response;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace LogiConnect.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PasswordHasher<string> _passwordHasher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _accountIdClaim;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private string GetAccountIdClaim()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        private string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(null, password);
        }

        public async Task<User> findAccountByEmail(string email)
        {
            return await _unitOfWork.UserRepository.GetByWhere(x => x.Email == email).Include(x => x.Role).FirstOrDefaultAsync();
        }

        public async Task<User> findAccountById(int id)
        {
            return await _unitOfWork.UserRepository.GetByWhere(x => x.UserId == id).Include(x => x.Role).FirstOrDefaultAsync();
        }

        public async Task Register(RegisterRequest request)
        {
            var checkAccount = await findAccountByEmail(request.Email);
            if (checkAccount != null)
            {
                throw new Exception("Email already exists");
            }
            request.Password = HashPassword(request.Password);
            //var acc = request.Adapt<User>();
            //acc.RoleId = await _unitOfWork.RoleRepository.GetByWhere(x => x.RoleName.ToLower() == "user").Select(x => x.RoleId).FirstOrDefaultAsync();
            //acc.Status = "INACTIVE";
            //acc.Avatar = "/images/avatar.png";
            //acc.CreateAt = DateTime.UtcNow;
            //await _unitOfWork.AccountRepository.CreateAsync(acc);
            //await _unitOfWork.SaveChangesAsync();


            //var token = await _tokenHandler.GenerateJwtTokenResetOrCreateAcc(acc);
            //string verifiedUrl = account.verifiedUrl.Replace("{token}", Uri.EscapeDataString(token));
            //var verifyLink = $"<a href='{verifiedUrl}?email={account.Email}'>Verify create your account</a>";

            //var plainTextMessage = "We received a request to create account in our service. If you didn't make the request, just ignore this email" +
            //                       "<br />" +
            //                       "Otherwise, you can verify your account using the link below:" +
            //                       "<br />" +
            //                       $"Verify create account link: {verifyLink}";
            //await _googleService.SendEmail(account.Email, "Verify Create Your Account", plainTextMessage);
        }

        public async Task<LoginResponse> Login(LoginRequest accountrequest)
        {
            var account = await findAccountByEmail(accountrequest.Email);
            if (account == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(accountrequest.Email, account.Password, accountrequest.Password);

            return null;
        }



    }
}
