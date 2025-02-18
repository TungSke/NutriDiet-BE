using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Repository.Repositories
{
    public class PersonalGoalRepository : GenericRepository<PersonalGoal>, IPersonalGoalRepository
    {
        public PersonalGoalRepository(NutriDietContext context) : base(context)
        {
        }
    }
}
