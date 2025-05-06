using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NutriDiet.Common;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Common.Enums;
using NutriDiet.Repository.Interface;
using NutriDiet.Service.Enums;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Response;
using System.Linq;

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
                .GetByWhere(x => x.Feedback != null)
                .CountAsync();

            var totalFeedbackMealLog = await _unitOfWork.AIRecommendationMeallogRepository
                .GetByWhere(x => x.Feedback != null)
                .CountAsync();

            var dashboard = new DashboardResponse
            {
                TotalUser = await _unitOfWork.UserRepository
                    .GetByWhere(x=>x.RoleId != (int)RoleEnum.Admin)
                    .CountAsync(),

                TotalPremiumUser = await _unitOfWork.UserRepository
                    .GetByWhere(x => x.UserPackages.Any(x => x.ExpiryDate >= DateTime.UtcNow && x.Status == "Active") && x.RoleId!= (int)RoleEnum.Admin)
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
            var userPackages = await _unitOfWork.UserPackageRepository
                .GetByWhere(x => x.StartDate != null && x.Package != null)
                .Include(x => x.Package)
                .ToListAsync();

            var dailyRevenue = userPackages
                .GroupBy(x => x.StartDate!.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    PackageSold = g.Count(),
                    TotalRevenue = g.Sum(x => x.Package!.Price)
                })
                .OrderBy(x => x.Date)
                .ToList();

            
            var weeklyRevenue = userPackages
                .GroupBy(x => new
                {
                    Year = x.StartDate!.Value.Year,
                    Month = x.StartDate!.Value.Month,
                    Week = ((x.StartDate!.Value.Day - 1) / 7) + 1 // Tính tuần trong tháng
                })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Week = g.Key.Week,
                    PackageSold = g.Count(),
                    TotalRevenue = g.Sum(x => x.Package!.Price)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ThenBy(x => x.Week)
                .ToList();

            var monthlyRevenue = userPackages
                .GroupBy(x => new { Year = x.StartDate!.Value.Year, Month = x.StartDate!.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    PackageSold = g.Count(),
                    TotalRevenue = g.Sum(x => x.Package!.Price)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            var annualRevenue = userPackages
                .GroupBy(x => x.StartDate!.Value.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    PackageSold = g.Count(),
                    TotalRevenue = g.Sum(x => x.Package!.Price)
                })
                .OrderBy(x => x.Year)
                .ToList();

            var totalRevenue = userPackages
                .Where(x => x.StartDate != null)
                .ToList();

            var totalRevenueAmount = totalRevenue.Sum(x => x.Package!.Price);

            var totalPackageSold = totalRevenue.Count;

            var response = new RevenueResponse
            {
                Revenue = new
                {
                    Daily = dailyRevenue,
                    Weekly = weeklyRevenue,
                    Monthly = monthlyRevenue,
                    Annual = annualRevenue,
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
                Price = x.PriceAtPurchase,
                PaidAt = x.StartDate,
                ExpiryDate = x.ExpiryDate
            });

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }
        public async Task<GoalChartResponse> GetGoalProgressChartData()
        {
            // 1. Lấy tất cả personal goals chỉ của 2 loại GoalType.GainWeight & GoalType.LoseWeight
            var goals = await _unitOfWork.PersonalGoalRepository
                .GetByWhere(pg =>
                    pg.GoalType == GoalType.GainWeight.ToString() ||
                    pg.GoalType == GoalType.LoseWeight.ToString())
                .AsNoTracking()
                .ToListAsync();

            // 2. Nhóm theo GoalType và tính tổng + đạt
            var grouped = goals
                .GroupBy(pg => pg.GoalType)
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        Total = g.Count(),
                        Achieved = g.Count(pg => pg.ProgressPercentage >= 100)
                    }
                );

            // 3. Chuẩn bị mảng kết quả với thứ tự cố định: [Tăng cân, Giảm cân]
            var labels = new[] { "Tăng cân", "Giảm cân" };
            var achieved = new int[2];
            var notAchieved = new int[2];
            var progressPercs = new double[2];

            // 4. Điền dữ liệu Tăng cân
            if (grouped.TryGetValue(GoalType.GainWeight.ToString(), out var gain))
            {
                achieved[0] = gain.Achieved;
                notAchieved[0] = gain.Total - gain.Achieved;
                progressPercs[0] = Math.Round(gain.Achieved * 100.0 / gain.Total, 2);
            }

            // 5. Điền dữ liệu Giảm cân
            if (grouped.TryGetValue(GoalType.LoseWeight.ToString(), out var lose))
            {
                achieved[1] = lose.Achieved;
                notAchieved[1] = lose.Total - lose.Achieved;
                progressPercs[1] = Math.Round(lose.Achieved * 100.0 / lose.Total, 2);
            }

            // 6. Trả về response
            return new GoalChartResponse
            {
                Labels = labels,
                Achieved = achieved,
                NotAchieved = notAchieved,
                ProgressPercentages = progressPercs
            };
        }
        public async Task<IBusinessResult> GetTopSelectedFoods(int top = 10)
        {
            var mealLogCounts = await _unitOfWork.MealLogRepository
                .GetByWhere(m => m.MealLogDetails.Any())
                .Include(m => m.MealLogDetails)
                .AsNoTracking()
                .SelectMany(m => m.MealLogDetails)
                .GroupBy(d => d.FoodId)
                .Select(g => new { FoodId = g.Key, Count = g.Count() })
                .ToListAsync();

            var mealPlanCounts = await _unitOfWork.MealPlanDetailRepository
                .GetAll()
                .AsNoTracking()
                .GroupBy(d => d.FoodId)
                .Select(g => new { FoodId = g.Key, Count = g.Count() })
                .ToListAsync();

            var allFoodIds = mealLogCounts
                .Select(x => x.FoodId)
                .Concat(mealPlanCounts.Select(x => x.FoodId))
                .Distinct()
                .ToList();

            var foods = await _unitOfWork.FoodRepository
                .GetByWhere(f => allFoodIds.Contains(f.FoodId))
                .AsNoTracking()
                .ToListAsync();

            var result = allFoodIds
                    .Select(id =>
                    {
                        var food = foods.FirstOrDefault(f => f.FoodId == id);
                        var ml = mealLogCounts.FirstOrDefault(x => x.FoodId == id)?.Count ?? 0;
                        var mp = mealPlanCounts.FirstOrDefault(x => x.FoodId == id)?.Count ?? 0;

                        if (food == null) return null;

                        return new TopFoodResponse
                        {
                            FoodName = food.FoodName,
                            MealLogCount = ml,
                            MealPlanCount = mp,
                            TotalCount = ml + mp
                        };
                    })
                    .Where(x => x != null) // Bỏ qua các item bị null (do không tìm thấy food)
                    .OrderByDescending(x => x.TotalCount)
                    .Take(top)
                    .ToList();


            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, result);
        }

        public async Task<IBusinessResult> GetActivityLevelDistributionAsync()
        {
            var query = _unitOfWork.HealthProfileRepository
                .GetByWhere(hp => !string.IsNullOrEmpty(hp.ActivityLevel));

            var total = await query.CountAsync();

            var grouped = await query
                .GroupBy(hp => hp.ActivityLevel)
                .Select(g => new { LevelName = g.Key, Count = g.Count() })
                .ToListAsync();

            var distribution = Enum.GetNames(typeof(ActivityLevel))
                .Select(name =>
                {
                    var entry = grouped.FirstOrDefault(g => g.LevelName == name);
                    var cnt = entry?.Count ?? 0;
                    var pct = total > 0
                                  ? Math.Round(cnt * 100.0 / total, 2)
                                  : 0.0;

                    return new ActivityLevelDistribution
                    {
                        ActivityLevel = name,
                        Count = cnt,
                        Percentage = pct
                    };
                })
                .ToList();

            return new BusinessResult(
                Const.HTTP_STATUS_OK,
                "Thống kê phân bố ActivityLevel",
                distribution
            );
        }
        public async Task<IBusinessResult> GetNutritionSummaryGlobalAsync(DateTime date)
        {
            var details = await _unitOfWork.MealLogRepository
                .GetByWhere(m => m.LogDate.HasValue && m.LogDate.Value.Date == date.Date)
                .SelectMany(m => m.MealLogDetails)
                .ToListAsync();

            if (!details.Any())
            {
                var empty = new NutritionGlobalSummaryResponse
                {
                    TotalCalories = 0,
                    TotalCarbs = 0,
                    TotalProtein = 0,
                    TotalFat = 0,
                    CarbsPercentage = 0,
                    ProteinPercentage = 0,
                    FatPercentage = 0
                };
                return new BusinessResult(Const.HTTP_STATUS_OK, "No nutrition data for this date.", empty);
            }

            double totalCarbs = details.Sum(d => d.Carbs ?? 0);
            double totalProtein = details.Sum(d => d.Protein ?? 0);
            double totalFat = details.Sum(d => d.Fat ?? 0);

            double carbCal = totalCarbs * 4;
            double proteinCal = totalProtein * 4;
            double fatCal = totalFat * 9;

            double totalCalories = carbCal + proteinCal + fatCal;

            double carbsPct = Math.Round(carbCal / totalCalories * 100, 2);
            double proteinPct = Math.Round(proteinCal / totalCalories * 100, 2);
            double fatPct = Math.Round(fatCal / totalCalories * 100, 2);

            var resp = new NutritionGlobalSummaryResponse
            {
                TotalCalories = Math.Round(totalCalories, 2),
                TotalCarbs = Math.Round(totalCarbs, 2),
                TotalProtein = Math.Round(totalProtein, 2),
                TotalFat = Math.Round(totalFat, 2),
                CarbsPercentage = carbsPct,
                ProteinPercentage = proteinPct,
                FatPercentage = fatPct
            };

            return new BusinessResult(
                Const.HTTP_STATUS_OK,
                "Global nutrition summary retrieved successfully.",
                resp
            );
        }

        public async Task<IBusinessResult> GetDietStyleDistributionAsync()
        {
            var query = _unitOfWork.HealthProfileRepository
                .GetByWhere(hp => !string.IsNullOrEmpty(hp.DietStyle));

            var total = await query.CountAsync();

            var grouped = await query
                .GroupBy(hp => hp.DietStyle)
                .Select(g => new { StyleName = g.Key, Count = g.Count() })
                .ToListAsync();

            var distribution = Enum.GetNames(typeof(DietStyleEnum))
                .Select(name =>
                {
                    var entry = grouped.FirstOrDefault(g => g.StyleName == name);
                    var cnt = entry?.Count ?? 0;
                    var pct = total > 0
                                  ? Math.Round(cnt * 100.0 / total, 2)
                                  : 0.0;

                    return new DietStyleDistribution
                    {
                        DietStyle = name,
                        Count = cnt,
                        Percentage = pct
                    };
                })
                .ToList();

            return new BusinessResult(
                Const.HTTP_STATUS_OK,
                "Thống kê phân bố DietStyle",
                distribution
            );
        }
    }
}
