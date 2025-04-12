using Mapster;
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
        private static readonly HttpClient _httpClient = new();

        public async Task<IBusinessResult> GetProductFromBarcodeAsync(string barcode)
        {
            try
            {
                string url = $"https://world.openfoodfacts.net/api/v2/product/{barcode}?fields=product_name,nutriments,nutrition_grades,serving_size,image_front_url&lang=vi";

                var response = await _httpClient.GetStringAsync(url);
                var json = JObject.Parse(response);

                // Kiểm tra sản phẩm có tồn tại không
                if (json["status"]?.ToObject<int>() != 1)
                {
                    return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Không tìm thấy sản phẩm với barcode này.", null);
                }

                var product = json["product"];
                var nutriments = product["nutriments"];

                string productName = product["product_name"]?.ToString() ?? "Không xác định";
                string nutritionGrade = product["nutrition_grades"]?.ToString() ?? "Không có";
                string servingSize = product["serving_size"]?.ToString() ?? "100g";
                string imageUrl = product["image_front_url"]?.ToString();

                // ✅ Lấy giá trị trực tiếp, không dùng serving
                float calories = GetNutrientValue(nutriments, "energy-kcal");
                float protein = GetNutrientValue(nutriments, "proteins");
                float carbs = GetNutrientValue(nutriments, "carbohydrates");
                float fat = GetNutrientValue(nutriments, "fat");
                float fiber = GetNutrientValue(nutriments, "sugars");

                var food = new Food
                {
                    FoodName = productName,
                    MealType = null,
                    ImageUrl = imageUrl,
                    FoodType = null,
                    Description = $"Sản phẩm từ barcode {barcode}, nutrition grade: {nutritionGrade}",
                    //ServingSize = servingSize,
                    //Calories = calories,
                    //Protein = protein,
                    //Carbs = carbs,
                    //Fat = fat,
                    //Glucid = carbs,
                    //Fiber = fiber
                };

                var foodResponse = food.Adapt<FoodResponse>();
                return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, foodResponse);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, $"Lỗi khi lấy dữ liệu từ barcode {barcode}: {ex.Message}", null);
            }
        }

        // ✅ Lấy 1 key duy nhất, đơn giản hóa
        private float GetNutrientValue(JToken nutriments, string key)
        {
            if (nutriments == null || key == null) return 0f;

            return float.TryParse(nutriments[key]?.ToString(), out var value) ? value : 0f;
        }
    }
}