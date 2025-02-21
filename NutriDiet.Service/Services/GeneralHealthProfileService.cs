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
using System.Security.Claims;
using System.Threading.Tasks;

namespace NutriDiet.Service.Services
{
    public class GeneralHealthProfileService : IGeneralHealthProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userIdClaim;

        public GeneralHealthProfileService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
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

        public async Task CreateHealthProfileRecord(HealthProfileRequest request)
        {
            var userid = int.Parse(_userIdClaim);
            var existingUser = await _unitOfWork.UserRepository.GetByIdAsync(userid);
            if (existingUser == null)
            {
                throw new Exception("User not exist.");
            }

            request.Adapt(existingUser);

            var healthProfile = request.Adapt<GeneralHealthProfile>();

            await _unitOfWork.BeginTransaction();
            try
            {
                // Update User Table
                await _unitOfWork.UserRepository.UpdateAsync(existingUser);
                // Add new Health profile Record for this User
                healthProfile.UserId = existingUser.UserId; 
                await _unitOfWork.HealthProfileRepository.AddAsync(healthProfile);
                // Add Allergy for User if any
                if (request.AllergyNames != null && request.AllergyNames.Any())
                {
                    foreach (var allergyName in request.AllergyNames)
                    {
                        var existingAllergy = await _unitOfWork.AllergyRepository.GetByWhere(a => a.AllergyName.ToLower() == allergyName.ToLower()).FirstOrDefaultAsync();
                        if (existingAllergy != null)
                        {
                            if (!existingUser.Allergies.Any(a => a.AllergyId == existingAllergy.AllergyId))
                            {
                                existingUser.Allergies.Add(existingAllergy);
                            }
                        }
                        else
                        {
                            throw new Exception($"Allergy '{allergyName}' does not exist in the system.");
                        }
                    }
                }
                // Add Disease for User if any
                if (request.DiseaseNames != null && request.DiseaseNames.Any())
                {
                    foreach (var diseaseName in request.DiseaseNames)
                    {
                        var existingDisease = await _unitOfWork.DiseaseRepository
                            .GetByWhere(d => d.DiseaseName.ToLower() == diseaseName.ToLower()).FirstOrDefaultAsync();
                        if (existingDisease != null)
                        {
                            if (!existingUser.Diseases.Any(d => d.DiseaseId == existingDisease.DiseaseId))
                            {
                                existingUser.Diseases.Add(existingDisease);
                            }
                        }
                        else
                        {
                            throw new Exception($"Disease '{diseaseName}' does not exist in the system.");
                        }
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        public async Task<IBusinessResult> GetHealthProfile()
        {
            var userid = int.Parse(_userIdClaim);

            var existingUser = await _unitOfWork.UserRepository
                .GetByWhere(u => u.UserId == userid)
                .Include(u => u.Allergies)
                .Include(u => u.Diseases)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (existingUser == null)
            {
                throw new Exception("User not exist.");
            }

            var healthProfile = await _unitOfWork.HealthProfileRepository
                .GetByWhere(hp => hp.UserId == userid)
                .AsNoTracking() 
                .FirstOrDefaultAsync();

            HealthProfileResponse response;
            try
            {
                response = existingUser.Adapt<HealthProfileResponse>();
            }
            catch (Exception ex)
            {
                throw new Exception("Error mapping User to HealthProfileResponse: " + ex.Message, ex);
            }

            if (healthProfile != null)
            {
                try
                {
                    healthProfile.Adapt(response);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error mapping HealthProfile to HealthProfileResponse: " + ex.Message, ex);
                }
            }

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }


        public async Task UpdateHealthProfile(HealthProfileRequest request)
        {
            var userid = int.Parse(_userIdClaim);

            var existingUser = await _unitOfWork.UserRepository
                .GetByWhere(u => u.UserId == userid)
                .Include(u => u.Allergies)
                .Include(u => u.Diseases)
                .FirstOrDefaultAsync();

            if (existingUser == null)
            {
                throw new Exception("User not exist.");
            }

            await _unitOfWork.BeginTransaction();
            try
            {
                request.Adapt(existingUser);

                if (request.AllergyNames != null)
                {
                    existingUser.Allergies.Clear();
                    foreach (var allergyName in request.AllergyNames)
                    {
                        var existingAllergy = await _unitOfWork.AllergyRepository
                            .GetByWhere(a => a.AllergyName.ToLower() == allergyName.ToLower())
                            .FirstOrDefaultAsync();
                        if (existingAllergy != null)
                        {
                            existingUser.Allergies.Add(existingAllergy);
                        }
                        else
                        {
                            throw new Exception($"Allergy '{allergyName}' does not exist in the system.");
                        }
                    }
                }

                if (request.DiseaseNames != null)
                {
                    existingUser.Diseases.Clear();
                    foreach (var diseaseName in request.DiseaseNames)
                    {
                        var existingDisease = await _unitOfWork.DiseaseRepository
                            .GetByWhere(d => d.DiseaseName.ToLower() == diseaseName.ToLower())
                            .FirstOrDefaultAsync();
                        if (existingDisease != null)
                        {
                            existingUser.Diseases.Add(existingDisease);
                        }
                        else
                        {
                            throw new Exception($"Disease '{diseaseName}' does not exist in the system.");
                        }
                    }
                }

                var existingHealthProfile = await _unitOfWork.HealthProfileRepository
                    .GetByWhere(hp => hp.UserId == userid)
                    .FirstOrDefaultAsync();
                if (existingHealthProfile != null)
                {
                    request.Adapt(existingHealthProfile);
                    await _unitOfWork.HealthProfileRepository.UpdateAsync(existingHealthProfile);
                }
                else
                {
                    var newHealthProfile = request.Adapt<GeneralHealthProfile>();
                    newHealthProfile.UserId = existingUser.UserId;
                    await _unitOfWork.HealthProfileRepository.AddAsync(newHealthProfile);
                }

                await _unitOfWork.UserRepository.UpdateAsync(existingUser);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransaction();
                throw;
            }
        }

    }
}
