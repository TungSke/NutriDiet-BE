using LogiConnect.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace LogiConnect.API
{
    public class ServiceRegister
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            DotNetEnv.Env.Load();
            
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            services.AddDbContext<NutriDietContext>(options =>
            {
                options.UseSqlServer(connectionString);
                options.EnableSensitiveDataLogging();
            });

            //services.AddAuthorizeService(configuration);
            //AddMapper();
            //AddEnum(services);
            //AddCorsToThisWeb(services);

            //services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services.AddScoped<TokenHandlerService>();

            //services.AddScoped<IAccountRepository, AccountRepository>();
            //services.AddScoped<IAccountService, AccountService>();

            //services.AddScoped<IProductRepository, ProductRepository>();
            //services.AddScoped<IProductService, ProductService>();

            //services.AddScoped<IStockRepository, StockRepository>();
            //services.AddScoped<IStockService, StockService>();

            //services.AddScoped<IComponentRepository, ComponentRepository>();
            //services.AddScoped<IComponentService, ComponentService>();

            //services.AddScoped<IOrderRepository, OrderRepository>();
            //services.AddScoped<IOrderService, OrderService>();

            //services.AddScoped<IGoogleService, GoogleService>();

            //services.AddScoped<IAdminService, AdminService>();
        }
    }
}
