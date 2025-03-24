using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Repository.Repositories
{
    public class UserFoodPreferenceRepository : GenericRepository<UserFoodPreference>, IUserFoodPreferenceRepository
    {
        public UserFoodPreferenceRepository(NutriDietContext context) : base(context)
        {
        }
    }
}
