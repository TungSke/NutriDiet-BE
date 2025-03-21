using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NutriDiet.Common;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using NutriDiet.Service.ModelDTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Services
{
    public class PackageService : IPackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PackageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IBusinessResult> GetAllPackage()
        {
            var packages =  _unitOfWork.PackageRepository.GetAll().ToList();
            if(packages == null || !packages.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
            }
            var response = packages.Adapt<List<PackageResponse>>();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }
        public async Task<IBusinessResult> CreatePackage(PackageRequest request)
        {

            var existingPackage = await _unitOfWork.PackageRepository
                .GetByWhere(p => p.PackageName == request.PackageName)
                .FirstOrDefaultAsync();
            if (existingPackage != null)
            {
                return new BusinessResult(Const.HTTP_STATUS_CONFLICT, "Tên gói đã tồn tại");
            }
            
            var package = request.Adapt<Package>();
            package.CreatedAt = DateTime.UtcNow;
            package.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.PackageRepository.AddAsync(package);
            await _unitOfWork.SaveChangesAsync();

            var response = package.Adapt<PackageResponse>();
            return new BusinessResult(Const.HTTP_STATUS_CREATED, Const.SUCCESS_CREATE_MSG, response);
        }

        public async Task<IBusinessResult> UpdatePackage(int packageId, PackageRequest request)
        {

            var package = await _unitOfWork.PackageRepository.GetByIdAsync(packageId);
            if (package == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
            }

            var existingPackage = await _unitOfWork.PackageRepository
                .GetByWhere(p => p.PackageName == request.PackageName && p.PackageId != packageId)
                .FirstOrDefaultAsync();
            if (existingPackage != null)
            {
                return new BusinessResult(Const.HTTP_STATUS_CONFLICT, "Tên gói đã tồn tại");
            }

            package.PackageName = request.PackageName;
            package.Price = request.Price;
            package.Duration = request.Duration;
            package.Description = request.Description;
            package.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.PackageRepository.UpdateAsync(package);
            await _unitOfWork.SaveChangesAsync();

            var response = package.Adapt<PackageResponse>();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_UPDATE_MSG, response);
        }

        public async Task<IBusinessResult> DeletePackage(int packageId)
        {
            var package = await _unitOfWork.PackageRepository.GetByIdAsync(packageId);
            if (package == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Gói không tồn tại");
            }

            await _unitOfWork.PackageRepository.DeleteAsync(package);
            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_DELETE_MSG);
        }

        public async Task<IBusinessResult> GetUserPackage(int pageIndex, int pageSize, string? status, string? search)
        {
            search = search?.ToLower() ?? string.Empty;

            var userPackages = await _unitOfWork.UserPackageRepository.GetPagedAsync(
                pageIndex,
                pageSize,
                x => (string.IsNullOrEmpty(status) || x.Status.ToLower() == status.ToLower()) &&
                      (string.IsNullOrEmpty(search) || x.User.FullName.ToLower().Contains(search)
                                                   || x.Package.PackageName.ToLower().Contains(search)),
                q => q.OrderByDescending(x => x.StartDate),
                i => i.Include(x => x.User).Include(x=>x.Package)
                );
            if (userPackages == null || !userPackages.Any())
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, Const.FAIL_READ_MSG);
            }
            foreach (var package in userPackages)
            {
                if (package.Status == "Active" && package.ExpiryDate <= DateTime.UtcNow)
                {
                    package.Status = "Inactive";
                    await _unitOfWork.UserPackageRepository.UpdateAsync(package);
                }
            }
            await _unitOfWork.SaveChangesAsync();
            var response = userPackages.Select(up => new UserPackageResponse
            {
                UserPackageId = up.UserPackageId,
                UserId = up.UserId,
                FullName = up.User?.FullName,
                PackageId = up.PackageId,
                PackageName = up.Package?.PackageName,
                StartDate = up.StartDate,
                ExpiryDate = up.ExpiryDate,
                Status = up.Status
            }).ToList();

            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, response);
        }

    }
}
