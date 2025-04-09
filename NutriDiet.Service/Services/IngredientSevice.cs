using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
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
using System.Text;

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

        public async Task<IBusinessResult> AnalyzeIngredientImport(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length <= 0)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "File không hợp lệ");
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var newIngredients = new List<Ingredient>();
            var duplicateIngredients = new List<Ingredient>();
            var processedIngredientNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name.Equals("Ingredient", StringComparison.OrdinalIgnoreCase));
                        if (worksheet == null)
                        {
                            return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Không tìm thấy worksheet với tên 'Ingredient'");
                        }
                        var rowCount = worksheet.Dimension.Rows;

                        var existingIngredientNames = await _unitOfWork.IngredientRepository
                            .GetAll()
                            .Select(i => i.IngredientName.ToLower().Trim())
                            .ToListAsync();

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var ingredientName = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                            if (string.IsNullOrEmpty(ingredientName))
                            {
                                continue;
                            }

                            ingredientName = ingredientName.Normalize(NormalizationForm.FormC);

                            if (processedIngredientNames.Contains(ingredientName.ToLower().Trim()))
                            {
                                duplicateIngredients.Add(new Ingredient { IngredientName = ingredientName });
                                continue;
                            }

                            var ingredient = new Ingredient
                            {
                                IngredientName = ingredientName,
                                Calories = double.TryParse(worksheet.Cells[row, 2].Value?.ToString()?.Trim(), out var cal) ? cal : 0,
                                Protein = double.TryParse(worksheet.Cells[row, 3].Value?.ToString()?.Trim(), out var prot) ? prot : 0,
                                Fat = double.TryParse(worksheet.Cells[row, 4].Value?.ToString()?.Trim(), out var fats) ? fats : 0,
                                Carbs = double.TryParse(worksheet.Cells[row, 5].Value?.ToString()?.Trim(), out var carb) ? carb : 0
                            };

                            processedIngredientNames.Add(ingredientName.ToLower().Trim());

                            if (existingIngredientNames.Contains(ingredientName.ToLower().Trim()))
                            {
                                duplicateIngredients.Add(ingredient);
                            }
                            else
                            {
                                newIngredients.Add(ingredient);
                            }
                        }
                    }
                }

                var result = new
                {
                    NewIngredientCount = newIngredients.Count,
                    DuplicateIngredientCount = duplicateIngredients.Count,
                    DuplicateIngredientName = duplicateIngredients.Select(x=>x.IngredientName).ToList()
                };

                return new BusinessResult(Const.HTTP_STATUS_OK, "Phân tích thành công", result);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, $"Lỗi khi phân tích: {ex.Message}");
            }
        }

        public async Task<IBusinessResult> ImportIngredientsFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length <= 0)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "File không hợp lệ");
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var ingredientsToImport = new List<Ingredient>();
            int duplicateCount = 0;
            var processedIngredientNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name.Equals("Ingredient", StringComparison.OrdinalIgnoreCase));
                        if (worksheet == null)
                        {
                            return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Không tìm thấy worksheet với tên 'Ingredient'");
                        }
                        var rowCount = worksheet.Dimension.Rows;

                        var existingIngredientNames = await _unitOfWork.IngredientRepository
                            .GetAll()
                            .Select(i => i.IngredientName.ToLower().Trim())
                            .ToListAsync();

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var ingredientName = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                            if (string.IsNullOrEmpty(ingredientName))
                            {
                                continue;
                            }

                            ingredientName = ingredientName.Normalize(NormalizationForm.FormC);

                            if (processedIngredientNames.Contains(ingredientName.ToLower().Trim()))
                            {
                                duplicateCount++;
                                continue;
                            }

                            if (existingIngredientNames.Contains(ingredientName.ToLower().Trim()))
                            {
                                duplicateCount++;
                                continue;
                            }

                            processedIngredientNames.Add(ingredientName.ToLower().Trim());

                            ingredientsToImport.Add(new Ingredient
                            {
                                IngredientName = ingredientName,
                                Calories = double.TryParse(worksheet.Cells[row, 2].Value?.ToString()?.Trim(), out var cal) ? cal : 0,
                                Protein = double.TryParse(worksheet.Cells[row, 3].Value?.ToString()?.Trim(), out var prot) ? prot : 0,
                                Carbs = double.TryParse(worksheet.Cells[row, 5].Value?.ToString()?.Trim(), out var carb) ? carb : 0,
                                Fat = double.TryParse(worksheet.Cells[row, 4].Value?.ToString()?.Trim(), out var fats) ? fats : 0,
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
                }

                string message = $"Import thành công {ingredientsToImport.Count} nguyên liệu mới.";
                if (duplicateCount > 0)
                {
                    message += $" {duplicateCount} nguyên liệu bị bỏ qua do trùng lặp.";
                }
                if (!ingredientsToImport.Any() && duplicateCount == 0)
                {
                    message = "Không có nguyên liệu mới nào được import.";
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

        public async Task<IBusinessResult> ImportAndUpdateIngredientsFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length <= 0)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "File không hợp lệ");
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var ingredientsToAdd = new List<Ingredient>();
            var ingredientsToUpdate = new List<Ingredient>();
            var processedIngredientNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            int duplicateCount = 0;

            try
            {
                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name.Equals("Ingredient", StringComparison.OrdinalIgnoreCase));
                        if (worksheet == null)
                        {
                            return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Không tìm thấy worksheet với tên 'Ingredient'");
                        }
                        var rowCount = worksheet.Dimension.Rows;

                        var existingIngredients = await _unitOfWork.IngredientRepository
                            .GetAll()
                            .ToDictionaryAsync(i => i.IngredientName.ToLower().Trim(), i => i);

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var ingredientName = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                            if (string.IsNullOrEmpty(ingredientName))
                            {
                                continue;
                            }

                            ingredientName = ingredientName.Normalize(NormalizationForm.FormC);

                            if (processedIngredientNames.Contains(ingredientName.ToLower().Trim()))
                            {
                                duplicateCount++;
                                continue;
                            }

                            var ingredient = new Ingredient
                            {
                                IngredientName = ingredientName,
                                Calories = double.TryParse(worksheet.Cells[row, 2].Value?.ToString()?.Trim(), out var cal) ? cal : 0,
                                Protein = double.TryParse(worksheet.Cells[row, 3].Value?.ToString()?.Trim(), out var prot) ? prot : 0,
                                Fat = double.TryParse(worksheet.Cells[row, 4].Value?.ToString()?.Trim(), out var fats) ? fats : 0,
                                Carbs = double.TryParse(worksheet.Cells[row, 5].Value?.ToString()?.Trim(), out var carb) ? carb : 0,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };

                            processedIngredientNames.Add(ingredientName.ToLower().Trim());

                            if (existingIngredients.TryGetValue(ingredientName.ToLower().Trim(), out var existingIngredient))
                            {
                                existingIngredient.Calories = ingredient.Calories;
                                existingIngredient.Protein = ingredient.Protein;
                                existingIngredient.Fat = ingredient.Fat;
                                existingIngredient.Carbs = ingredient.Carbs;
                                existingIngredient.UpdatedAt = DateTime.UtcNow;
                                ingredientsToUpdate.Add(existingIngredient);
                            }
                            else
                            {
                                ingredientsToAdd.Add(ingredient);
                            }
                        }
                    }
                }

                if (ingredientsToAdd.Any())
                {
                    await _unitOfWork.IngredientRepository.AddRangeAsync(ingredientsToAdd);
                }
                if (ingredientsToUpdate.Any())
                {
                    await _unitOfWork.IngredientRepository.UpdateRangeAsync(ingredientsToUpdate);
                }
                if (ingredientsToAdd.Any() || ingredientsToUpdate.Any())
                {
                    await _unitOfWork.SaveChangesAsync();
                }

                string message = $"Import thành công: {ingredientsToAdd.Count} nguyên liệu mới được thêm, {ingredientsToUpdate.Count} nguyên liệu được cập nhật.";
                if (duplicateCount > 0)
                {
                    message += $" {duplicateCount} nguyên liệu bị bỏ qua do trùng lặp trong file.";
                }
                if (!ingredientsToAdd.Any() && !ingredientsToUpdate.Any() && duplicateCount == 0)
                {
                    message = "Không có nguyên liệu nào được import.";
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

    }
}
