using Mapster;
using Microsoft.EntityFrameworkCore;
using NutriDiet.Common;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Request;
using NutriDiet.Service.ModelDTOs.Response;

namespace NutriDiet.Service.Services
{
    public class CuisineTypeService : ICuisineTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CuisineTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IBusinessResult> GetAllCuisineTypes()
        {
            var list = await _unitOfWork.CuisineRepository.GetAll().ToListAsync();
            var response = list.Adapt<IEnumerable<CuisineTypeResponse>>();
            return new BusinessResult(Const.HTTP_STATUS_OK,Const.SUCCESS_READ_MSG, response);
        }

        public async Task<IBusinessResult> GetCuisineTypeById(int id)
        {
            var cuisine = await _unitOfWork.CuisineRepository.GetByIdAsync(id);
            var list = cuisine.Adapt<CuisineTypeResponse>();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_READ_MSG, list);
        }

        public async Task<IBusinessResult> CreateCuisineType(CuisineTypeRequest cuisineType)
        {
            var cuisine = cuisineType.Adapt<CuisineType>();
            await _unitOfWork.CuisineRepository.AddAsync(cuisine);
            await _unitOfWork.SaveChangesAsync();
            return new BusinessResult(Const.HTTP_STATUS_CREATED, Const.SUCCESS_CREATE_MSG, cuisine.Adapt<CuisineTypeResponse>());
        }

        public async Task<IBusinessResult> DeleteCuisineType(int id)
        {
            var cuisine = await _unitOfWork.CuisineRepository.GetByIdAsync(id);
            if (cuisine == null)
            {
                return new BusinessResult(Const.HTTP_STATUS_NOT_FOUND, "Cusine not found");
            }
            await _unitOfWork.CuisineRepository.DeleteAsync(cuisine);
            await _unitOfWork.SaveChangesAsync();
            return new BusinessResult(Const.HTTP_STATUS_OK, Const.SUCCESS_DELETE_MSG);
        }
    }
}
