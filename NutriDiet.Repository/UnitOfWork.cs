using LogiConnect.Repository.Interface;
using LogiConnect.Repository.Models;
using LogiConnect.Repository.Repositories;
using System;
using System.Threading.Tasks;

namespace LogiConnect.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly NutriDietContext _context;
        private IUserRepository _userRepository;

        public UnitOfWork(NutriDietContext context)
        {
            _context = context;
        }

        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);

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
