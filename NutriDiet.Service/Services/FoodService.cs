using Mapster;
using Microsoft.EntityFrameworkCore;
using NutriDiet.Common;
using NutriDiet.Common.BusinessResult;
using NutriDiet.Repository.Interface;
using NutriDiet.Service.Interface;
using NutriDiet.Service.ModelDTOs.Response;

namespace NutriDiet.Service.Services
{
    public class FoodService : IFoodService
    {
        private readonly IUnitOfWork _unitOfWork;
        public FoodService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IBusinessResult> GetAllFood(int pageindex, int pagesize, string search)
        {
            var foods = await _unitOfWork.FoodRepository.GetPagedAsync(pageindex,pagesize, x => x.FoodName.ToLower().Contains(search.ToLower()));
            if (foods == null)
            {
                return new BusinessResult(Const.FAILURE, Const.FAIL_READ_MSG);
            }
            var resposne = foods.Adapt<List<FoodResponse>>();
            return new BusinessResult(Const.SUCCESS, Const.SUCCESS_READ_MSG, resposne);
        }
    }
}
