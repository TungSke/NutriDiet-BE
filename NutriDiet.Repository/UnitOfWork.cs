using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using NutriDiet.Repository.Repositories;
using System;
using System.Threading.Tasks;

namespace NutriDiet.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly NutriDietContext _context;
        private IUserRepository _userRepository;
        private IFoodRepository _foodRepository;
        private IHealthProfileRepository _healthProfileRepository;
        private IMealPlanRepository _mealPlanRepository;
        private IMealPlanDetailRepository _mealPlanDetailRepository;

        public UnitOfWork(NutriDietContext context)
        {
            _context = context;
        }

        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);

        public IFoodRepository FoodRepository => _foodRepository ??= new FoodRepository(_context);
        public IHealthProfileRepository HealthProfileRepository => _healthProfileRepository ??= new HealthProfileRepository(_context);
        public IMealPlanRepository MealPlanRepository => _mealPlanRepository ??= new MealPlanRepository(_context);
        public IMealPlanDetailRepository MealPlanDetailRepository => _mealPlanDetailRepository ??= new MealPlanDetailRepository(_context);

        public async Task BeginTransaction()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransaction()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransaction()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
