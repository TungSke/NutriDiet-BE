using NutriDiet.Common.BusinessResult;
using NutriDiet.Repository.Models;
using NutriDiet.Service.Enums;
using NutriDiet.Service.ModelDTOs.Request;

namespace NutriDiet.Service.Interface
{
    public interface IUserService
    {
        Task<User> findUserById(int id);

        Task<IBusinessResult> Register(RegisterRequest request);

        Task<IBusinessResult> VerifyAccount(VerifyAccountRequest request);

        Task<IBusinessResult> ResendOTP(ResendOtpRequest request);

        Task<IBusinessResult> Login(LoginRequest accountrequest);

        Task<IBusinessResult> LoginWithGoogle(string idToken, string fcmToken);

        Task<IBusinessResult> LoginWithFacebook(string accessToken, string fcmToken);

        Task<IBusinessResult> ForgotPassword(string email);

        Task<IBusinessResult> ResetPassword(ResetPasswordRequest request);
        Task<IBusinessResult> SearchUser(int pageIndex, int pageSize, string status, string search);
        Task<IBusinessResult> GetUserById(int id);

        Task<IBusinessResult> RefreshToken(RefreshTokenRequest request);
        Task<IBusinessResult> UpdateUser(UpdateUserRequest request);
        Task<IBusinessResult> UpgradePackage(int packageId);
        Task<IBusinessResult> IsPremium();
        Task<IBusinessResult> IsAdvancedPremium();
        Task<IBusinessResult> UpdateStatusUser(int userId, UserStatus status);
        Task<IBusinessResult> ChangeRole(int userId, RoleEnum role);
    }
}
