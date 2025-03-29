using NutriDiet.Common.BusinessResult;
using NutriDiet.Service.ModelDTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Interface
{
    public interface IPackageService
    {
        Task<IBusinessResult> GetPackage();
        Task<IBusinessResult> GetPackageById(int packageId);
        Task<IBusinessResult> CreatePackage(PackageRequest request);
        Task<IBusinessResult> UpdatePackage(int packageId, PackageRequest request);
        Task<IBusinessResult> DeletePackage(int packageId);
        Task<IBusinessResult> GetUserPackage(int pageIndex, int pageSize, string? status, string? search);
        Task<IBusinessResult> PayforPackage(string cancelUrl, string returnUrl, int packageId);
        Task<IBusinessResult> PAYOSCallback(string status);
    }
}
