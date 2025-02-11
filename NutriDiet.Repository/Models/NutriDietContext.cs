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

    public virtual DbSet<Airecommendation> Airecommendations { get; set; }

    public virtual DbSet<Allergy> Allergies { get; set; }

    public virtual DbSet<Disease> Diseases { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<FeedbackReply> FeedbackReplies { get; set; }

    public virtual DbSet<Food> Foods { get; set; }

    public virtual DbSet<FoodSubstitution> FoodSubstitutions { get; set; }

    public virtual DbSet<HealthProfile> HealthProfiles { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<MealLog> MealLogs { get; set; }

    public virtual DbSet<MealLogDetail> MealLogDetails { get; set; }

    public virtual DbSet<MealPlan> MealPlans { get; set; }

    public virtual DbSet<MealPlanDetail> MealPlanDetails { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Package> Packages { get; set; }

    public virtual DbSet<PersonalGoal> PersonalGoals { get; set; }

    public virtual DbSet<RecipeSuggestion> RecipeSuggestions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserEatingHabit> UserEatingHabits { get; set; }

    public virtual DbSet<UserFoodPreference> UserFoodPreferences { get; set; }

    public virtual DbSet<UserPackage> UserPackages { get; set; }

    public virtual DbSet<UserParameter> UserParameters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Airecommendation>(entity =>
        {
            entity.HasKey(e => e.RecommendationId).HasName("PK__AIRecomm__AA15BEC4186684FB");

            entity.ToTable("AIRecommendation");

            entity.Property(e => e.RecommendationId).HasColumnName("RecommendationID");
            entity.Property(e => e.IsAccepted).HasDefaultValue(false);
            entity.Property(e => e.RecommendedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Airecommendations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__AIRecomme__UserI__2DE6D218");
        });

        modelBuilder.Entity<Allergy>(entity =>
        {
            entity.HasKey(e => e.AllergyId).HasName("PK__Allergy__A49EBE621B092302");

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

        modelBuilder.Entity<Disease>(entity =>
        {
            entity.HasKey(e => e.DiseaseId).HasName("PK__Disease__69B533A9AA4A8517");

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

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDF693DFA3A2");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId).HasColumnName("FeedbackID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MealPlanId).HasColumnName("MealPlanID");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.MealPlan).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.MealPlanId)
                .HasConstraintName("FK__Feedback__MealPl__151B244E");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedback__UserID__160F4887");
        });

        modelBuilder.Entity<FeedbackReply>(entity =>
        {
            entity.HasKey(e => e.ReplyId).HasName("PK__Feedback__C25E46291C1C26F8");

            entity.ToTable("FeedbackReply");

            entity.Property(e => e.ReplyId).HasColumnName("ReplyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FeedbackId).HasColumnName("FeedbackID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Feedback).WithMany(p => p.FeedbackReplies)
                .HasForeignKey(d => d.FeedbackId)
                .HasConstraintName("FK__FeedbackR__Feedb__19DFD96B");

            entity.HasOne(d => d.User).WithMany(p => p.FeedbackReplies)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FeedbackR__UserI__1AD3FDA4");
        });

        modelBuilder.Entity<Food>(entity =>
        {
            entity.HasKey(e => e.FoodId).HasName("PK__Food__856DB3CB6DEF1C4F");

            entity.ToTable("Food");

            entity.HasIndex(e => e.FoodName, "UQ__Food__81B4FC25AD3B4701").IsUnique();

            entity.Property(e => e.FoodId).HasColumnName("FoodID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.FoodName).HasMaxLength(100);
            entity.Property(e => e.FoodType).HasMaxLength(100);
            entity.Property(e => e.MealType).HasMaxLength(100);
            entity.Property(e => e.Others).HasMaxLength(255);
            entity.Property(e => e.ServingSize).HasMaxLength(50);
        });

        modelBuilder.Entity<FoodSubstitution>(entity =>
        {
            entity.HasKey(e => e.SubstitutionId).HasName("PK__FoodSubs__95BE7DE43E4E6F5E");

            entity.ToTable("FoodSubstitution");

            entity.Property(e => e.SubstitutionId).HasColumnName("SubstitutionID");
            entity.Property(e => e.OriginalFoodId).HasColumnName("OriginalFoodID");
            entity.Property(e => e.Reason).HasMaxLength(255);
            entity.Property(e => e.SubstituteFoodId).HasColumnName("SubstituteFoodID");

            entity.HasOne(d => d.OriginalFood).WithMany(p => p.FoodSubstitutionOriginalFoods)
                .HasForeignKey(d => d.OriginalFoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FoodSubst__Origi__1DB06A4F");

            entity.HasOne(d => d.SubstituteFood).WithMany(p => p.FoodSubstitutionSubstituteFoods)
                .HasForeignKey(d => d.SubstituteFoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FoodSubst__Subst__1EA48E88");
        });

        modelBuilder.Entity<HealthProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK__HealthPr__290C8884589FFAE6");

            entity.ToTable("HealthProfile");

            entity.Property(e => e.ProfileId).HasColumnName("ProfileID");
            entity.Property(e => e.ActivityLevel).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.HealthGoal).HasMaxLength(50);
            entity.Property(e => e.MedicalCondition).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.HealthProfiles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__HealthPro__UserI__656C112C");
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.IngredientId).HasName("PK__Ingredie__BEAEB27A90BD989A");

            entity.ToTable("Ingredient");

            entity.HasIndex(e => e.IngredientName, "UQ__Ingredie__A1B2F1CC8C1EC5D6").IsUnique();

            entity.Property(e => e.IngredientId).HasColumnName("IngredientID");
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.FoodId).HasColumnName("FoodID");
            entity.Property(e => e.IngredientName).HasMaxLength(100);
            entity.Property(e => e.Unit).HasMaxLength(20);

            entity.HasOne(d => d.Food).WithMany(p => p.Ingredients)
                .HasForeignKey(d => d.FoodId)
                .HasConstraintName("FK__Ingredien__FoodI__797309D9");
        });

        modelBuilder.Entity<MealLog>(entity =>
        {
            entity.HasKey(e => e.MealLogId).HasName("PK__MealLog__0ED21C528DAD99BF");

            entity.ToTable("MealLog");

            entity.Property(e => e.MealLogId).HasColumnName("MealLogID");
            entity.Property(e => e.LogDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MealType).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.MealLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__MealLog__UserID__32AB8735");
        });

        modelBuilder.Entity<MealLogDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__MealLogD__135C314DB6223106");

            entity.ToTable("MealLogDetail");

            entity.Property(e => e.DetailId).HasColumnName("DetailID");
            entity.Property(e => e.FoodId).HasColumnName("FoodID");
            entity.Property(e => e.MealLogId).HasColumnName("MealLogID");

            entity.HasOne(d => d.Food).WithMany(p => p.MealLogDetails)
                .HasForeignKey(d => d.FoodId)
                .HasConstraintName("FK__MealLogDe__FoodI__3864608B");

            entity.HasOne(d => d.MealLog).WithMany(p => p.MealLogDetails)
                .HasForeignKey(d => d.MealLogId)
                .HasConstraintName("FK__MealLogDe__MealL__37703C52");
        });

        modelBuilder.Entity<MealPlan>(entity =>
        {
            entity.HasKey(e => e.MealPlanId).HasName("PK__MealPlan__0620DB56BF7B950D");

            entity.ToTable("MealPlan");

            entity.HasIndex(e => e.PlanName, "UQ__MealPlan__46E12F9E1C83D4E9").IsUnique();

            entity.Property(e => e.MealPlanId).HasColumnName("MealPlanID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.HealthGoal).HasMaxLength(50);
            entity.Property(e => e.PlanName).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.MealPlans)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__MealPlan__UserID__09A971A2");
        });

        modelBuilder.Entity<MealPlanDetail>(entity =>
        {
            entity.HasKey(e => e.MealPlanDetailId).HasName("PK__MealPlan__37DC012B6C372BD1");

            entity.ToTable("MealPlanDetail");

            entity.Property(e => e.MealPlanDetailId).HasColumnName("MealPlanDetailID");
            entity.Property(e => e.FoodId).HasColumnName("FoodID");
            entity.Property(e => e.FoodName).HasMaxLength(255);
            entity.Property(e => e.MealPlanId).HasColumnName("MealPlanID");
            entity.Property(e => e.MealType).HasMaxLength(50);

            entity.HasOne(d => d.Food).WithMany(p => p.MealPlanDetails)
                .HasForeignKey(d => d.FoodId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__MealPlanD__FoodI__0F624AF8");

            entity.HasOne(d => d.MealPlan).WithMany(p => p.MealPlanDetails)
                .HasForeignKey(d => d.MealPlanId)
                .HasConstraintName("FK__MealPlanD__MealP__0E6E26BF");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E12CF010B48");

            entity.ToTable("Notification");

            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__236943A5");
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("PK__Package__322035ECBEE48E56");

            entity.ToTable("Package");

            entity.HasIndex(e => e.PackageName, "UQ__Package__73856F7A0F888EEB").IsUnique();

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
            entity.HasKey(e => e.GoalId).HasName("PK__Personal__8A4FFF3153FA0E40");

            entity.ToTable("PersonalGoal");

            entity.Property(e => e.GoalId).HasColumnName("GoalID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.GoalDescription).HasMaxLength(255);
            entity.Property(e => e.GoalType).HasMaxLength(50);
            entity.Property(e => e.ProgressPercentage).HasDefaultValue(0.0);
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
                .HasConstraintName("FK__PersonalG__UserI__41EDCAC5");
        });

        modelBuilder.Entity<RecipeSuggestion>(entity =>
        {
            entity.HasKey(e => e.RecipeId).HasName("PK__RecipeSu__FDD988D0919C4697");

            entity.ToTable("RecipeSuggestion");

            entity.Property(e => e.RecipeId).HasColumnName("RecipeID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.FoodId).HasColumnName("FoodID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Food).WithMany(p => p.RecipeSuggestions)
                .HasForeignKey(d => d.FoodId)
                .HasConstraintName("FK__RecipeSug__FoodI__7D439ABD");

            entity.HasOne(d => d.User).WithMany(p => p.RecipeSuggestions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__RecipeSug__UserI__7E37BEF6");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3AC93B41FA");

            entity.ToTable("Role");

            entity.HasIndex(e => e.RoleName, "UQ__Role__8A2B61605ED0EBA6").IsUnique();

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCAC056206E0");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D10534FB0A400E").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FcmToken).HasColumnName("fcmToken");
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Location).HasMaxLength(20);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User__RoleID__5165187F");

            entity.HasMany(d => d.Allergies).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserAllergy",
                    r => r.HasOne<Allergy>().WithMany()
                        .HasForeignKey("AllergyId")
                        .HasConstraintName("FK__UserAller__Aller__6D0D32F4"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__UserAller__UserI__6C190EBB"),
                    j =>
                    {
                        j.HasKey("UserId", "AllergyId").HasName("PK__UserAlle__2DC1274A2F74D231");
                        j.ToTable("UserAllergy");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                        j.IndexerProperty<int>("AllergyId").HasColumnName("AllergyID");
                    });

            entity.HasMany(d => d.Diseases).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserDisease",
                    r => r.HasOne<Disease>().WithMany()
                        .HasForeignKey("DiseaseId")
                        .HasConstraintName("FK__UserDisea__Disea__4D5F7D71"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__UserDisea__UserI__4C6B5938"),
                    j =>
                    {
                        j.HasKey("UserId", "DiseaseId").HasName("PK__UserDise__91139F9627AE9459");
                        j.ToTable("UserDisease");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                        j.IndexerProperty<int>("DiseaseId").HasColumnName("DiseaseID");
                    });
        });

        modelBuilder.Entity<UserEatingHabit>(entity =>
        {
            entity.HasKey(e => e.HabitId).HasName("PK__UserEati__C587AF03EE7D62B2");

            entity.ToTable("UserEatingHabit");

            entity.Property(e => e.HabitId).HasColumnName("HabitID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.HabitType).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.UserEatingHabits)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserEatin__UserI__45BE5BA9");
        });

        modelBuilder.Entity<UserFoodPreference>(entity =>
        {
            entity.HasKey(e => e.UserFoodPreferenceId).HasName("PK__UserFood__997D6AD73382E0ED");

            entity.ToTable("UserFoodPreference");

            entity.Property(e => e.UserFoodPreferenceId).HasColumnName("UserFoodPreferenceID");
            entity.Property(e => e.FoodId).HasColumnName("FoodID");
            entity.Property(e => e.Preference).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Food).WithMany(p => p.UserFoodPreferences)
                .HasForeignKey(d => d.FoodId)
                .HasConstraintName("FK__UserFoodP__FoodI__02084FDA");

            entity.HasOne(d => d.User).WithMany(p => p.UserFoodPreferences)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserFoodP__UserI__01142BA1");
        });

        modelBuilder.Entity<UserPackage>(entity =>
        {
            entity.HasKey(e => e.UserPackageId).HasName("PK__UserPack__AE9B91FA584F6F10");

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
                .HasConstraintName("FK__UserPacka__Packa__5DCAEF64");

            entity.HasOne(d => d.User).WithMany(p => p.UserPackages)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserPacka__UserI__5CD6CB2B");
        });

        modelBuilder.Entity<UserParameter>(entity =>
        {
            entity.HasKey(e => e.UserParameterId).HasName("PK__UserPara__DD228AC2F126ED40");

            entity.ToTable("UserParameter");

            entity.Property(e => e.UserParameterId).HasColumnName("UserParameterID");
            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.Aisuggestion)
                .HasMaxLength(255)
                .HasColumnName("AISuggestion");
            entity.Property(e => e.Bmi).HasColumnName("BMI");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FoodsAvoid).HasMaxLength(255);
            entity.Property(e => e.TargetDate).HasColumnType("datetime");
            entity.Property(e => e.Tdee).HasColumnName("TDEE");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.UserParameters)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserParam__UserI__29221CFB");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
