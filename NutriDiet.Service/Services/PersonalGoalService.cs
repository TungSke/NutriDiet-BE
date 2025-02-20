using Microsoft.AspNetCore.Http;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Common;
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
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace NutriDiet.Service.Services
{
    public class PersonalGoalService : IPersonalGoalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userIdClaim;

        public PersonalGoalService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
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

        //public async Task CreatePersonalGoal(PersonalGoalRequest request)
        //{
        //    var userid = int.Parse(_userIdClaim);
        //    var existingUser = await _unitOfWork.UserRepository
        //        .GetByWhere(u => u.UserId == userid)
        //        //.Include(u => u.HealthProfiles)
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync();

        //    if (existingUser == null)
        //    {
        //        throw new Exception("User not exist.");
        //    }
        //    var ProgressRate = Convert.ToInt32(existingUser.HealthProfiles
        //                                .OrderBy(h => h.CreatedAt)
        //                                .FirstOrDefault()?.Weight - existingUser.HealthProfiles
        //                                .OrderBy(h => h.CreatedAt)
        //                                .FirstOrDefault()?.TargetWeight ?? 0);

        //    var personalGoal = request.Adapt<PersonalGoal>();
        //    personalGoal.UserId = userid;
        //    personalGoal.StartDate = DateTime.Now;
        //    personalGoal.Status = "Active";
        //    personalGoal.ProgressPercentage = 0;
        //    personalGoal.ProgressRate = ProgressRate;
        //    await _unitOfWork.PersonalGoalRepository.AddAsync(personalGoal);
        //    await _unitOfWork.SaveChangesAsync();
        //}


        public async Task<IBusinessResult> GetPersonalGoal()
        {
            var userid = int.Parse(_userIdClaim);

            var existingUser = await _unitOfWork.UserRepository.GetByIdAsync(userid);

            if (existingUser == null)
            {
                throw new Exception("User not exist.");
            }

            var personalGoal = await _unitOfWork.PersonalGoalRepository
                .GetByWhere(hp => hp.UserId == userid)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var response = personalGoal.Adapt<PersonalGoalResponse>();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }
        public async Task<IBusinessResult> UpdatePersonalGoal(PersonalGoalRequest request)
        {
            var userId = int.Parse(_userIdClaim);

            var existingGoal = await _unitOfWork.PersonalGoalRepository
                .GetByWhere(pg => pg.UserId == userId)
                .FirstOrDefaultAsync();

            if (existingGoal == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Personal goal not found.", null);
            }

            request.Adapt(existingGoal);

            await _unitOfWork.PersonalGoalRepository.UpdateAsync(existingGoal);
            await _unitOfWork.SaveChangesAsync();

            var response = existingGoal.Adapt<PersonalGoalResponse>();
            return new BusinessResult(Const.HTTP_STATUS_OK, "Personal goal updated successfully.", response);
        }



    }
}
