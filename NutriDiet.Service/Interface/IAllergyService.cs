using NutriDiet.Common.BusinessResult;
using NutriDiet.Service.ModelDTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Interface
{
    public interface IAllergyService 
    {
        Task<IBusinessResult> GetAllAllergy(int pageindex, int pagesize, string AllergyName);
        Task<IBusinessResult> GetAllergyById(int AllergyId);
        Task<IBusinessResult> CreateAllergy(AllergyRequest request);
        Task<IBusinessResult> UpdateAllergy(AllergyRequest request);
        Task<IBusinessResult> DeleteAllergy(int AllergyId);

    }
}
