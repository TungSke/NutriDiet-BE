using NutriDiet.Common.BusinessResult;
using NutriDiet.Service.ModelDTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Interface
{
    public interface IDiseaseService
    {
        Task<IBusinessResult> CreateDisease(DiseaseRequest request);
        Task<IBusinessResult> DeleteDisease(int diseaseId);
        Task<IBusinessResult> GetAllDisease(int pageIndex, int pageSize, string diseaseName);
        Task<IBusinessResult> GetDiseaseById(int diseaseId);
        Task<IBusinessResult> UpdateDisease(DiseaseRequest request, int diseaseId);
    }
}
