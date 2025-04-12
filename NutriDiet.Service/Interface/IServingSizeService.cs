using NutriDiet.Common.BusinessResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Interface
{
    public interface IServingSizeService
    {
        Task<IBusinessResult> GetAllServingSize();
        Task<IBusinessResult> GetServingSizeById(int id);
    }
}
