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

        public async Task<CuisineTypeResponse> GetCuisineTypeById(int id)
        {
            var cuisine = await _unitOfWork.CuisineRepository.GetByIdAsync(id);
            return cuisine.Adapt<CuisineTypeResponse>();
        }

        public async Task<CuisineTypeResponse> CreateCuisineType(CuisineTypeRequest cuisineType)
        {
            var cuisine = cuisineType.Adapt<CuisineType>();
            await _unitOfWork.CuisineRepository.AddAsync(cuisine);
            await _unitOfWork.SaveChangesAsync();
            return cuisine.Adapt<CuisineTypeResponse>();
        }
    }
}
