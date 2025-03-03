using NutriDiet.Repository.Base;
using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Repository.Repositories
{
    public class AIRecommendationRepository : GenericRepository<Airecommendation>, IAIRecommendationRepository
    {
        public AIRecommendationRepository(NutriDietContext context) : base(context)
        {
        }
    }
}
