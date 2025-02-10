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
            var userid = 8;

            var existingUser = await _unitOfWork.UserRepository.GetByIdAsync(userid);
            if (existingUser == null)
            {
                throw new Exception("User not exist.");
            } 
            try
            {
                var existedAllergy = await _unitOfWork.AllergyRepository
                .GetByWhere(x => x.AllergyName.ToLower() == request.AllergyName.ToLower() && x.UserId == userid)
                .FirstOrDefaultAsync();

                if (existedAllergy != null)
                {
                    return new BusinessResult(Const.HTTP_STATUS_CONFLICT, "Allergy name already exists");
                }

                var allergy = request.Adapt<Allergy>();
                allergy.UserId = 8;
                await _unitOfWork.AllergyRepository.AddAsync(allergy);
                await _unitOfWork.SaveChangesAsync();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
            }

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_CREATE_MSG);
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

        public async Task<IBusinessResult> UpdateAllergy(AllergyRequest request)
        {
            //var allergy = await _unitOfWork.AllergyRepository.GetByIdAsync(request.AllergyId);
            //if (allergy == null)
            //{
            //    return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Allergy not found");
            //}

            //var duplicateAllergy = await _unitOfWork.AllergyRepository
            //    .GetByWhere(x => x.AllergyName.ToLower() == request.AllergyName.ToLower() && x.AllergyId != request.AllergyId)
            //    .FirstOrDefaultAsync();

            //if (duplicateAllergy != null)
            //{
            //    return new BusinessResult(Const.HTTP_STATUS_CONFLICT, "Allergy name already exists");
            //}

            //request.Adapt(allergy);

            //await _unitOfWork.AllergyRepository.UpdateAsync(allergy);
            //await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_UPDATE_MSG);
        }
    }

}
