using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NutriDiet.Common;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using NutriDiet.Service.Helpers;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using NutriDiet.Service.ModelDTOs.Response;
using OfficeOpenXml;

namespace NutriDiet.Service.Services
{
    public class IngreDientSevice : IIngreDientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TokenHandlerHelper _tokenHandlerHelper;

        public IngreDientSevice(IUnitOfWork unitOfWork, TokenHandlerHelper tokenHandlerHelper)
        {
            _unitOfWork = unitOfWork;
            _tokenHandlerHelper = tokenHandlerHelper;
        }

        public async Task<IBusinessResult> GetIngreDients(int pageIndex, int pageSize, string search)
        {
            search = search?.ToLower() ?? string.Empty;

            var foods = await _unitOfWork.IngredientRepository.GetPagedAsync(
                pageIndex,
                pageSize,
                x => (string.IsNullOrEmpty(search) || x.IngredientName.ToLower().Contains(search))
            );

            if (foods == null || !foods.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
            }

            var response = foods.Adapt<List<IngredientResponse>>();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> GetIngredientById(int ingredientId)
        {
            var ingredient = await _unitOfWork.IngredientRepository.GetByIdAsync(ingredientId);
            if (ingredient == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Ingredient not found");
            }

            var response = ingredient.Adapt<IngredientResponse>();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> AddIngredient(IngredientRequest request)
        {
            var ingre = await _unitOfWork.IngredientRepository.GetByWhere(x => x.IngredientName.ToLower() == request.IngredientName).FirstOrDefaultAsync();
            if (ingre != null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Food not found");
            }
            ingre = request.Adapt<Ingredient>();
            await _unitOfWork.IngredientRepository.AddAsync(ingre);
            await _unitOfWork.SaveChangesAsync();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_CREATE_MSG);
        }

        public async Task<IBusinessResult> UpdateIngredient(int ingredientid, IngredientRequest request)
        {
            var ingredient = await _unitOfWork.IngredientRepository.GetByIdAsync(ingredientid);
            if (ingredient == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "ingredient not found");
            }

            request.Adapt(ingredient);
            await _unitOfWork.IngredientRepository.UpdateAsync(ingredient);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_UPDATE_MSG);
        }

        public async Task<IBusinessResult> DeleteIngredient(int ingredientId)
        {
            var ingredient = await _unitOfWork.IngredientRepository.GetByIdAsync(ingredientId);
            if (ingredient == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Ingredient not found");
            }

            await _unitOfWork.IngredientRepository.DeleteAsync(ingredient);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_DELETE_MSG);
        }

        public async Task<IBusinessResult> UpdatePreferenceIngredient(int ingredientId, int preferenceLevel)
        {
            var userId = await _tokenHandlerHelper.GetUserId();

            var ingredients = await _unitOfWork.IngredientRepository.GetByIdAsync(ingredientId);
            if (ingredients == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "No preference ingredient found");
            }

            var userIngredient = await _unitOfWork.UserIngredientPreferenceRepository.GetByWhere(x => x.UserId == userId && x.IngredientId == ingredientId).FirstOrDefaultAsync();
            if (userIngredient == null)
            {
                userIngredient = new UserIngreDientPreference
                {
                    UserId = userId,
                    IngredientId = ingredientId,
                    Level = preferenceLevel
                };
                await _unitOfWork.UserIngredientPreferenceRepository.AddAsync(userIngredient);
            }
            else
            {
                userIngredient.Level = preferenceLevel;
                await _unitOfWork.UserIngredientPreferenceRepository.UpdateAsync(userIngredient);
            }

            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG);
        }

        public async Task<IBusinessResult> GetPreferenceIngredient()
        {
            var userId = await _tokenHandlerHelper.GetUserId();
            var userIngredients = await _unitOfWork.UserIngredientPreferenceRepository.GetByWhere(x => x.UserId == userId).ToListAsync();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, userIngredients);
        }

        public async Task<IBusinessResult> ImportIngredientsFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length <= 0)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "File không hợp lệ");
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var ingredientsToImport = new List<Ingredient>();

            try
            {
                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        // Lấy danh sách IngredientName hiện có trong database
                        var existingIngredientNames = await _unitOfWork.IngredientRepository
                            .GetAll()
                            .Select(i => i.IngredientName.ToLower().Trim())
                            .ToListAsync();

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var ingredientName = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                            var calories = worksheet.Cells[row, 3].Value?.ToString()?.Trim();     
                            var protein = worksheet.Cells[row, 4].Value?.ToString()?.Trim();      
                            var carbs = worksheet.Cells[row, 5].Value?.ToString()?.Trim();         
                            var fat = worksheet.Cells[row, 6].Value?.ToString()?.Trim();           

                            // Kiểm tra dữ liệu hợp lệ
                            if (string.IsNullOrEmpty(ingredientName))
                            {
                                continue; // Bỏ qua nếu tên nguyên liệu rỗng
                            }

                            // Kiểm tra xem IngredientName đã tồn tại chưa (không phân biệt hoa thường)
                            if (existingIngredientNames.Contains(ingredientName.ToLower().Trim()))
                            {
                                continue;
                            }

                            // Tạo mới Ingredient
                            ingredientsToImport.Add(new Ingredient
                            {
                                IngredientName = ingredientName,
                                Calories = double.TryParse(calories, out var cal) ? cal : 0,
                                Protein = double.TryParse(protein, out var prot) ? prot : 0,
                                Carbs = double.TryParse(carbs, out var carb) ? carb : 0,
                                Fat = double.TryParse(fat, out var fats) ? fats : 0,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            });
                        }
                    }
                }

                if (ingredientsToImport.Any())
                {
                    await _unitOfWork.IngredientRepository.AddRangeAsync(ingredientsToImport);
                    await _unitOfWork.SaveChangesAsync();

                    return new BusinessResult(Const.HTTP_STATUS_OK, $"Import thành công {ingredientsToImport.Count} nguyên liệu mới");
                }
                else
                {
                    return new BusinessResult(Const.HTTP_STATUS_OK, "Không có nguyên liệu mới nào được import (có thể tất cả đã tồn tại)");
                }
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, $"Lỗi khi import: {ex.Message}");
            }
        }
    }
}
