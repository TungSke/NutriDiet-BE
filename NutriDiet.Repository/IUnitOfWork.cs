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
        IFoodRepository FoodRepository { get; }

        IHealthProfileRepository HealthProfileRepository { get; }
        IMealPlanRepository MealPlanRepository { get; }
        IMealPlanDetailRepository MealPlanDetailRepository { get; }
        IIngredientRepository IngredientRepository { get; }
        IAllergyRepository AllergyRepository { get; }
    }
}