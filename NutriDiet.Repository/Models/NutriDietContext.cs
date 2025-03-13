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

    public virtual DbSet<FoodIngredient> FoodIngredients { get; set; }

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

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserFoodPreference> UserFoodPreferences { get; set; }

    public virtual DbSet<UserIngreDientPreference> UserIngreDientPreferences { get; set; }

    public virtual DbSet<UserPackage> UserPackages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AirecommendMealLog>(entity =>
        {
            entity.HasKey(e => e.AirecommendMealLogId).HasName("PK__AIRecomm__9CD3D5F8B63836A3");

            entity.ToTable("AIRecommendMealLog");

            entity.Property(e => e.AirecommendMealLogId).HasColumnName("AIRecommendMealLogID");
            entity.Property(e => e.AirecommendMealPlanResponse).HasColumnName("AIRecommendMealPlanResponse");
            entity.Property(e => e.MealLogId).HasColumnName("MealLogID");
            entity.Property(e => e.RecommendedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RejectionReason).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.MealLog).WithMany(p => p.AirecommendMealLogs)
                .HasForeignKey(d => d.MealLogId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__AIRecomme__MealL__3F115E1A");
        });

        modelBuilder.Entity<AirecommendMealPlan>(entity =>
        {
            entity.HasKey(e => e.AirecommendMealPlanId).HasName("PK__AIRecomm__CF019A627B5E35DB");

            entity.ToTable("AIRecommendMealPlan");

            entity.Property(e => e.AirecommendMealPlanId).HasColumnName("AIRecommendMealPlanID");
            entity.Property(e => e.AirecommendMealPlanResponse).HasColumnName("AIRecommendMealPlanResponse");
            entity.Property(e => e.MealPlanId).HasColumnName("MealPlanID");
            entity.Property(e => e.RecommendedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RejectionReason).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.MealPlan).WithMany(p => p.AirecommendMealPlans)
                .HasForeignKey(d => d.MealPlanId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__AIRecomme__MealP__2739D489");
        });

        modelBuilder.Entity<Allergy>(entity =>
        {
            entity.HasKey(e => e.AllergyId).HasName("PK__Allergy__A49EBE62B08FCD46");

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
            entity.HasKey(e => e.CuisineId).HasName("PK__CuisineT__B1C3E7AB45A4D8D4");

            entity.ToTable("CuisineType");

            entity.HasIndex(e => e.CuisineName, "UQ__CuisineT__2C77DCC8395104E1").IsUnique();

            entity.Property(e => e.CuisineId).HasColumnName("CuisineID");
            entity.Property(e => e.CuisineName).HasMaxLength(50);
        });

        modelBuilder.Entity<Disease>(entity =>
        {
            entity.HasKey(e => e.DiseaseId).HasName("PK__Disease__69B533A9608C106E");

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
            entity.HasKey(e => e.FoodId).HasName("PK__Food__856DB3CBFD430CFE");

            entity.ToTable("Food");

            entity.HasIndex(e => e.FoodName, "UQ__Food__81B4FC25BDFA1837").IsUnique();

            entity.Property(e => e.FoodId).HasColumnName("FoodID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.FoodName).HasMaxLength(100);
            entity.Property(e => e.FoodType).HasMaxLength(100);
            entity.Property(e => e.MealType).HasMaxLength(100);
            entity.Property(e => e.ServingSize).HasMaxLength(50);
        });

        modelBuilder.Entity<FoodIngredient>(entity =>
        {
            entity.HasKey(e => new { e.FoodId, e.IngredientId }).HasName("PK__FoodIngr__3E8758ECCC101DDE");

            entity.ToTable("FoodIngredient");

            entity.Property(e => e.FoodId).HasColumnName("FoodID");
            entity.Property(e => e.IngredientId).HasColumnName("IngredientID");
            entity.Property(e => e.Unit).HasMaxLength(50);

            entity.HasOne(d => d.Food).WithMany(p => p.FoodIngredients)
                .HasForeignKey(d => d.FoodId)
                .HasConstraintName("FK__FoodIngre__FoodI__72C60C4A");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.FoodIngredients)
                .HasForeignKey(d => d.IngredientId)
                .HasConstraintName("FK__FoodIngre__Ingre__73BA3083");
        });

        modelBuilder.Entity<FoodSubstitution>(entity =>
        {
            entity.HasKey(e => e.SubstitutionId).HasName("PK__FoodSubs__95BE7DE42D07EB1E");

            entity.ToTable("FoodSubstitution");

            entity.Property(e => e.SubstitutionId).HasColumnName("SubstitutionID");
            entity.Property(e => e.OriginalFoodId).HasColumnName("OriginalFoodID");
            entity.Property(e => e.Reason).HasMaxLength(255);
            entity.Property(e => e.SubstituteFoodId).HasColumnName("SubstituteFoodID");

            entity.HasOne(d => d.OriginalFood).WithMany(p => p.FoodSubstitutionOriginalFoods)
                .HasForeignKey(d => d.OriginalFoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FoodSubst__Origi__17036CC0");

            entity.HasOne(d => d.SubstituteFood).WithMany(p => p.FoodSubstitutionSubstituteFoods)
                .HasForeignKey(d => d.SubstituteFoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FoodSubst__Subst__17F790F9");
        });

        modelBuilder.Entity<GeneralHealthProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK__GeneralH__290C88846C265D27");

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
                .HasConstraintName("FK__GeneralHe__UserI__534D60F1");
        });

        modelBuilder.Entity<HealthcareIndicator>(entity =>
        {
            entity.HasKey(e => e.HealthcareIndicatorId).HasName("PK__Healthca__B62181045179C60C");

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
                .HasConstraintName("FK__Healthcar__UserI__22751F6C");
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.IngredientId).HasName("PK__Ingredie__BEAEB27A06B2F1D0");

            entity.ToTable("Ingredient");

            entity.HasIndex(e => e.IngredientName, "UQ__Ingredie__A1B2F1CC6644EE13").IsUnique();

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
            entity.HasKey(e => e.MealLogId).HasName("PK__MealLog__0ED21C5254D0ABF4");

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
                .HasConstraintName("FK__MealLog__UserID__31B762FC");
        });

        modelBuilder.Entity<MealLogDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__MealLogD__135C314DB5A0579A");

            entity.ToTable("MealLogDetail");

            entity.Property(e => e.DetailId).HasColumnName("DetailID");
            entity.Property(e => e.FoodId).HasColumnName("FoodID");
            entity.Property(e => e.MealLogId).HasColumnName("MealLogID");
            entity.Property(e => e.MealType).HasMaxLength(50);
            entity.Property(e => e.ServingSize).HasMaxLength(50);

            entity.HasOne(d => d.Food).WithMany(p => p.MealLogDetails)
                .HasForeignKey(d => d.FoodId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__MealLogDe__FoodI__3A4CA8FD");

            entity.HasOne(d => d.MealLog).WithMany(p => p.MealLogDetails)
                .HasForeignKey(d => d.MealLogId)
                .HasConstraintName("FK__MealLogDe__MealL__395884C4");
        });

        modelBuilder.Entity<MealPlan>(entity =>
        {
            entity.HasKey(e => e.MealPlanId).HasName("PK__MealPlan__0620DB56D89C6DF9");

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
                .HasConstraintName("FK__MealPlan__UserID__07C12930");
        });

        modelBuilder.Entity<MealPlanDetail>(entity =>
        {
            entity.HasKey(e => e.MealPlanDetailId).HasName("PK__MealPlan__37DC012B543754F7");

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
                .HasConstraintName("FK__MealPlanD__FoodI__14270015");

            entity.HasOne(d => d.MealPlan).WithMany(p => p.MealPlanDetails)
                .HasForeignKey(d => d.MealPlanId)
                .HasConstraintName("FK__MealPlanD__MealP__1332DBDC");
        });

        modelBuilder.Entity<MyFood>(entity =>
        {
            entity.HasKey(e => e.MyFoodId).HasName("PK__MyFood__4A243935A6F2194E");

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
                .HasConstraintName("FK__MyFood__UserID__5AB9788F");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E12C26AC95C");

            entity.ToTable("Notification");

            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__1CBC4616");
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("PK__Package__322035ECD76DE1CA");

            entity.ToTable("Package");

            entity.HasIndex(e => e.PackageName, "UQ__Package__73856F7A41482B6B").IsUnique();

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
            entity.HasKey(e => e.GoalId).HasName("PK__Personal__8A4FFF3147DD63FC");

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
                .HasConstraintName("FK__PersonalG__UserI__4A8310C6");
        });

        modelBuilder.Entity<RecipeSuggestion>(entity =>
        {
            entity.HasKey(e => e.RecipeId).HasName("PK__RecipeSu__FDD988D07415E51A");

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
                .HasConstraintName("FK__RecipeSug__Cuisi__797309D9");

            entity.HasOne(d => d.Food).WithMany(p => p.RecipeSuggestions)
                .HasForeignKey(d => d.FoodId)
                .HasConstraintName("FK__RecipeSug__FoodI__778AC167");

            entity.HasOne(d => d.User).WithMany(p => p.RecipeSuggestions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__RecipeSug__UserI__787EE5A0");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3AD3AC5A7A");

            entity.ToTable("Role");

            entity.HasIndex(e => e.RoleName, "UQ__Role__8A2B6160CB4DAC2D").IsUnique();

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCAC2E3114C6");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D10534F442D502").IsUnique();

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
                .HasConstraintName("FK__User__RoleID__3F466844");

            entity.HasMany(d => d.Allergies).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserAllergy",
                    r => r.HasOne<Allergy>().WithMany()
                        .HasForeignKey("AllergyId")
                        .HasConstraintName("FK__UserAller__Aller__5AEE82B9"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__UserAller__UserI__59FA5E80"),
                    j =>
                    {
                        j.HasKey("UserId", "AllergyId").HasName("PK__UserAlle__2DC1274AB01D8149");
                        j.ToTable("UserAllergy");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                        j.IndexerProperty<int>("AllergyId").HasColumnName("AllergyID");
                    });

            entity.HasMany(d => d.Diseases).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserDisease",
                    r => r.HasOne<Disease>().WithMany()
                        .HasForeignKey("DiseaseId")
                        .HasConstraintName("FK__UserDisea__Disea__5224328E"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__UserDisea__UserI__51300E55"),
                    j =>
                    {
                        j.HasKey("UserId", "DiseaseId").HasName("PK__UserDise__91139F9673A23AB5");
                        j.ToTable("UserDisease");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                        j.IndexerProperty<int>("DiseaseId").HasColumnName("DiseaseID");
                    });
        });

        modelBuilder.Entity<UserFoodPreference>(entity =>
        {
            entity.HasKey(e => e.UserFoodPreferenceId).HasName("PK__UserFood__997D6AD786CF30C8");

            entity.ToTable("UserFoodPreference");

            entity.Property(e => e.UserFoodPreferenceId).HasColumnName("UserFoodPreferenceID");
            entity.Property(e => e.FoodId).HasColumnName("FoodID");
            entity.Property(e => e.Preference).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Food).WithMany(p => p.UserFoodPreferences)
                .HasForeignKey(d => d.FoodId)
                .HasConstraintName("FK__UserFoodP__FoodI__7D439ABD");

            entity.HasOne(d => d.User).WithMany(p => p.UserFoodPreferences)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserFoodP__UserI__7C4F7684");
        });

        modelBuilder.Entity<UserIngreDientPreference>(entity =>
        {
            entity.HasKey(e => e.UserIngreDientPreferenceId).HasName("PK__UserIngr__27A229AFC938752E");

            entity.ToTable("UserIngreDientPreference");

            entity.Property(e => e.UserIngreDientPreferenceId).HasColumnName("UserIngreDientPreferenceID");
            entity.Property(e => e.IngredientId).HasColumnName("IngredientID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.UserIngreDientPreferences)
                .HasForeignKey(d => d.IngredientId)
                .HasConstraintName("FK__UserIngre__Ingre__01142BA1");

            entity.HasOne(d => d.User).WithMany(p => p.UserIngreDientPreferences)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserIngre__UserI__00200768");
        });

        modelBuilder.Entity<UserPackage>(entity =>
        {
            entity.HasKey(e => e.UserPackageId).HasName("PK__UserPack__AE9B91FAEA961D6B");

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
                .HasConstraintName("FK__UserPacka__Packa__4BAC3F29");

            entity.HasOne(d => d.User).WithMany(p => p.UserPackages)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserPacka__UserI__4AB81AF0");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
