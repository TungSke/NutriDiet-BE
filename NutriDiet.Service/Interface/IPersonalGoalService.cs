using NutriDiet.Common.BusinessResult;
using NutriDiet.Repository.Models;
using NutriDiet.Service.ModelDTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Interface
{
    public interface IPersonalGoalService
    {
        Task CreatePersonalGoal(PersonalGoalRequest request);
        Task<IBusinessResult> GetPersonalGoal();
        Task<IBusinessResult> GetAllPersonalGoals();
        Task<IBusinessResult> UpdateDailyMacronutrients(EditDailyMacronutrientsRequest request);
        Task<IBusinessResult> ValidateBMIBasedGoal(PersonalGoalRequest request);
    }
}
