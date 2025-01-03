using NutriDiet.Common.BusinessResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Interface
{
    public interface IFoodService
    {
        Task<IBusinessResult> GetAllFood(int pageindex, int pagesize, string search);
    }
}
