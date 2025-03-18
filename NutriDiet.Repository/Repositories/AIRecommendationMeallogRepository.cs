using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;

namespace NutriDiet.Repository.Repositories
{
    public class AIRecommendationMeallogRepository : GenericRepository<AirecommendMealLog>, IAIRecommendationMeallogRepository
    {
        public AIRecommendationMeallogRepository(NutriDietContext context) : base(context)
        {
        }
    }
}
