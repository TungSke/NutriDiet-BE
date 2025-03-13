using Google.Apis.Drive.v3.Data;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NutriDiet.Common;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using NutriDiet.Service.ModelDTOs.Response;
using NutriDiet.Service.Utilities;
using System.Security.Claims;

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
                     (string.IsNullOrEmpty(foodType) || x.FoodType.ToLower() == foodType)
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
            var food = await _unitOfWork.FoodRepository.GetByWhere(x => x.FoodId == foodId).FirstOrDefaultAsync();
            if (food == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Food not found");
            }

            var response = food.Adapt<FoodResponse>();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> CreateFood(FoodRequest request)
        {
            var existedFood = await _unitOfWork.FoodRepository
                .GetByWhere(x => x.FoodName.ToLower().Equals(request.FoodName.ToLower()))
                .FirstOrDefaultAsync();

            if (existedFood != null)
            {
                return new BusinessResult(Const.HTTP_STATUS_CONFLICT, "Food name already exists");
            }

            var food = request.Adapt<Food>();

            var validAllergyIds = await _unitOfWork.AllergyRepository
                .GetByWhere(a => request.AllergyId.Contains(a.AllergyId)).AsNoTracking()
                .Select(a => a.AllergyId)
                .ToListAsync();

            var validDiseaseIds = await _unitOfWork.DiseaseRepository
                .GetByWhere(d => request.DiseaseId.Contains(d.DiseaseId)).AsNoTracking()
                .Select(d => d.DiseaseId)
                .ToListAsync();

            

            if (request.FoodImageUrl != null)
            {
                var cloudinaryHelper = new CloudinaryHelper();
                food.ImageUrl = await cloudinaryHelper.UploadImageWithCloudDinary(request.FoodImageUrl);
            }

            await _unitOfWork.FoodRepository.AddAsync(food);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_CREATE_MSG);
        }

        public async Task<IBusinessResult> UpdateFood(int foodId, FoodRequest request)
        {
            var existedFood = await _unitOfWork.FoodRepository.GetByWhere(x => x.FoodId == foodId).FirstOrDefaultAsync();
            if (existedFood == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Food not found");
            }

            request.Adapt(existedFood);

           

            if (request.FoodImageUrl != null)
            {
                var cloudinaryHelper = new CloudinaryHelper();
                existedFood.ImageUrl = await cloudinaryHelper.UploadImageWithCloudDinary(request.FoodImageUrl);
            }

            await _unitOfWork.FoodRepository.UpdateAsync(existedFood);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_UPDATE_MSG);
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

        public async Task<IBusinessResult> GetFoodRecommend(int pageIndex, int pageSize, string searchName)
        {
            int userid = int.Parse(_userIdClaim);
            var userAllergyDisease = await _unitOfWork.UserRepository
                            .GetByWhere(x => x.UserId == userid)
                            .Select(x => new
                            {
                                AllergyIds = x.Allergies.Select(a => a.AllergyId).ToList(),
                                DiseaseIds = x.Diseases.Select(d => d.DiseaseId).ToList()
                            })
                            .FirstOrDefaultAsync();
            if (userAllergyDisease == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "User not found");
            }

            var allergyIds = new List<int>(userAllergyDisease.AllergyIds);
            var diseaseIds = new List<int>(userAllergyDisease.DiseaseIds);

            var foods = await _unitOfWork.FoodRepository
            .GetByWhere(x =>
             x.FoodName.ToLower().Contains(searchName.ToLower()))
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            var foodResponse = foods.Adapt<List<FoodResponse>>();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, foodResponse);
        }

        public async Task<IBusinessResult> CreateFoodRecipeByAI(int foodId, int cuisineId)
        {
            //lấy thông tin tình trạng sức khỏe của user
            int userid = int.Parse(_userIdClaim);
            var userError = await _unitOfWork.UserRepository
                            .GetByWhere(x => x.UserId == userid)
                            .Select(x => new
                            {
                                AllergyNames = x.Allergies.Select(a => a.AllergyName).ToList(),
                                DiseaseNames = x.Diseases.Select(d => d.DiseaseName).ToList()
                            })
                            .FirstOrDefaultAsync();

            if (userError == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "User not found");
            }

            var allergyNames = userError?.AllergyNames ?? new List<string>();
            var diseaseNames = userError?.DiseaseNames ?? new List<string>();

            var formattedAllergies = allergyNames.Any() ? string.Join(", ", allergyNames) : "không có";
            var formattedDiseases = diseaseNames.Any() ? string.Join(", ", diseaseNames) : "không có";

            var food = await _unitOfWork.FoodRepository.GetByIdAsync(foodId);
            var cuisineType = await _unitOfWork.CuisineRepository.GetByIdAsync(cuisineId);

            if (food == null || cuisineType == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, $"{(food == null ? "food" : "cuisine")} not found");
            }

            // kiểm tra xem đã có công thức nấu món ăn này chưa
            var existingRecipe = await _unitOfWork.RecipeSuggestionRepository
                .GetByWhere(x => x.UserId == userid && x.FoodId == foodId)
                .FirstOrDefaultAsync();

            string rejectionReason = existingRecipe?.RejectionReason ?? "";

            string input = $"Tôi có các bệnh này: {formattedDiseases} \n" +
                           $"và dị ứng này: {formattedAllergies} \n" +
                           $"Hãy gợi ý cho tôi công thức để nấu món {food.FoodName}, nấu theo phong cách {cuisineType.CuisineName}.\n";

            if (!string.IsNullOrEmpty(rejectionReason))
            {
                input += $"Lưu ý: Trước đó tôi đã không thích một công thức vì lý do '{rejectionReason}', hãy điều chỉnh lại công thức cho phù hợp.\n";
            }

            input += "Trả lời dưới dạng như này: Công thức của bạn là...";

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
    }
}
