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
        private IFoodDetailRepository _foodDetailRepository;

        public UnitOfWork()
        {
            _context ??= new NutriDietContext();
        }

        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);

        public IFoodRepository FoodRepository => _foodRepository ??= new FoodRepository(_context);

        public IFoodDetailRepository FoodDetailRepository => _foodDetailRepository ??= new FoodDetailRepository(_context);

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
