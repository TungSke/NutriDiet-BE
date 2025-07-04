﻿using NutriDiet.Repository;
using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using NutriDiet.Service.Helpers;
using NutriDiet.Service.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using NutriDiet.Service.Interface;
using NutriDiet.Service.Services;
using Mapster;
using NutriDiet.Service.ModelDTOs.Response;
using NutriDiet.Service.BackgroundServices;

namespace NutriDiet.API.Extensions
{
    public static class ServiceRegister
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            DotNetEnv.Env.Load("../.env");

            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            services.AddDbContext<NutriDietContext>(options =>
            {
                options.UseSqlServer(connectionString);
                options.EnableSensitiveDataLogging();
            });

            services.AddAuthorizeService(configuration);
            AddCorsToThisWeb(services);
            AddEnum(services);
            AddKebab(services);
            AddMapster();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<GoogleService>();
            services.AddScoped<TokenHandlerHelper>();
            services.AddScoped<CloudinaryHelper>();
            services.AddScoped<AIGeneratorService>();
            services.AddScoped<FirebaseService>();
            services.AddScoped<BarcodeHelper>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFoodService, FoodService>();

            services.AddScoped<IGeneralHealthProfileService, GeneralHealthProfileService>();

            services.AddScoped<IMealPlanService, MealPlanService>();
            services.AddScoped<IMealPlanDetailService, MealPlanDetailService>();

            services.AddScoped<IAllergyService, AllergyService>();
            services.AddScoped<IDiseaseService, DiseaseService>();

            services.AddScoped<IPersonalGoalService, PersonalGoalService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IMealLogService, MealLogService>();
            services.AddScoped<ICuisineTypeService, CuisineTypeService>();
            services.AddScoped<IIngreDientService, IngreDientSevice>();        
            services.AddScoped<IPackageService, PackageService>();
            services.AddScoped<ISystemConfigurationService, SystemConfigationService>();
            services.AddHostedService<NotificationBackgroundService>();
        }

        public static IServiceCollection AddAuthorizeService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                };
            });

            //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            //{
            //    options.LoginPath = "/Account/Login";
            //    options.AccessDeniedPath = "/Account/AccessDenied";
            //});


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "NutriDiet_API",
                    Version = "v1",
                    Description = "API for managing NutriDiet app",                
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter your JWT token in this field",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

            });

            return services;
        }

        private static void AddCorsToThisWeb(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });
        }

        private static void AddEnum(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        }

        private static void AddKebab(IServiceCollection services)
        {
            services.AddControllers(opts =>
                    opts.Conventions.Add(new RouteTokenTransformerConvention(new ToKebabParameterTransformer())))

                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    });
        }

        private static void AddMapster()
        {
            TypeAdapterConfig<User, UserResponse>
                .NewConfig()
                .Map(dest => dest.Role, src => src.Role.RoleName)
                .Map(dest => dest.UserPackages, src => src.UserPackages);

            TypeAdapterConfig.GlobalSettings.NewConfig<UserPackage, UserPackagesResponse>()
                .Map(dest => dest.PackageName, src => src.Package.PackageName);
        }
    }
}