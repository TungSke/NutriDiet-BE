using NutriDiet.Common.BusinessResult;
using NutriDiet.Service.ModelDTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Interface
{
    public interface IGeneralHealthProfileService
    {
        Task CreateHealthProfileRecord(HealthProfileRequest request);
        Task<IBusinessResult> GetHealthProfile();
        Task<IBusinessResult> DeleteHealthProfile(int userId);
        Task<IBusinessResult> TrackingHealthProfile(string field);
    }
}
