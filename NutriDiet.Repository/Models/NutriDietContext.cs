using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace NutriDiet.Repository.Models;

public partial class NutriDietContext : DbContext
{
    public NutriDietContext()
    {
    }

    public NutriDietContext(DbContextOptions<NutriDietContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AirecommendMealLog> AirecommendMealLogs { get; set; }

    public virtual DbSet<AirecommendMealPlan> AirecommendMealPlans { get; set; }

    public virtual DbSet<Allergy> Allergies { get; set; }

    public virtual DbSet<CuisineType> CuisineTypes { get; set; }

    public virtual DbSet<Disease> Diseases { get; set; }

    public virtual DbSet<Food> Foods { get; set; }

    public virtual DbSet<FoodSubstitution> FoodSubstitutions { get; set; }

    public virtual DbSet<GeneralHealthProfile> GeneralHealthProfiles { get; set; }

    public virtual DbSet<HealthcareIndicator> HealthcareIndicators { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<MealLog> MealLogs { get; set; }

    public virtual DbSet<MealLogDetail> MealLogDetails { get; set; }

    public virtual DbSet<MealPlan> MealPlans { get; set; }

    public virtual DbSet<MealPlanDetail> MealPlanDetails { get; set; }

    public virtual DbSet<MyFood> MyFoods { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Package> Packages { get; set; }

    public virtual DbSet<PersonalGoal> PersonalGoals { get; set; }

    public virtual DbSet<RecipeSuggestion> RecipeSuggestions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SystemConfiguration> SystemConfigurations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserFoodPreference> UserFoodPreferences { get; set; }

    public virtual DbSet<UserIngreDientPreference> UserIngreDientPreferences { get; set; }

    public virtual DbSet<UserPackage> UserPackages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AirecommendMealLog>(entity =>
        {
            entity.HasKey(e => e.AirecommendMealLogId).HasName("PK__AIRecomm__9CD3D5F87D1F5933");

            entity.ToTable("AIRecommendMealLog");

            entity.Property(e => e.AirecommendMealLogId).HasColumnName("AIRecommendMealLogID");
            entity.Property(e => e.AirecommendMealLogResponse).HasColumnName("AIRecommendMealLogResponse");
            entity.Property(e => e.Feedback).HasMaxLength(255);
            entity.Property(e => e.MealLogId).HasColumnName("MealLogID");
            entity.Property(e => e.RecommendedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RejectionReason).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.MealLog).WithMany(p => p.AirecommendMealLogs)
                .HasForeignKey(d => d.MealLogId)
                .HasConstraintName("FK__AIRecomme__MealL__7246E95D");

            entity.HasOne(d => d.User).WithMany(p => p.AirecommendMealLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__AIRecomme__UserI__733B0D96");
        });

        modelBuilder.Entity<AirecommendMealPlan>(entity =>
        {
            entity.HasKey(e => e.AirecommendMealPlanId).HasName("PK__AIRecomm__CF019A62E30FB1E6");

            entity.ToTable("AIRecommendMealPlan");

            entity.Property(e => e.AirecommendMealPlanId).HasColumnName("AIRecommendMealPlanID");
            entity.Property(e => e.AirecommendMealPlanResponse).HasColumnName("AIRecommendMealPlanResponse");
            entity.Property(e => e.Feedback).HasMaxLength(255);
            entity.Property(e => e.MealPlanId).HasColumnName("MealPlanID");
            entity.Property(e => e.RecommendedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RejectionReason).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.MealPlan).WithMany(p => p.AirecommendMealPlans)
                .HasForeignKey(d => d.MealPlanId)
                .HasConstraintName("FK__AIRecomme__MealP__597B3B93");

            entity.HasOne(d => d.User).WithMany(p => p.AirecommendMealPlans)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__AIRecomme__UserI__5A6F5FCC");
        });

        modelBuilder.Entity<Allergy>(entity =>
        {
            entity.HasKey(e => e.AllergyId).HasName("PK__Allergy__A49EBE62A21633E5");

            entity.ToTable("Allergy");

            entity.Property(e => e.AllergyId).HasColumnName("AllergyID");
            entity.Property(e => e.AllergyName).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Notes).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<CuisineType>(entity =>
        {
            entity.HasKey(e => e.CuisineId).HasName("PK__CuisineT__B1C3E7AB4933C9E2");

            entity.ToTable("CuisineType");

            entity.HasIndex(e => e.CuisineName, "UQ__CuisineT__2C77DCC8BDF0E775").IsUnique();

            entity.Property(e => e.CuisineId).HasColumnName("CuisineID");
            entity.Property(e => e.CuisineName).HasMaxLength(50);
        });

        modelBuilder.Entity<Disease>(entity =>
        {
            entity.HasKey(e => e.DiseaseId).HasName("PK__Disease__69B533A9AC9E4B72");

            entity.ToTable("Disease");

            entity.Property(e => e.DiseaseId).HasColumnName("DiseaseID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.DiseaseName).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Food>(entity =>
        {
            entity.HasKey(e => e.FoodId).HasName("PK__Food__856DB3CB47409237");

            entity.ToTable("Food");

            entity.HasIndex(e => e.FoodName, "UQ__Food__81B4FC25D7315BC0").IsUnique();

            entity.Property(e => e.FoodId).HasColumnName("FoodID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.FoodName).HasMaxLength(100);
            entity.Property(e => e.FoodType).HasMaxLength(100);
            entity.Property(e => e.MealType).HasMaxLength(100);
            entity.Property(e => e.ServingSize).HasMaxLength(50);

            entity.HasMany(d => d.Ingredients).WithMany(p => p.Foods)
                .UsingEntity<Dictionary<string, object>>(
                    "FoodIngredient",
                    r => r.HasOne<Ingredient>().WithMany()
                        .HasForeignKey("IngredientId")
                        .HasConstraintName("FK__FoodIngre__Ingre__25FB978D"),
                    l => l.HasOne<Food>().WithMany()
                        .HasForeignKey("FoodId")
                        .HasConstraintName("FK__FoodIngre__FoodI__25077354"),
                    j =>
                    {
                        j.HasKey("FoodId", "IngredientId").HasName("PK__FoodIngr__3E8758EC14F250E2");
                        j.ToTable("FoodIngredient");
                        j.IndexerProperty<int>("FoodId").HasColumnName("FoodID");
                        j.IndexerProperty<int>("IngredientId").HasColumnName("IngredientID");
                    });
        });

        modelBuilder.Entity<FoodSubstitution>(entity =>
        {
            entity.HasKey(e => e.SubstitutionId).HasName("PK__FoodSubs__95BE7DE47CA85485");

            entity.ToTable("FoodSubstitution");

            entity.Property(e => e.SubstitutionId).HasColumnName("SubstitutionID");
            entity.Property(e => e.OriginalFoodId).HasColumnName("OriginalFoodID");
            entity.Property(e => e.Reason).HasMaxLength(255);
            entity.Property(e => e.SubstituteFoodId).HasColumnName("SubstituteFoodID");

            entity.HasOne(d => d.OriginalFood).WithMany(p => p.FoodSubstitutionOriginalFoods)
                .HasForeignKey(d => d.OriginalFoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FoodSubst__Origi__4944D3CA");

            entity.HasOne(d => d.SubstituteFood).WithMany(p => p.FoodSubstitutionSubstituteFoods)
                .HasForeignKey(d => d.SubstituteFoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FoodSubst__Subst__4A38F803");
        });

        modelBuilder.Entity<GeneralHealthProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK__GeneralH__290C888475DF6AAF");

            entity.ToTable("GeneralHealthProfile");

            entity.Property(e => e.ProfileId).HasColumnName("ProfileID");
            entity.Property(e => e.ActivityLevel).HasMaxLength(50);
            entity.Property(e => e.Aisuggestion)
                .HasMaxLength(255)
                .HasColumnName("AISuggestion");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.GeneralHealthProfiles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__GeneralHe__UserI__0682EC34");
        });

        modelBuilder.Entity<HealthcareIndicator>(entity =>
        {
            entity.HasKey(e => e.HealthcareIndicatorId).HasName("PK__Healthca__B62181044EF43BFF");

            entity.ToTable("HealthcareIndicator");

            entity.Property(e => e.HealthcareIndicatorId).HasColumnName("HealthcareIndicatorID");
            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.Aisuggestion)
                .HasMaxLength(255)
                .HasColumnName("AISuggestion");
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.HealthcareIndicators)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Healthcar__UserI__54B68676");
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.IngredientId).HasName("PK__Ingredie__BEAEB27A4BC41058");

            entity.ToTable("Ingredient");

            entity.HasIndex(e => e.IngredientName, "UQ__Ingredie__A1B2F1CCBF67281D").IsUnique();

            entity.Property(e => e.IngredientId).HasColumnName("IngredientID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IngredientName).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<MealLog>(entity =>
        {
            entity.HasKey(e => e.MealLogId).HasName("PK__MealLog__0ED21C52A2DEC0BF");

            entity.ToTable("MealLog");

            entity.Property(e => e.MealLogId).HasColumnName("MealLogID");
            entity.Property(e => e.LogDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TotalCarbs).HasDefaultValue(0.0);
            entity.Property(e => e.TotalFat).HasDefaultValue(0.0);
            entity.Property(e => e.TotalProtein).HasDefaultValue(0.0);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.MealLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__MealLog__UserID__64ECEE3F");
        });

        modelBuilder.Entity<MealLogDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__MealLogD__135C314D5E844D54");

            entity.ToTable("MealLogDetail");

            entity.Property(e => e.DetailId).HasColumnName("DetailID");
            entity.Property(e => e.FoodId).HasColumnName("FoodID");
            entity.Property(e => e.MealLogId).HasColumnName("MealLogID");
            entity.Property(e => e.MealType).HasMaxLength(50);
            entity.Property(e => e.ServingSize).HasMaxLength(50);

            entity.HasOne(d => d.Food).WithMany(p => p.MealLogDetails)
                .HasForeignKey(d => d.FoodId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__MealLogDe__FoodI__6D823440");

            entity.HasOne(d => d.MealLog).WithMany(p => p.MealLogDetails)
                .HasForeignKey(d => d.MealLogId)
                .HasConstraintName("FK__MealLogDe__MealL__6C8E1007");
        });

        modelBuilder.Entity<MealPlan>(entity =>
        {
            entity.HasKey(e => e.MealPlanId).HasName("PK__MealPlan__0620DB56B11793DF");

            entity.ToTable("MealPlan");

            entity.Property(e => e.MealPlanId).HasColumnName("MealPlanID");
            entity.Property(e => e.Aiwarning).HasColumnName("AIWarning");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.HealthGoal).HasMaxLength(50);
            entity.Property(e => e.PlanName).HasMaxLength(100);
            entity.Property(e => e.StartAt).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Inactive");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.MealPlans)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__MealPlan__UserID__3A02903A");
        });

        modelBuilder.Entity<MealPlanDetail>(entity =>
        {
            entity.HasKey(e => e.MealPlanDetailId).HasName("PK__MealPlan__37DC012B77F67FD6");

            entity.ToTable("MealPlanDetail");

            entity.Property(e => e.MealPlanDetailId).HasColumnName("MealPlanDetailID");
            entity.Property(e => e.FoodId).HasColumnName("FoodID");
            entity.Property(e => e.FoodName).HasMaxLength(255);
            entity.Property(e => e.MealPlanId).HasColumnName("MealPlanID");
            entity.Property(e => e.MealType).HasMaxLength(50);
            entity.Property(e => e.TotalCalories).HasDefaultValue(0.0);
            entity.Property(e => e.TotalCarbs).HasDefaultValue(0.0);
            entity.Property(e => e.TotalFat).HasDefaultValue(0.0);
            entity.Property(e => e.TotalProtein).HasDefaultValue(0.0);

            entity.HasOne(d => d.Food).WithMany(p => p.MealPlanDetails)
                .HasForeignKey(d => d.FoodId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__MealPlanD__FoodI__4668671F");

            entity.HasOne(d => d.MealPlan).WithMany(p => p.MealPlanDetails)
                .HasForeignKey(d => d.MealPlanId)
                .HasConstraintName("FK__MealPlanD__MealP__457442E6");
        });

        modelBuilder.Entity<MyFood>(entity =>
        {
            entity.HasKey(e => e.MyFoodId).HasName("PK__MyFood__4A2439357BCBE9A1");

            entity.ToTable("MyFood");

            entity.Property(e => e.MyFoodId).HasColumnName("MyFoodID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FoodName).HasMaxLength(100);
            entity.Property(e => e.ServingSize).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.MyFoods)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__MyFood__UserID__0EE3280B");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E12902121DA");

            entity.ToTable("Notification");

            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__4EFDAD20");
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("PK__Package__322035EC15F46844");

            entity.ToTable("Package");

            entity.HasIndex(e => e.PackageName, "UQ__Package__73856F7AD4363AB3").IsUnique();

            entity.Property(e => e.PackageId).HasColumnName("PackageID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.PackageName).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<PersonalGoal>(entity =>
        {
            entity.HasKey(e => e.GoalId).HasName("PK__Personal__8A4FFF310F853375");

            entity.ToTable("PersonalGoal");

            entity.Property(e => e.GoalId).HasColumnName("GoalID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.GoalDescription).HasMaxLength(255);
            entity.Property(e => e.GoalType).HasMaxLength(50);
            entity.Property(e => e.ProgressPercentage).HasDefaultValue(0.0);
            entity.Property(e => e.ProgressRate).HasDefaultValue(0);
            entity.Property(e => e.StartDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
            entity.Property(e => e.TargetDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.PersonalGoals)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__PersonalG__UserI__7EACC042");
        });

        modelBuilder.Entity<RecipeSuggestion>(entity =>
        {
            entity.HasKey(e => e.RecipeId).HasName("PK__RecipeSu__FDD988D0A5C73EFF");

            entity.ToTable("RecipeSuggestion");

            entity.Property(e => e.RecipeId).HasColumnName("RecipeID");
            entity.Property(e => e.Aimodel)
                .HasMaxLength(255)
                .HasColumnName("AIModel");
            entity.Property(e => e.Airequest).HasColumnName("AIRequest");
            entity.Property(e => e.Airesponse).HasColumnName("AIResponse");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CuisineId).HasColumnName("CuisineID");
            entity.Property(e => e.FoodId).HasColumnName("FoodID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Cuisine).WithMany(p => p.RecipeSuggestions)
                .HasForeignKey(d => d.CuisineId)
                .HasConstraintName("FK__RecipeSug__Cuisi__2BB470E3");

            entity.HasOne(d => d.Food).WithMany(p => p.RecipeSuggestions)
                .HasForeignKey(d => d.FoodId)
                .HasConstraintName("FK__RecipeSug__FoodI__29CC2871");

            entity.HasOne(d => d.User).WithMany(p => p.RecipeSuggestions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__RecipeSug__UserI__2AC04CAA");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3A7CF196AE");

            entity.ToTable("Role");

            entity.HasIndex(e => e.RoleName, "UQ__Role__8A2B61607AB7FAE3").IsUnique();

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<SystemConfiguration>(entity =>
        {
            entity.HasKey(e => e.ConfigId).HasName("PK__SystemCo__C3BC333C0FF3D389");

            entity.ToTable("SystemConfiguration");

            entity.HasIndex(e => e.Name, "UQ__SystemCo__737584F676459B04").IsUnique();

            entity.Property(e => e.ConfigId).HasColumnName("ConfigID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.EffectedDateFrom)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EffectedDateTo).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCACFB68DF32");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D10534C4869ABF").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FcmToken)
                .HasMaxLength(255)
                .HasColumnName("fcmToken");
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Location).HasMaxLength(20);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.RefreshTokenExpiryTime).HasColumnType("datetime");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User__RoleID__727BF387");

            entity.HasMany(d => d.Allergies).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserAllergy",
                    r => r.HasOne<Allergy>().WithMany()
                        .HasForeignKey("AllergyId")
                        .HasConstraintName("FK__UserAller__Aller__0E240DFC"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__UserAller__UserI__0D2FE9C3"),
                    j =>
                    {
                        j.HasKey("UserId", "AllergyId").HasName("PK__UserAlle__2DC1274AC9B61153");
                        j.ToTable("UserAllergy");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                        j.IndexerProperty<int>("AllergyId").HasColumnName("AllergyID");
                    });

            entity.HasMany(d => d.Diseases).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserDisease",
                    r => r.HasOne<Disease>().WithMany()
                        .HasForeignKey("DiseaseId")
                        .HasConstraintName("FK__UserDisea__Disea__064DE20A"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__UserDisea__UserI__0559BDD1"),
                    j =>
                    {
                        j.HasKey("UserId", "DiseaseId").HasName("PK__UserDise__91139F9682313865");
                        j.ToTable("UserDisease");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                        j.IndexerProperty<int>("DiseaseId").HasColumnName("DiseaseID");
                    });
        });

        modelBuilder.Entity<UserFoodPreference>(entity =>
        {
            entity.HasKey(e => e.UserFoodPreferenceId).HasName("PK__UserFood__997D6AD7AE6EF6AB");

            entity.ToTable("UserFoodPreference");

            entity.Property(e => e.UserFoodPreferenceId).HasColumnName("UserFoodPreferenceID");
            entity.Property(e => e.FoodId).HasColumnName("FoodID");
            entity.Property(e => e.Preference).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Food).WithMany(p => p.UserFoodPreferences)
                .HasForeignKey(d => d.FoodId)
                .HasConstraintName("FK__UserFoodP__FoodI__2F8501C7");

            entity.HasOne(d => d.User).WithMany(p => p.UserFoodPreferences)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserFoodP__UserI__2E90DD8E");
        });

        modelBuilder.Entity<UserIngreDientPreference>(entity =>
        {
            entity.HasKey(e => e.UserIngreDientPreferenceId).HasName("PK__UserIngr__27A229AF941912BF");

            entity.ToTable("UserIngreDientPreference");

            entity.Property(e => e.UserIngreDientPreferenceId).HasColumnName("UserIngreDientPreferenceID");
            entity.Property(e => e.IngredientId).HasColumnName("IngredientID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.UserIngreDientPreferences)
                .HasForeignKey(d => d.IngredientId)
                .HasConstraintName("FK__UserIngre__Ingre__335592AB");

            entity.HasOne(d => d.User).WithMany(p => p.UserIngreDientPreferences)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserIngre__UserI__32616E72");
        });

        modelBuilder.Entity<UserPackage>(entity =>
        {
            entity.HasKey(e => e.UserPackageId).HasName("PK__UserPack__AE9B91FAEDAD0ED3");

            entity.ToTable("UserPackage");

            entity.Property(e => e.UserPackageId).HasColumnName("UserPackageID");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.PackageId).HasColumnName("PackageID");
            entity.Property(e => e.StartDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Package).WithMany(p => p.UserPackages)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("FK__UserPacka__Packa__7EE1CA6C");

            entity.HasOne(d => d.User).WithMany(p => p.UserPackages)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserPacka__UserI__7DEDA633");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
