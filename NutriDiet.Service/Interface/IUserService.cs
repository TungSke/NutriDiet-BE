using NutriDiet.Service.ModelDTOs.Request;
using NutriDiet.Service.ModelDTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Interface
{
    public interface IUserService
    {
        Task Register(RegisterRequest request);
        Task<LoginResponse> Login(LoginRequest accountrequest);
    }
}
