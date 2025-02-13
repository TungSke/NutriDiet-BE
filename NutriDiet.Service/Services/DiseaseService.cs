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
using System.Threading.Tasks;

namespace NutriDiet.Service.Services
{
    public class DiseaseService : IDiseaseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userIdClaim;

        public DiseaseService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
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

        public async Task<IBusinessResult> CreateDisease(DiseaseRequest request)
        {
            if (request == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Invalid request");
            }

            // Check if a disease with the same name already exists (case-insensitive)
            var existingDisease = await _unitOfWork.DiseaseRepository.GetByWhere(
                d => d.DiseaseName.ToLower() == request.DiseaseName.ToLower()).FirstOrDefaultAsync();

            if (existingDisease != null)
            {
                return new BusinessResult(Const.HTTP_STATUS_CONFLICT, "Disease already exists.");
            }

            // Map the request to a Disease entity
            var diseaseEntity = request.Adapt<Disease>();

            // Add the new Disease to the repository
            await _unitOfWork.DiseaseRepository.AddAsync(diseaseEntity);
            await _unitOfWork.SaveChangesAsync();

            var response = diseaseEntity.Adapt<DiseaseResponse>();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_CREATE_MSG, response);
        }

        public async Task<IBusinessResult> DeleteDisease(int diseaseId)
        {
            var disease = await _unitOfWork.DiseaseRepository.GetByIdAsync(diseaseId);
            if (disease == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Disease not found");
            }

            await _unitOfWork.DiseaseRepository.DeleteAsync(disease);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_DELETE_MSG);
        }

        public async Task<IBusinessResult> GetAllDisease(int pageIndex, int pageSize, string diseaseName)
        {
            string searchTerm = diseaseName?.ToLower() ?? string.Empty;

            var diseases = await _unitOfWork.DiseaseRepository.GetPagedAsync(
                pageIndex,
                pageSize,
                x => string.IsNullOrEmpty(searchTerm) || x.DiseaseName.ToLower().Contains(searchTerm)
            );

            diseases = diseases.Distinct().ToList();

            if (diseases == null || !diseases.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
            }

            var response = diseases.Adapt<List<DiseaseResponse>>();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> GetDiseaseById(int diseaseId)
        {
            var disease = await _unitOfWork.DiseaseRepository
                .GetByWhere(x => x.DiseaseId == diseaseId)
                .FirstOrDefaultAsync();

            if (disease == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Disease not found");
            }

            var response = disease.Adapt<DiseaseResponse>();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> UpdateDisease(DiseaseRequest request, int diseaseId)
        {
            if (request == null || diseaseId <= 0)
            {
                return new BusinessResult(Const.HTTP_STATUS_BAD_REQUEST, "Invalid request or missing DiseaseId");
            }

            var disease = await _unitOfWork.DiseaseRepository.GetByIdAsync(diseaseId);
            if (disease == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Disease not found");
            }

            // Check for duplicate disease names on other records (case-insensitive)
            var conflictDisease = await _unitOfWork.DiseaseRepository.GetByWhere(
                d => d.DiseaseName.ToLower() == request.DiseaseName.ToLower() && d.DiseaseId != diseaseId)
                .FirstOrDefaultAsync();
            if (conflictDisease != null)
            {
                return new BusinessResult(Const.HTTP_STATUS_CONFLICT, "Another disease with the same name already exists.");
            }

            // Map updated properties from the request into the existing entity
            request.Adapt(disease);

            await _unitOfWork.DiseaseRepository.UpdateAsync(disease);
            await _unitOfWork.SaveChangesAsync();

            var response = disease.Adapt<DiseaseResponse>();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_UPDATE_MSG, response);
        }
    }
}
