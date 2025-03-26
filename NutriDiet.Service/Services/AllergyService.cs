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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Services
{
    public class AllergyService : IAllergyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userIdClaim;

        public AllergyService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _userIdClaim = GetUserIdClaim();
        }
        private string GetUserIdClaim()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public async Task<IBusinessResult> CreateAllergy(AllergyRequest request)
        {
            if (request == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Invalid request");
            }

            var existingAllergy = await _unitOfWork.AllergyRepository.GetByWhere(
                a => a.AllergyName.ToLower() == request.AllergyName.ToLower()).FirstOrDefaultAsync();

            if (existingAllergy != null)
            {
                return new BusinessResult(Const.HTTP_STATUS_CONFLICT, "Allergy already exists.");
            }

            // Map dữ liệu từ request sang entity
            var allergyEntity = request.Adapt<Allergy>();
            allergyEntity.CreatedAt = DateTime.Now;
            allergyEntity.UpdatedAt = DateTime.Now;

            // Nếu có danh sách Ingredient cần tránh thì xử lý thêm vào Allergy
            if (request.ingredientIds != null && request.ingredientIds.Any())
            {
                // Lấy danh sách Ingredient từ IngredientRepository dựa trên ingredientIds
                var ingredients = await _unitOfWork.IngredientRepository
                    .GetByWhere(i => request.ingredientIds.Contains(i.IngredientId))
                    .ToListAsync();

                // Kiểm tra nếu không tìm thấy một hoặc nhiều Ingredient theo yêu cầu
                if (ingredients.Count != request.ingredientIds.Count)
                {
                    return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "One or more ingredients not found.");
                }

                // Thêm các Ingredient vào Allergy
                foreach (var ingredient in ingredients)
                {
                    allergyEntity.Ingredients.Add(ingredient);
                }
            }

            await _unitOfWork.AllergyRepository.AddAsync(allergyEntity);
            await _unitOfWork.SaveChangesAsync();
            var response = allergyEntity.Adapt<AllergyResponse>();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_CREATE_MSG, response);
        }

        public async Task<IBusinessResult> DeleteAllergy(int allergyId)
        {
            var allergy = await _unitOfWork.AllergyRepository.GetByIdAsync(allergyId);
            if (allergy == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Allergy not found");
            }

            await _unitOfWork.AllergyRepository.DeleteAsync(allergy);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_DELETE_MSG);
        }

        public async Task<IBusinessResult> GetAllAllergy(int pageIndex, int pageSize, string allergyName)
        {
            string searchTerm = allergyName?.ToLower() ?? string.Empty;

            var allergies = await _unitOfWork.AllergyRepository.GetPagedAsync(
                pageIndex,
                pageSize,
                x => string.IsNullOrEmpty(searchTerm) || x.AllergyName.ToLower().Contains(searchTerm)
            );
            allergies = allergies.Distinct().ToList();
            if (allergies == null || !allergies.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
            }

            var response = allergies.Adapt<List<AllergyResponse>>();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> GetAllergyById(int allergyId)
        {
            var allergy = await _unitOfWork.AllergyRepository
                .GetByWhere(x => x.AllergyId == allergyId)
                .FirstOrDefaultAsync();

            if (allergy == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Allergy not found");
            }

            var response = allergy.Adapt<AllergyResponse>();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> UpdateAllergy(AllergyRequest request, int allergyId)
        {
            if (request == null || allergyId <= 0)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Invalid request or missing AllergyId");
            }

            var allergy = await _unitOfWork.AllergyRepository.GetByIdAsync(allergyId);
            if (allergy == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Allergy not found");
            }
            var conflictAllergy = await _unitOfWork.AllergyRepository.GetByWhere(
                a => a.AllergyName.ToLower() == request.AllergyName.ToLower() && a.AllergyId != allergyId).FirstOrDefaultAsync();
            if (conflictAllergy != null)
            {
                return new BusinessResult(Const.HTTP_STATUS_CONFLICT, "Another allergy with the same name already exists.");
            }

            // Cập nhật thông tin của allergy
            allergy.UpdatedAt = DateTime.Now;
            request.Adapt(allergy);

            // Nếu có danh sách Ingredient cần tránh thì cập nhật lại danh sách Ingredient cho Allergy
            if (request.ingredientIds != null)
            {
                // Xóa toàn bộ Ingredient hiện có của Allergy
                allergy.Ingredients.Clear();

                // Lấy danh sách Ingredient từ IngredientRepository dựa trên ingredientIds
                var ingredients = await _unitOfWork.IngredientRepository
                    .GetByWhere(i => request.ingredientIds.Contains(i.IngredientId))
                    .ToListAsync();

                // Kiểm tra nếu không tìm thấy một hoặc nhiều Ingredient theo yêu cầu
                if (ingredients.Count != request.ingredientIds.Count)
                {
                    return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "One or more ingredients not found.");
                }

                // Thêm tất cả các Ingredient vào Allergy
                foreach (var ingredient in ingredients)
                {
                    allergy.Ingredients.Add(ingredient);
                }
            }

            await _unitOfWork.AllergyRepository.UpdateAsync(allergy);
            await _unitOfWork.SaveChangesAsync();

            var response = allergy.Adapt<AllergyResponse>();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_UPDATE_MSG, response);
        }

        public async Task<IBusinessResult> GetAvoidIngredientsByAllergyId(int allergyId)
        {
            // Truy vấn Allergy kèm theo các Ingredient được liên kết
            var allergy = await _unitOfWork.AllergyRepository
                .GetByWhere(a => a.AllergyId == allergyId)
                .Include(a => a.Ingredients)
                .FirstOrDefaultAsync();

            if (allergy == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Allergy not found");
            }

            // Lấy danh sách Ingredient cần tránh từ thuộc tính Ingredients của Allergy
            var avoidIngredients = allergy.Ingredients.ToList();

            // Nếu cần, map sang DTO (ví dụ: IngredientResponse)
            var response = avoidIngredients.Adapt<List<IngredientResponse>>();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }
    }
}
