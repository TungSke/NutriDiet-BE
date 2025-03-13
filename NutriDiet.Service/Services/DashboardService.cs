using Microsoft.AspNetCore.Http;
using NutriDiet.Common;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Repository.Interface;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IBusinessResult> Dashboard()
        {
            var dashboard = new DashboardResponse
            {
                TotalUser = await _unitOfWork.UserRepository.CountAsync(),
                MealPlanNumber = await _unitOfWork.MealPlanRepository.CountAsync(),
                //PackageNumber = await _unitOfWork.PackageRepository.CountAsync(),
            };
            return new BusinessResult(Const.HTTP_STATUS_OK,Const.SUCCESS_READ_MSG,dashboard);
        }
    }
}
