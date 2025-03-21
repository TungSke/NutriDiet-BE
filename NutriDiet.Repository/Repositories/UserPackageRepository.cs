using Microsoft.EntityFrameworkCore;
using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Repository.Repositories
{
    public class UserPackageRepository : GenericRepository<UserPackage>, IUserPackageRepository
    {
        public UserPackageRepository(NutriDietContext context) : base(context) { }

        public async Task<bool> IsUserPremiumAsync(int userId)
        {
            var activePackage = await GetByWhere(up => up.UserId == userId && up.Status == "Active" && up.ExpiryDate > DateTime.UtcNow)
                .FirstOrDefaultAsync();
            return activePackage != null;
        }
    }

}
