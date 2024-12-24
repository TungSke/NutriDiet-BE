using LogiConnect.Repository.Base;
using LogiConnect.Repository.Interface;
using LogiConnect.Repository.Models;
using System;

namespace LogiConnect.Repository.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(NutriDietContext context) : base(context)
        {
        }
    }
}