using System.Threading.Tasks;

namespace NutriDiet.Repository.Interface
{
    public interface IUnitOfWork
    {
        Task BeginTransaction();
        Task CommitTransaction();
        Task RollbackTransaction();
        Task SaveChangesAsync();

        IUserRepository UserRepository { get; }
    }
}