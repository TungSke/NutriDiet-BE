using NutriDiet.Common.BusinessResult;
using NutriDiet.Service.ModelDTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Interface
{
    public interface ISystemConfigurationService
    {
        Task<IBusinessResult> GetSystemConfig(int pageIndex, int pageSize, string search);
        Task<IBusinessResult> CreateSystemConfig(SystemConfigurationRequest request);
        Task<IBusinessResult> UpdateSystemConfig(int configId, SystemConfigurationRequest request);
        Task<IBusinessResult> DeleteSystemConfig(int configId);
        Task<IBusinessResult> CheckMySystemConfig();
    }
}
