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
    public class FoodServingSizeRepository : GenericRepository<FoodServingSize>, IFoodServingSizeRepository
    {
        public FoodServingSizeRepository(NutriDietContext context) : base(context)
        {
        }

        public async Task<IEnumerable<FoodServingSize>> GetFoodServingSizesByFoodIdAsync(int foodId)
        {
            return await _context.FoodServingSizes
                .Where(fss => fss.FoodId == foodId)
                .ToListAsync();
        }
    }
}
