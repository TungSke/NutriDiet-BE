using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;

namespace NutriDiet.Repository.Repositories
{
    public class CuisineRepository : GenericRepository<CuisineType>, ICuisineRepository
    {
        public CuisineRepository(NutriDietContext context) : base(context)
        {
        }
    }
}
