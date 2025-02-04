using Mapster;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using System;
using System.Threading.Tasks;

namespace NutriDiet.Service.Services
{
    public class HealthProfileService : IHealthProfileService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HealthProfileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddUserHealthRecord(UserHealthRequest request)
        {
            var existingUser = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
            if (existingUser == null)
            {
                throw new Exception("User not exist.");
            }

            request.Adapt(existingUser);

            var healthProfile = request.Adapt<HealthProfile>();

            await _unitOfWork.BeginTransaction();
            try
            {
                await _unitOfWork.UserRepository.UpdateAsync(existingUser);
                await _unitOfWork.SaveChangesAsync();

                healthProfile.UserId = existingUser.UserId; 

                await _unitOfWork.HealthProfileRepository.AddAsync(healthProfile);
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
