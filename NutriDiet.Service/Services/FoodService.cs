using Google.Apis.Drive.v3.Data;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NutriDiet.Common;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using NutriDiet.Service.ModelDTOs.Response;
using NutriDiet.Service.Utilities;
using System.Security.Claims;
using OfficeOpenXml;
using System.IO;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Runtime.InteropServices;

namespace NutriDiet.Service.Services
{
    public class FoodService : IFoodService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AIGeneratorService _aIGeneratorService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userIdClaim;

        public FoodService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, AIGeneratorService aIGeneratorService)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _userIdClaim = GetUserIdClaim();
            _aIGeneratorService=aIGeneratorService;
        }

        private string GetUserIdClaim()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public async Task<IBusinessResult> GetAllFood(int pageIndex, int pageSize, string foodType, string search)
        {
            search = search?.ToLower() ?? string.Empty;
            foodType = foodType?.ToLower();

            var foods = await _unitOfWork.FoodRepository.GetPagedAsync(
                pageIndex,
                pageSize,
                x => (string.IsNullOrEmpty(search) || x.FoodName.ToLower().Contains(search)) &&
                     (string.IsNullOrEmpty(foodType) || x.FoodType.ToLower() == foodType),
                null,
                query => query
                    .Include(x => x.ServingSize)
                    .Include(x => x.FoodServingSizes)
                    .ThenInclude(fss => fss.ServingSize)
            );

            if (foods == null || !foods.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
            }

            var response = foods.Adapt<List<FoodResponse>>();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> GetFoodById(int foodId)
        {
            var food = await _unitOfWork.FoodRepository.GetByWhere(x => x.FoodId == foodId).Include(x => x.Ingredients).Include(x => x.ServingSize)
                    .Include(x => x.FoodServingSizes)
                    .ThenInclude(fss => fss.ServingSize).FirstOrDefaultAsync();
            if (food == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Food not found");
            }

            var response = food.Adapt<FoodResponse>();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> CreateFood(FoodRequest request)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(request.FoodName))
                {
                    return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Food name is required");
                }

                // Kiểm tra tên món ăn đã tồn tại
                var existedFood = await _unitOfWork.FoodRepository
                    .GetByWhere(x => x.FoodName.ToLower().Equals(request.FoodName.ToLower()))
                    .AnyAsync();

                if (existedFood)
                {
                    return new BusinessResult(Const.HTTP_STATUS_CONFLICT, "Food name already exists");
                }

                // Kiểm tra kích thước khẩu phần mặc định
                if (request.ServingSizeId.HasValue)
                {
                    var servingSizeExists = await _unitOfWork.ServingSizeRepository
                        .GetByWhere(x => x.ServingSizeId == request.ServingSizeId)
                        .AnyAsync();
                    if (!servingSizeExists)
                    {
                        return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Serving size not found");
                    }
                }

                // Tạo mới món ăn
                var food = request.Adapt<Food>();

                // Xử lý nguyên liệu
                if (request.Ingredients != null && request.Ingredients.Count > 0)
                {
                    var ingredients = await _unitOfWork.IngredientRepository
                        .GetByWhere(x => request.Ingredients.Contains(x.IngredientId))
                        .ToListAsync();

                    if (ingredients.Count != request.Ingredients.Count)
                    {
                        return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "One or more ingredients not found");
                    }

                    food.Ingredients = ingredients;
                }
                else
                {
                    food.Ingredients = new List<Ingredient>();
                }

                // Xử lý FoodServingSizes
                if (request.FoodServingSizes != null && request.FoodServingSizes.Count > 0)
                {
                    var servingSizeIds = request.FoodServingSizes.Select(fss => fss.ServingSizeId).ToList();
                    var servingSizes = await _unitOfWork.ServingSizeRepository
                        .GetByWhere(x => servingSizeIds.Contains(x.ServingSizeId))
                        .ToListAsync();

                    if (servingSizes.Count != request.FoodServingSizes.Count)
                    {
                        return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "One or more serving sizes not found");
                    }

                    food.FoodServingSizes = request.FoodServingSizes.Adapt<List<FoodServingSize>>();
                }
                else
                {
                    food.FoodServingSizes = new List<FoodServingSize>();
                }

                // Lưu vào cơ sở dữ liệu
                await _unitOfWork.FoodRepository.AddAsync(food);
                await _unitOfWork.SaveChangesAsync();

                // Ánh xạ sang FoodResponse để trả về
                var response = food.Adapt<FoodResponse>();

                return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_CREATE_MSG, response);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, $"Error creating food: {ex.Message}");
            }
        }

        public async Task<IBusinessResult> UpdateFood(int foodId, FoodRequest request)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(request.FoodName))
                {
                    return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Food name is required");
                }

                // Tìm món ăn cần cập nhật
                var existedFood = await _unitOfWork.FoodRepository
                    .GetByWhere(x => x.FoodId == foodId)
                    .Include(f => f.Ingredients)
                    .Include(f => f.FoodServingSizes)
                    .FirstOrDefaultAsync();

                if (existedFood == null)
                {
                    return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Food not found");
                }

                // Kiểm tra tên món ăn đã tồn tại (trừ chính món ăn đang cập nhật)
                var nameExists = await _unitOfWork.FoodRepository
                    .GetByWhere(x => x.FoodName.ToLower().Equals(request.FoodName.ToLower()) && x.FoodId != foodId)
                    .AnyAsync();

                if (nameExists)
                {
                    return new BusinessResult(Const.HTTP_STATUS_CONFLICT, "Food name already exists");
                }

                // Kiểm tra kích thước khẩu phần mặc định
                if (request.ServingSizeId.HasValue)
                {
                    var servingSizeExists = await _unitOfWork.ServingSizeRepository
                        .GetByWhere(x => x.ServingSizeId == request.ServingSizeId)
                        .AnyAsync();
                    if (!servingSizeExists)
                    {
                        return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Serving size not found");
                    }
                }

                // Ánh xạ thông tin cơ bản từ request sang existedFood
                request.Adapt(existedFood);

                // Xử lý nguyên liệu
                if (request.Ingredients != null)
                {
                    existedFood.Ingredients.Clear();

                    var ingredients = await _unitOfWork.IngredientRepository
                        .GetByWhere(x => request.Ingredients.Contains(x.IngredientId))
                        .ToListAsync();

                    if (ingredients.Count != request.Ingredients.Count)
                    {
                        return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "One or more ingredients not found");
                    }

                    existedFood.Ingredients = ingredients;
                }
                else
                {
                    existedFood.Ingredients.Clear(); // Xóa nguyên liệu nếu request không gửi Ingredients
                }

                // Xử lý FoodServingSizes
                if (request.FoodServingSizes != null && request.FoodServingSizes.Count > 0)
                {
                    var servingSizeIds = request.FoodServingSizes.Select(fss => fss.ServingSizeId).ToList();
                    var servingSizes = await _unitOfWork.ServingSizeRepository
                        .GetByWhere(x => servingSizeIds.Contains(x.ServingSizeId))
                        .ToListAsync();

                    if (servingSizes.Count != request.FoodServingSizes.Count)
                    {
                        return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "One or more serving sizes not found");
                    }

                    existedFood.FoodServingSizes.Clear();
                    existedFood.FoodServingSizes = request.FoodServingSizes.Adapt<List<FoodServingSize>>();
                }
                else
                {
                    existedFood.FoodServingSizes.Clear(); // Xóa FoodServingSizes nếu request không gửi dữ liệu
                }

                // Cập nhật vào cơ sở dữ liệu
                await _unitOfWork.FoodRepository.UpdateAsync(existedFood);
                await _unitOfWork.SaveChangesAsync();

                // Ánh xạ sang FoodResponse để trả về
                var response = existedFood.Adapt<FoodResponse>();

                return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_UPDATE_MSG, response);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, $"Error updating food: {ex.Message}");
            }
        }

        public async Task<IBusinessResult> DeleteFood(int foodId)
        {
            var food = await _unitOfWork.FoodRepository.GetByIdAsync(foodId);
            if (food == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Food not found");
            }

            await _unitOfWork.FoodRepository.DeleteAsync(food);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_DELETE_MSG);
        }

        public async Task<IBusinessResult> CreateFoodRecipeByAI(int foodId, int cuisineId)
        {
            //lấy thông tin tình trạng sức khỏe của user
            int userid = int.Parse(_userIdClaim);

            var isPremium = await _unitOfWork.UserPackageRepository.IsUserPremiumAsync(userid);
            if (!isPremium)
            {
                return new BusinessResult(Const.HTTP_STATUS_FORBIDDEN, "Chỉ tài khoản Premium mới sử dụng được tính năng này");
            }

            var userInfo = await _unitOfWork.UserRepository
                            .GetByWhere(x => x.UserId == userid)
                            .Include(x => x.Allergies).Include(x => x.Diseases)
                            .Include(x => x.GeneralHealthProfiles)
                            .Include(x => x.UserIngreDientPreferences).ThenInclude(x => x.Ingredient)
                            .FirstOrDefaultAsync();

            if (userInfo == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "User not found");
            }
            //thông tin cơ bản của người dùng
            var allergyNames = userInfo.Allergies?.Select(a => a.AllergyName).ToList() ?? new List<string>();
            var diseaseNames = userInfo.Diseases?.Select(d => d.DiseaseName).ToList() ?? new List<string>();

            var formattedAllergies = allergyNames.Any() ? string.Join(", ", allergyNames) : "không có";
            var formattedDiseases = diseaseNames.Any() ? string.Join(", ", diseaseNames) : "không có";

            var dietStyle = userInfo.GeneralHealthProfiles.OrderByDescending(x => x.CreatedAt).FirstOrDefault().DietStyle;

            var food = await _unitOfWork.FoodRepository.GetByIdAsync(foodId);
            var cuisineType = await _unitOfWork.CuisineRepository.GetByIdAsync(cuisineId);


            var userIngredientsReference = userInfo.UserIngreDientPreferences.Select(x => new
            {
                x.Ingredient.IngredientName,
                x.Level,
            }).ToList();

            string favoriteIngredientsFormatted = userIngredientsReference.Any() ? string.Join(", ", userIngredientsReference.Select(x => $"{x.IngredientName} ({x.Level})")) : "không có";

            if (food == null || cuisineType == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, $"{(food == null ? "food" : "cuisine")} not found");
            }

            // kiểm tra xem đã có công thức nấu món ăn này chưa
            var existingRecipe = await _unitOfWork.RecipeSuggestionRepository
                .GetByWhere(x => x.UserId == userid && x.FoodId == foodId)
                .FirstOrDefaultAsync();

            string rejectionReason = existingRecipe?.RejectionReason ?? "";

            string input = @$"Tôi là người ăn theo phong cách {dietStyle}
