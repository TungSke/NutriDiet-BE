using Mapster;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NutriDiet.Common;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Repository.Models;
using NutriDiet.Service.ModelDTOs.Response;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NutriDiet.Service.Utilities
{
    public class BarcodeHelper
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<IBusinessResult> GetProductFromBarcodeAsync(string barcode)
        {
            try
            {
                string url = $"https://world.openfoodfacts.net/api/v2/product/{barcode}?fields=product_name,nutriments,nutrition_grades,serving_size,image_front_url&lang=vi";

                // Gọi API
                var response = await _httpClient.GetStringAsync(url);
                dynamic data = JsonConvert.DeserializeObject(response);

                // Kiểm tra nếu sản phẩm không tồn tại
                if (data.status == 0)
                {
                    return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Không tìm thấy sản phẩm với barcode này.", null);
                }

                // Lấy thông tin sản phẩm
                string productName = data.product.product_name ?? "Không xác định";
                string nutritionGrade = data.product.nutrition_grades ?? "Không có";
                string servingSize = data.product.serving_size ?? "100g";
                string imageUrl = data.product.image_front_url ?? null;

                // Lấy thông tin dinh dưỡng từ nutriments
                var nutriments = data.product.nutriments;

                // ✅ Kiểm tra calories, nếu có giá trị thì lấy, nếu không thì giữ nguyên 0
                float calories = GetNutrientValue(nutriments, "energy-kcal_serving", "energy-kcal_100g");
                float protein = GetNutrientValue(nutriments, "proteins_serving", "proteins_100g");
                float carbs = GetNutrientValue(nutriments, "carbohydrates_serving", "carbohydrates_100g");
                float fat = GetNutrientValue(nutriments, "fat_serving", "fat_100g");
                float fiber = GetNutrientValue(nutriments, "sugars_serving", "sugars_100g");

                // Tạo đối tượng Food
                var food = new Food
                {
                    FoodName = productName,
                    MealType = null,
                    ImageUrl = imageUrl,
                    FoodType = null,
                    Description = $"Sản phẩm từ barcode {barcode}, nutrition grade: {nutritionGrade}",
                    ServingSize = servingSize,
                    Calories = calories,
                    Protein = protein,
                    Carbs = carbs,
                    Fat = fat,
                    Glucid = carbs,
                    Fiber = fiber
                };

                // Chuyển đổi sang FoodResponse
                var foodResponse = food.Adapt<FoodResponse>();

                return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, foodResponse);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, $"Lỗi khi lấy dữ liệu từ barcode {barcode}: {ex.Message}", null);
            }
        }

        /// <summary>
        /// Hàm lấy giá trị dinh dưỡng, ưu tiên giá trị theo khẩu phần trước, nếu không có thì lấy theo 100g
        /// </summary>
        private float GetNutrientValue(dynamic nutriments, string servingKey, string per100gKey)
        {
            if (nutriments == null) return 0f;

            // Lấy giá trị theo khẩu phần trước, nếu không có thì lấy theo 100g, nếu vẫn không có thì trả về 0
            return (nutriments[servingKey] != null && nutriments[servingKey].Type != JTokenType.Null)
                ? (float)nutriments[servingKey]
                : (nutriments[per100gKey] != null && nutriments[per100gKey].Type != JTokenType.Null)
                    ? (float)nutriments[per100gKey]
                    : 0f;
        }

    }
}