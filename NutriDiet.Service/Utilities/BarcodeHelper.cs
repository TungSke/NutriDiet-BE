using Mapster;
using Newtonsoft.Json;
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

                // Kiểm tra nếu sản phẩm tồn tại
                if (data.status == 0)
                {
                    return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Không tìm thấy sản phẩm với barcode này.", null);
                }

                // Lấy thông tin từ API
                string productName = data.product.product_name ?? "Không xác định";
                string nutritionGrade = data.product.nutrition_grades ?? "Không có";
                string servingSize = data.product.serving_size ?? "100g";
                string imageUrl = data.product.image_front_url ?? null; // Lấy URL hình ảnh

                // Lấy thông tin dinh dưỡng từ nutriments
                var nutriments = data.product.nutriments;
                // Ưu tiên giá trị theo khẩu phần (_serving), nếu không có thì dùng 100g
                float calories = nutriments?.energy_kcal_serving ?? nutriments?.energy_kcal_100g ?? 0f;
                float protein = nutriments?.proteins_serving ?? nutriments?.proteins_100g ?? 0f;
                float carbs = nutriments?.carbohydrates_serving ?? nutriments?.carbohydrates_100g ?? 0f;
                float fat = nutriments?.fat_serving ?? nutriments?.fat_100g ?? 0f;
                float fiber = nutriments?.fiber_serving ?? nutriments?.fiber_100g ?? 0f;

                // Tạo đối tượng Food
                var food = new Food
                {
                    FoodName = productName,
                    MealType = null, // Có thể thêm logic xác định
                    ImageUrl = imageUrl, // Gán ImageUrl từ API
                    FoodType = null,
                    Description = $"Sản phẩm từ barcode {barcode}, nutrition grade: {nutritionGrade}",
                    ServingSize = servingSize,
                    Calories = calories,
                    Protein = protein,
                    Carbs = carbs,
                    Fat = fat,
                    Glucid = carbs, // Glucid trùng với Carbs nếu không có dữ liệu riêng
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
    }
}