Tôi có các bệnh sau: {formattedDiseases}.
Tôi bị dị ứng với các thành phần sau: {formattedAllergies}.
Mức độ yêu thích là -1(ghét) 0(bình thường) 1(thích)
Mức độ yêu thích các nguyên liệu như sau: {favoriteIngredientsFormatted}.
Hãy gợi ý cho tôi một công thức để nấu món {food.FoodName}, theo phong cách {cuisineType.CuisineName}.";

            if (!string.IsNullOrEmpty(rejectionReason))
            {
                input += $"\nLưu ý: Trước đó tôi đã không thích một công thức vì lý do: '{rejectionReason}'. Hãy điều chỉnh lại công thức sao cho phù hợp.";
            }

            input += "\nTrả lời ví dụ: Công thức cho người dị ứng thịt bò và ghét hành, ớt... của bạn là... ";


            var airesponse = await _aIGeneratorService.AIResponseText(input);

            if (existingRecipe == null)
            {
                var recipeSuggestion = new RecipeSuggestion
                {
                    UserId = userid,
                    CuisineId = cuisineId,
                    FoodId = foodId,
                    Airequest = input,
                    Airesponse = airesponse,
                    Aimodel = "Gemini AI",
                    RejectionReason = null
                };

                await _unitOfWork.RecipeSuggestionRepository.AddAsync(recipeSuggestion);
            }
            else
            {
                existingRecipe.Airesponse = airesponse;
                existingRecipe.RejectionReason = null;
                await _unitOfWork.RecipeSuggestionRepository.UpdateAsync(existingRecipe);
            }

            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, airesponse);
        }

        public async Task<IBusinessResult> RejectRecipe(RejectRecipeRequest request)
        {
            int userid = int.Parse(_userIdClaim);

            var isPremium = await _unitOfWork.UserPackageRepository.IsUserPremiumAsync(userid);
            if (!isPremium)
            {
                return new BusinessResult(Const.HTTP_STATUS_FORBIDDEN, "Chỉ tài khoản Premium mới sử dụng được tính năng này");
            }

            var recipe = await _unitOfWork.RecipeSuggestionRepository.GetByWhere(x => x.FoodId == request.FoodId && x.UserId == userid).FirstOrDefaultAsync();
            if (recipe == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Recipe not found");
            }

            recipe.RejectionReason = request.RejectionReason;
            await _unitOfWork.RecipeSuggestionRepository.UpdateAsync(recipe);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG);
        }


        public async Task<IBusinessResult> GetFoodRecipe(int foodId)
        {
            int userid = int.Parse(_userIdClaim);
            var foodRecipe = await _unitOfWork.RecipeSuggestionRepository.GetByWhere(x => x.UserId == userid && x.FoodId == foodId).FirstOrDefaultAsync();
            if (foodRecipe == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Recipe not found");
            }

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, foodRecipe);
        }

        public async Task<IBusinessResult> AddFavoriteFood(int foodId)
        {
            int userId = int.Parse(_userIdClaim);

            var food = await _unitOfWork.FoodRepository.GetByIdAsync(foodId);
            if (food == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Food not found");
            }

            var existingPreference = await _unitOfWork.UserFoodPreferenceRepository
                .GetByWhere(x => x.UserId == userId && x.FoodId == foodId)
                .FirstOrDefaultAsync();

            if (existingPreference != null)
            {
                return new BusinessResult(Const.HTTP_STATUS_CONFLICT, "Food is already in your favorite list");
            }

            var userFoodPreference = new UserFoodPreference
            {
                UserId = userId,
                FoodId = foodId,
                Preference = "Favorite"
            };

            await _unitOfWork.UserFoodPreferenceRepository.AddAsync(userFoodPreference);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_CREATE_MSG);
        }

        public async Task<IBusinessResult> RemoveFavoriteFood(int foodId)
        {
            int userId = int.Parse(_userIdClaim);

            var preference = await _unitOfWork.UserFoodPreferenceRepository
                .GetByWhere(x => x.UserId == userId && x.FoodId == foodId)
                .FirstOrDefaultAsync();

            if (preference == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Food not found in your favorite list");
            }

            await _unitOfWork.UserFoodPreferenceRepository.DeleteAsync(preference);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_DELETE_MSG);
        }

        public async Task<IBusinessResult> GetFavoriteFoods(int pageIndex, int pageSize)
        {
            int userid = int.Parse(_userIdClaim);

            var preference = await _unitOfWork.UserFoodPreferenceRepository.GetPagedAsync(
                pageIndex,
                pageSize,
                x => x.UserId == userid,
                null,
                i => i.Include(x => x.Food).Include(x => x.User));

            if (preference == null || !preference.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
            }

            var response = preference.Select(p => new UserFoodPreferenceResponse
            {
                UserFoodPreferenceId = p.UserFoodPreferenceId,
                UserId = p.UserId,
                FullName = p.User.FullName,
                FoodId = p.FoodId,
                FoodName = p.Food.FoodName
            }).ToList();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> CheckFoodAvoidance(int foodId)
        {
            int userId = int.Parse(_userIdClaim);
            var user = await _unitOfWork.UserRepository
                .GetByWhere(u => u.UserId == userId)
                .Include(u => u.Allergies)
                    .ThenInclude(a => a.Ingredients)
                .Include(u => u.Diseases)
                    .ThenInclude(d => d.Ingredients)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "User not found");
            }

            var food = await _unitOfWork.FoodRepository
                .GetByWhere(f => f.FoodId == foodId)
                .Include(f => f.Ingredients)
                .FirstOrDefaultAsync();

            if (food == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Food not found");
            }

            var avoidReasons = new List<string>();

            if (user.Allergies != null)
            {
                foreach (var allergy in user.Allergies)
                {
                    if (allergy.Ingredients != null && allergy.Ingredients.Any())
                    {
                        var matchedIngredients = food.Ingredients
                            .Where(i => allergy.Ingredients.Any(ai => ai.IngredientId == i.IngredientId))
                            .Select(i => i.IngredientName)
                            .ToList();

                        if (matchedIngredients.Any())
                        {
                            avoidReasons.Add($"({allergy.AllergyName}) với nguyên liệu: {string.Join(", ", matchedIngredients)}");
                        }
                    }
                }
            }

            if (user.Diseases != null)
            {
                foreach (var disease in user.Diseases)
                {
                    if (disease.Ingredients != null && disease.Ingredients.Any())
                    {
                        var matchedIngredients = food.Ingredients
                            .Where(i => disease.Ingredients.Any(di => di.IngredientId == i.IngredientId))
                            .Select(i => i.IngredientName)
                            .ToList();

                        if (matchedIngredients.Any())
                        {
                            avoidReasons.Add($"({disease.DiseaseName}) với nguyên liệu: {string.Join(", ", matchedIngredients)}");
                        }
                    }
                }
            }
            if (avoidReasons.Any())
            {
                string message = $"Đây là món ăn cần tránh: Do {string.Join(" và ", avoidReasons)}.";
                return new BusinessResult(Const.HTTP_STATUS_OK, message);
            }
            else
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Không cần tránh");
            }
        }

        public async Task<IBusinessResult> GetAvoidFoods()
        {
            int userId = int.Parse(_userIdClaim);
            var user = await _unitOfWork.UserRepository
                .GetByWhere(u => u.UserId == userId)
                .Include(u => u.Allergies)
                    .ThenInclude(a => a.Ingredients)
                .Include(u => u.Diseases)
                    .ThenInclude(d => d.Ingredients)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "User not found");
            }
            var avoidIngredientIds = new HashSet<int>();

            if (user.Allergies != null)
            {
                foreach (var allergy in user.Allergies)
                {
                    if (allergy.Ingredients != null)
                    {
                        foreach (var ingredient in allergy.Ingredients)
                        {
                            avoidIngredientIds.Add(ingredient.IngredientId);
                        }
                    }
                }
            }

            if (user.Diseases != null)
            {
                foreach (var disease in user.Diseases)
                {
                    if (disease.Ingredients != null)
                    {
                        foreach (var ingredient in disease.Ingredients)
                        {
                            avoidIngredientIds.Add(ingredient.IngredientId);
                        }
                    }
                }
            }
            if (!avoidIngredientIds.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_OK, "User không có dị ứng hoặc bệnh cần tránh nguyên liệu nào", new List<FoodResponse>());
            }
            var foodsToAvoid = await _unitOfWork.FoodRepository
                .GetByWhere(f => f.Ingredients.Any(i => avoidIngredientIds.Contains(i.IngredientId)))
                .Include(f => f.Ingredients)
                .ToListAsync();

            if (foodsToAvoid == null || !foodsToAvoid.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_OK, "Không tìm thấy món ăn nào cần tránh", new List<FoodResponse>());
            }
            var response = foodsToAvoid.Adapt<List<FoodResponse>>();

            return new BusinessResult(Const.HTTP_STATUS_OK, "Danh sách món ăn cần tránh", response);
        }

        public async Task<IBusinessResult> AnalyzeFoodImport(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length <= 0)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "File không hợp lệ");
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var newFoods = new List<Food>();
            var duplicateFoods = new List<Food>();
            var processedFoodNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name.Equals("Food", StringComparison.OrdinalIgnoreCase));
                        if (worksheet == null)
                        {
                            return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Không tìm thấy worksheet với tên 'Food'");
                        }
                        var rowCount = worksheet.Dimension.Rows;

                        // Lấy danh sách các ServingSize hiện có để ánh xạ UnitName với ServingSizeId
                        var existingServingSizes = await _unitOfWork.ServingSizeRepository
                            .GetAll()
                            .ToDictionaryAsync(ss => ss.UnitName.ToLower().Trim(), ss => ss.ServingSizeId);

                        var existingFoodNames = await _unitOfWork.FoodRepository
                            .GetAll()
                            .Select(f => f.FoodName.ToLower().Trim())
                            .ToListAsync();

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var foodName = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                            var mealType = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                            var foodType = worksheet.Cells[row, 3].Value?.ToString()?.Trim();

                            if (string.IsNullOrEmpty(foodName) || string.IsNullOrEmpty(foodType))
                            {
                                continue; // Bỏ qua nếu thiếu dữ liệu bắt buộc
                            }

                            foodName = foodName.Normalize(NormalizationForm.FormC);
                            var foodNameLower = foodName.ToLower().Trim();

                            if (processedFoodNames.Contains(foodNameLower))
                            {
                                duplicateFoods.Add(new Food { FoodName = foodName });
                                continue;
                            }

                            // Xử lý cột Khẩu phần (ServingSize)
                            var servingSizeValue = worksheet.Cells[row, 4].Value?.ToString()?.Trim();
                            int? servingSizeId = null;
                            double quantity = 1.0;

                            if (!string.IsNullOrEmpty(servingSizeValue))
                            {
                                var parts = servingSizeValue.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length >= 2 && double.TryParse(parts[0], out var parsedQuantity))
                                {
                                    quantity = parsedQuantity;
                                    var unitName = parts[1].ToLower().Trim();
                                    if (existingServingSizes.TryGetValue(unitName, out var id))
                                    {
                                        servingSizeId = id;
                                    }
                                    else
                                    {
                                        continue; // Bỏ qua nếu đơn vị không tồn tại
                                    }
                                }
                                else if (parts.Length == 1)
                                {
                                    var unitName = parts[0].ToLower().Trim();
                                    if (existingServingSizes.TryGetValue(unitName, out var id))
                                    {
                                        servingSizeId = id;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }

                            // Tạo đối tượng Food
                            var food = new Food
                            {
                                FoodName = foodName,
                                MealType = mealType,
                                FoodType = foodType,
                                ServingSizeId = servingSizeId,
                                Description = worksheet.Cells[row, 11].Value?.ToString()?.Trim(),
                                FoodServingSizes = new List<FoodServingSize>()
                            };

                            // Thêm thông tin dinh dưỡng vào FoodServingSizes
                            if (servingSizeId.HasValue)
                            {
                                var foodServingSize = new FoodServingSize
                                {
                                    ServingSizeId = servingSizeId.Value,
                                    Quantity = quantity,
                                    Calories = double.TryParse(worksheet.Cells[row, 5].Value?.ToString()?.Trim(), out var cal) ? cal : 0,
                                    Protein = double.TryParse(worksheet.Cells[row, 6].Value?.ToString()?.Trim(), out var prot) ? prot : 0,
                                    Carbs = double.TryParse(worksheet.Cells[row, 7].Value?.ToString()?.Trim(), out var carb) ? carb : 0,
                                    Fat = double.TryParse(worksheet.Cells[row, 8].Value?.ToString()?.Trim(), out var fat) ? fat : 0,
                                    Glucid = double.TryParse(worksheet.Cells[row, 9].Value?.ToString()?.Trim(), out var gluc) ? gluc : 0,
                                    Fiber = double.TryParse(worksheet.Cells[row, 10].Value?.ToString()?.Trim(), out var fib) ? fib : 0
                                };
                                food.FoodServingSizes.Add(foodServingSize);
                            }

                            processedFoodNames.Add(foodNameLower);

                            if (existingFoodNames.Contains(foodNameLower))
                            {
                                duplicateFoods.Add(food);
                            }
                            else
                            {
                                newFoods.Add(food);
                            }
                        }
                    }
                }

                var result = new
                {
                    NewFoodCount = newFoods.Count,
                    DuplicateFoodCount = duplicateFoods.Count,
                    DuplicateFoodNames = duplicateFoods.Select(x => x.FoodName).ToList(),
                    NewFoods = newFoods // Thêm để sử dụng trong các hàm khác
                };

                return new BusinessResult(Const.HTTP_STATUS_OK, "Phân tích thành công", result);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, $"Lỗi khi phân tích: {ex.Message}");
            }
        }

        public async Task<IBusinessResult> ImportFoodFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length <= 0)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "File không hợp lệ");
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var foodsToImport = new List<Food>();
            var foodServingSizesToImport = new List<FoodServingSize>();
            int duplicateCount = 0;
            var processedFoodNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name.Equals("Food", StringComparison.OrdinalIgnoreCase));
                        if (worksheet == null)
                        {
                            return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Không tìm thấy worksheet với tên 'Food'");
                        }
                        var rowCount = worksheet.Dimension.Rows;

                        var existingServingSizes = await _unitOfWork.ServingSizeRepository
                            .GetAll()
                            .ToDictionaryAsync(ss => ss.UnitName.ToLower().Trim(), ss => ss.ServingSizeId);

                        var existingFoodNames = await _unitOfWork.FoodRepository
                            .GetAll()
                            .Select(f => f.FoodName.ToLower().Trim())
                            .ToListAsync();

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var foodName = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                            var foodType = worksheet.Cells[row, 3].Value?.ToString()?.Trim();

                            if (string.IsNullOrEmpty(foodName) || string.IsNullOrEmpty(foodType))
                            {
                                continue;
                            }

                            foodName = foodName.Normalize(NormalizationForm.FormC);
                            var foodNameLower = foodName.ToLower().Trim();

                            if (processedFoodNames.Contains(foodNameLower) || existingFoodNames.Contains(foodNameLower))
                            {
                                duplicateCount++;
                                continue;
                            }

                            // Xử lý cột Khẩu phần
                            var servingSizeValue = worksheet.Cells[row, 4].Value?.ToString()?.Trim();
                            int? defaultServingSizeId = null;
                            double defaultQuantity = 1.0;

                            if (!string.IsNullOrEmpty(servingSizeValue))
                            {
                                var parts = servingSizeValue.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length >= 2 && double.TryParse(parts[0], out var quantity))
                                {
                                    defaultQuantity = quantity;
                                    var unitName = parts[1].ToLower().Trim();
                                    if (existingServingSizes.TryGetValue(unitName, out var servingSizeId))
                                    {
                                        defaultServingSizeId = servingSizeId;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else if (parts.Length == 1)
                                {
                                    var unitName = parts[0].ToLower().Trim();
                                    if (existingServingSizes.TryGetValue(unitName, out var servingSizeId))
                                    {
                                        defaultServingSizeId = servingSizeId;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }

                            // Tạo đối tượng Food
                            var food = new Food
                            {
                                FoodName = foodName,
                                MealType = worksheet.Cells[row, 2].Value?.ToString()?.Trim(),
                                FoodType = foodType,
                                ServingSizeId = defaultServingSizeId,
                                Description = worksheet.Cells[row, 11].Value?.ToString()?.Trim(),
                                FoodServingSizes = new List<FoodServingSize>()
                            };

                            // Thêm thông tin dinh dưỡng vào FoodServingSize
                            if (defaultServingSizeId.HasValue)
                            {
                                var foodServingSize = new FoodServingSize
                                {
                                    ServingSizeId = defaultServingSizeId.Value,
                                    Quantity = defaultQuantity,
                                    Calories = double.TryParse(worksheet.Cells[row, 5].Value?.ToString()?.Trim(), out var cal) ? cal : 0,
                                    Protein = double.TryParse(worksheet.Cells[row, 6].Value?.ToString()?.Trim(), out var prot) ? prot : 0,
                                    Carbs = double.TryParse(worksheet.Cells[row, 7].Value?.ToString()?.Trim(), out var carb) ? carb : 0,
                                    Fat = double.TryParse(worksheet.Cells[row, 8].Value?.ToString()?.Trim(), out var fat) ? fat : 0,
                                    Glucid = double.TryParse(worksheet.Cells[row, 9].Value?.ToString()?.Trim(), out var gluc) ? gluc : 0,
                                    Fiber = double.TryParse(worksheet.Cells[row, 10].Value?.ToString()?.Trim(), out var fib) ? fib : 0
                                };
                                food.FoodServingSizes.Add(foodServingSize);
                                foodServingSizesToImport.Add(foodServingSize);
                            }

                            processedFoodNames.Add(foodNameLower);
                            foodsToImport.Add(food);
                        }
                    }
                }

                if (foodsToImport.Any())
                {
                    // Lưu từng Food riêng lẻ để tránh concurrency
                    foreach (var food in foodsToImport)
                    {
                        try
                        {
                            await _unitOfWork.FoodRepository.AddAsync(food);
                            await _unitOfWork.SaveChangesAsync();
                        }
                        catch (DbUpdateException dbEx)
                        {
                            duplicateCount++;
                            continue;
                        }
                    }

                    foreach (var foodServingSize in foodServingSizesToImport)
                    {
                        var food = foodsToImport.FirstOrDefault(f => f.FoodName.Equals(foodServingSize.Food?.FoodName, StringComparison.OrdinalIgnoreCase));
                        if (food != null && food.FoodId > 0)
                        {
                            foodServingSize.FoodId = food.FoodId;
                        }
                    }

                    if (foodServingSizesToImport.Any())
                    {
                        await _unitOfWork.FoodServingSizeRepository.AddRangeAsync(foodServingSizesToImport.Where(fss => fss.FoodId > 0));
                        await _unitOfWork.SaveChangesAsync();
                    }
                }

                string message = $"Import thành công {foodsToImport.Count - duplicateCount} món ăn mới.";
                if (duplicateCount > 0)
                {
                    message += $" {duplicateCount} món bị bỏ qua do trùng lặp.";
                }
                if (!foodsToImport.Any() && duplicateCount == 0)
                {
                    message = "Không có món ăn mới nào được import.";
                }

                return new BusinessResult(Const.HTTP_STATUS_OK, message.Trim());
            }
            catch (DbUpdateException dbEx)
            {
                var sqlEx = dbEx.InnerException as SqlException;
                if (sqlEx != null && sqlEx.Number == 2627)
                {
                    return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Dữ liệu bị trùng lặp trong database.");
                }
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, $"Lỗi khi import: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, $"Lỗi khi import: {ex.Message}");
            }
        }

        public async Task<IBusinessResult> ImportAndUpdateFoodFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length <= 0)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "File không hợp lệ");
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var addedFoods = new List<Food>();
            var updatedFoods = new List<Food>();
            var servingSizesToInsert = new List<FoodServingSize>();
            var servingSizesToDelete = new List<FoodServingSize>();
            var processedFoodNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            int duplicateCount = 0;

            try
            {
                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name.Equals("Food", StringComparison.OrdinalIgnoreCase));
                        if (worksheet == null)
                        {
                            return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Không tìm thấy worksheet với tên 'Food'");
                        }

                        var rowCount = worksheet.Dimension.Rows;

                        var existingServingSizes = await _unitOfWork.ServingSizeRepository
                            .GetAll()
                            .AsNoTracking()
                            .ToDictionaryAsync(ss => ss.UnitName.ToLower().Trim(), ss => ss.ServingSizeId);

                        var existingFoods = await _unitOfWork.FoodRepository
                            .GetAll()
                            .Include(f => f.FoodServingSizes)
                            .ToDictionaryAsync(f => f.FoodName.ToLower().Trim(), f => f);

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var foodName = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                            var mealType = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                            var foodType = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
                            var servingText = worksheet.Cells[row, 4].Value?.ToString()?.Trim();
                            var description = worksheet.Cells[row, 11].Value?.ToString()?.Trim();

                            if (string.IsNullOrEmpty(foodName) || string.IsNullOrEmpty(foodType)) continue;

                            var foodNameKey = foodName.Normalize(NormalizationForm.FormC).ToLower().Trim();
                            if (processedFoodNames.Contains(foodNameKey))
                            {
                                duplicateCount++;
                                continue;
                            }

                            processedFoodNames.Add(foodNameKey);

                            // Parse serving info
                            int? servingSizeId = null;
                            double quantity = 1;
                            if (!string.IsNullOrEmpty(servingText))
                            {
                                var parts = servingText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length >= 2 && double.TryParse(parts[0], out var parsedQty))
                                {
                                    quantity = parsedQty;
                                    var unitName = parts[1].ToLower().Trim();
                                    if (!existingServingSizes.TryGetValue(unitName, out var ssid)) continue;
                                    servingSizeId = ssid;
                                }
                                else if (parts.Length == 1)
                                {
                                    var unitName = parts[0].ToLower().Trim();
                                    if (!existingServingSizes.TryGetValue(unitName, out var ssid)) continue;
                                    servingSizeId = ssid;
                                }
                                else continue;
                            }

                            var fss = new FoodServingSize
                            {
                                ServingSizeId = servingSizeId ?? 0,
                                Quantity = quantity,
                                Calories = double.TryParse(worksheet.Cells[row, 5].Value?.ToString()?.Trim(), out var cal) ? cal : 0,
                                Protein = double.TryParse(worksheet.Cells[row, 6].Value?.ToString()?.Trim(), out var prot) ? prot : 0,
                                Carbs = double.TryParse(worksheet.Cells[row, 7].Value?.ToString()?.Trim(), out var carb) ? carb : 0,
                                Fat = double.TryParse(worksheet.Cells[row, 8].Value?.ToString()?.Trim(), out var fat) ? fat : 0,
                                Glucid = double.TryParse(worksheet.Cells[row, 9].Value?.ToString()?.Trim(), out var gluc) ? gluc : 0,
                                Fiber = double.TryParse(worksheet.Cells[row, 10].Value?.ToString()?.Trim(), out var fib) ? fib : 0
                            };

                            if (existingFoods.TryGetValue(foodNameKey, out var existingFood))
                            {
                                // Cập nhật
                                existingFood.MealType = mealType;
                                existingFood.FoodType = foodType;
                                existingFood.ServingSizeId = servingSizeId;
                                existingFood.Description = description;

                                updatedFoods.Add(existingFood);
                                servingSizesToDelete.AddRange(existingFood.FoodServingSizes);

                                fss.FoodId = existingFood.FoodId;
                                fss.Food = null;
                                servingSizesToInsert.Add(fss);
                            }
                            else
                            {
                                var newFood = new Food
                                {
                                    FoodName = foodName,
                                    MealType = mealType,
                                    FoodType = foodType,
                                    ServingSizeId = servingSizeId,
                                    Description = description,
                                    FoodServingSizes = new List<FoodServingSize> { fss }
                                };
                                addedFoods.Add(newFood);
                            }
                        }
                    }
                }

                // Batch add mới
                if (addedFoods.Count > 0)
                {
                    await _unitOfWork.FoodRepository.AddRangeAsync(addedFoods);
                    await _unitOfWork.SaveChangesAsync();

                    foreach (var food in addedFoods)
                    {
                        var fss = food.FoodServingSizes.FirstOrDefault();
                        if (fss != null)
                        {
                            fss.FoodId = food.FoodId;
                            fss.Food = null;
                            servingSizesToInsert.Add(fss);
                        }
                    }
                }

                // Cập nhật món ăn
                foreach (var food in updatedFoods)
                {
                    await _unitOfWork.FoodRepository.UpdateAsync(food);
                }
                await _unitOfWork.SaveChangesAsync();

                // Xoá serving size cũ
                if (servingSizesToDelete.Count > 0)
                {
                    await _unitOfWork.FoodServingSizeRepository.RemoveRange(servingSizesToDelete);
                    await _unitOfWork.SaveChangesAsync();
                }

                // Thêm mới serving size mới
                if (servingSizesToInsert.Count > 0)
                {
                    await _unitOfWork.FoodServingSizeRepository.AddRangeAsync(servingSizesToInsert);
                    await _unitOfWork.SaveChangesAsync();
                }

                string message = $"Import thành công: {addedFoods.Count} món mới, {updatedFoods.Count} món được cập nhật.";
                if (duplicateCount > 0) message += $" {duplicateCount} dòng bị bỏ qua do trùng tên.";
                if (addedFoods.Count == 0 && updatedFoods.Count == 0 && duplicateCount == 0)
                    message = "Không có món ăn nào được import.";

                return new BusinessResult(Const.HTTP_STATUS_OK, message);
            }
            catch (DbUpdateException dbEx)
            {
                var sqlEx = dbEx.InnerException as SqlException;
                if (sqlEx?.Number == 2627)
                    return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Dữ liệu bị trùng lặp trong database.");

                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, $"Lỗi khi import: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, $"Lỗi khi import: {ex.Message}");
            }
        }

    }
}
