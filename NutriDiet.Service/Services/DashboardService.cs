using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NutriDiet.Common;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Repository.Interface;
using NutriDiet.Service.Enums;
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

            var totalFeedbackMealPlan = await _unitOfWork.AIRecommendationRepository
                .GetAll()
                .Where(x => x.Feedback != null)
                .CountAsync();
            var totalFeedbackMealLog = await _unitOfWork.AIRecommendationMeallogRepository
                .GetAll()
                .Where(x => x.Feedback != null)
                .CountAsync();

            var dashboard = new DashboardResponse
            {
                TotalUser = await _unitOfWork.UserRepository
                    .GetAll()
                    .Where(x=>x.RoleId != (int)RoleEnum.Admin)
                    .CountAsync(),
                TotalPremiumUser = await _unitOfWork.UserRepository
                    .GetAll()
                    .Where(x => x.UserPackages.Any(x => x.ExpiryDate >= DateTime.UtcNow && x.Status == "Active") && x.RoleId!= (int)RoleEnum.Admin)
                    .CountAsync(),
                TotalPackage = await _unitOfWork.PackageRepository.CountAsync(),
                TotalAllergy = await _unitOfWork.AllergyRepository.CountAsync(),
                TotalDisease = await _unitOfWork.DiseaseRepository.CountAsync(),
                TotalIngredient = await _unitOfWork.IngredientRepository.CountAsync(),
                TotalFood = await _unitOfWork.FoodRepository.CountAsync(),
                TotalMealPlan = await _unitOfWork.MealPlanRepository.CountAsync(),
                TotalFeedbackAI = totalFeedbackMealPlan + totalFeedbackMealLog
            };
            return new BusinessResult(Const.HTTP_STATUS_OK,Const.SUCCESS_READ_MSG,dashboard);
        }

        public async Task<IBusinessResult> Revenue()
        {
            var dailyRevenue = await _unitOfWork.UserPackageRepository
                .GetAll()
                .Where(x => x.StartDate != null)
                .GroupBy(x => x.StartDate.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    PackageSold = g.Count(),
                    TotalRevenue = g.Sum(x => x.Package.Price)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            var weeklyRevenue = await _unitOfWork.UserPackageRepository
                .GetAll()
                .Where(x => x.StartDate != null)
                .GroupBy(x => EF.Functions.DateDiffWeek(DateTime.UtcNow, x.StartDate.Value))
                .Select(g => new
                {
                    Week = g.Key + 1,
                    PackageSold = g.Count(),
                    TotalRevenue = g.Sum(x => x.Package.Price)
                })
                .OrderBy(x => x.Week)
                .ToListAsync();

            var monthlyRevenue = await _unitOfWork.UserPackageRepository
                .GetAll()
                .Where(x => x.StartDate != null)
                .GroupBy(x => x.StartDate.Value.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    PackageSold = g.Count(),
                    TotalRevenue = g.Sum(x => x.Package.Price)
                })
                .OrderBy(x => x.Month)
                .ToListAsync();

            var totalRevenue = await _unitOfWork.UserPackageRepository
                .GetAll()
                .Include(x=>x.Package)
                .Where(x => x.StartDate != null)
                .ToListAsync();

            var totalRevenueAmount = totalRevenue.Sum(x => x.Package.Price);

            var totalPackageSold = totalRevenue.Count;

            var response = new RevenueResponse
            {
                Revenue = new
                {
                    Daily = dailyRevenue,
                    Weekly = weeklyRevenue,
                    Monthly = monthlyRevenue,
                    Total = new
                    {
                        PackageSold = totalPackageSold,
                        TotalRevenue = totalRevenueAmount
                    }
                }
            };

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);

        }

        public async Task<IBusinessResult> Transaction(int pageIndex, int pageSize, string? search)
        {
            search = search?.ToLower() ?? string.Empty;
            var transaction = await _unitOfWork.UserPackageRepository.GetPagedAsync(
                pageIndex,
                pageSize,
                x => (string.IsNullOrEmpty(search) || x.User.Email.ToLower().Contains(search)
                                                   || x.Package.PackageName.ToLower().Contains(search))
                                                   || x.Package.Price.Equals(search),
                q => q.OrderByDescending(x => x.StartDate),
                i => i.Include(x => x.User).Include(x=>x.Package)
                );

            var response = transaction.Select(x => new TransactionResponse
            {
                UserId = x.UserId,
                Email = x.User.Email,
                PackageId = x.PackageId,
                PackageName = x.Package.PackageName,
                Description = x.Package.Description,
                Price = x.Package.Price,
                PaidAt = x.StartDate,
                ExpiryDate = x.ExpiryDate
            });

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }
    }
}
