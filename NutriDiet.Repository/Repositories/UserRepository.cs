using NutriDiet.Repository.Base;
using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using System;

namespace NutriDiet.Repository.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(NutriDietContext context) : base(context)
        {
        }
    }
}