using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;

namespace NutriDiet.Repository.Repositories
{
    public class ServingSizeRepository : GenericRepository<ServingSize>, IServingSizeRepository
    {
        public ServingSizeRepository(NutriDietContext context) : base(context)
        {
        }
    }
}
