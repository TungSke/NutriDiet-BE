using NutriDiet.Common.BusinessResult;
using NutriDiet.Service.ModelDTOs.Request;
using NutriDiet.Service.ModelDTOs.Response;

namespace NutriDiet.Service.Interface
{
    public interface IUserService
    {
        Task<IBusinessResult> Register(RegisterRequest request);

        Task<IBusinessResult> VerifyAccount(VerifyAccountRequest request);

        Task<IBusinessResult> Login(LoginRequest accountrequest);
    }
}
