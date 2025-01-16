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

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<FeedbackReply> FeedbackReplies { get; set; }

    public virtual DbSet<Food> Foods { get; set; }

    public virtual DbSet<FoodIngredient> FoodIngredients { get; set; }

    public virtual DbSet<FoodSubstitution> FoodSubstitutions { get; set; }

    public virtual DbSet<HealthProfile> HealthProfiles { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<MealLog> MealLogs { get; set; }

    public virtual DbSet<MealLogDetail> MealLogDetails { get; set; }

    public virtual DbSet<MealPlan> MealPlans { get; set; }

    public virtual DbSet<MealPlanDetail> MealPlanDetails { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<PersonalGoal> PersonalGoals { get; set; }

    public virtual DbSet<RecipeSuggestion> RecipeSuggestions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserEatingHabit> UserEatingHabits { get; set; }

    public virtual DbSet<UserFoodPreference> UserFoodPreferences { get; set; }

    public virtual DbSet<UserParameter> UserParameters { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(local);database=NutriDiet;uid=sa;pwd=12345;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Airecommendation>(entity =>
        {
            entity.HasKey(e => e.RecommendationId).HasName("PK__AIRecomm__AA15BEC4E802D6E9");

            entity.ToTable("AIRecommendation");

            entity.Property(e => e.RecommendationId).HasColumnName("RecommendationID");
            entity.Property(e => e.IsAccepted).HasDefaultValue(false);
            entity.Property(e => e.RecommendedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Airecommendations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__AIRecomme__UserI__0D7A0286");
        });

        modelBuilder.Entity<Allergy>(entity =>
        {
            entity.HasKey(e => e.AllergyId).HasName("PK__Allergy__A49EBE625976F8E4");

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
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Allergies)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Allergy__UserID__4BAC3F29");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDF62F71B562");

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
                .HasConstraintName("FK__Feedback__MealPl__76969D2E");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedback__UserID__778AC167");
        });

        modelBuilder.Entity<FeedbackReply>(entity =>
        {
            entity.HasKey(e => e.ReplyId).HasName("PK__Feedback__C25E46299F4278D0");

            entity.ToTable("FeedbackReply");

            entity.Property(e => e.ReplyId).HasColumnName("ReplyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FeedbackId).HasColumnName("FeedbackID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Feedback).WithMany(p => p.FeedbackReplies)
                .HasForeignKey(d => d.FeedbackId)
                .HasConstraintName("FK__FeedbackR__Feedb__7B5B524B");

            entity.HasOne(d => d.User).WithMany(p => p.FeedbackReplies)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FeedbackR__UserI__7C4F7684");
        });

        modelBuilder.Entity<Food>(entity =>
        {
            entity.HasKey(e => e.FoodId).HasName("PK__Food__856DB3CB674A404D");

            entity.ToTable("Food");

            entity.HasIndex(e => e.FoodName, "UQ__Food__81B4FC250A7A47C7").IsUnique();

            entity.Property(e => e.FoodId).HasColumnName("FoodID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.FoodName).HasMaxLength(100);
            entity.Property(e => e.FoodType).HasMaxLength(100);
            entity.Property(e => e.MealType).HasMaxLength(100);
            entity.Property(e => e.Others).HasMaxLength(255);
            entity.Property(e => e.ServingSize).HasMaxLength(50);
        });

        modelBuilder.Entity<FoodIngredient>(entity =>
        {
            entity.HasKey(e => e.FoodIngredientId).HasName("PK__FoodIngr__CB78CF4638261613");

            entity.ToTable("FoodIngredient");

            entity.Property(e => e.FoodIngredientId).HasColumnName("FoodIngredientID");
            entity.Property(e => e.FoodId).HasColumnName("FoodID");
            entity.Property(e => e.IngredientId).HasColumnName("IngredientID");
            entity.Property(e => e.Notes).HasMaxLength(255);

            entity.HasOne(d => d.Food).WithMany(p => p.FoodIngredients)
                .HasForeignKey(d => d.FoodId)
                .HasConstraintName("FK__FoodIngre__FoodI__5AEE82B9");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.FoodIngredients)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FoodIngre__Ingre__5BE2A6F2");
        });

        modelBuilder.Entity<FoodSubstitution>(entity =>
        {
            entity.HasKey(e => e.SubstitutionId).HasName("PK__FoodSubs__95BE7DE45BD468A1");

            entity.ToTable("FoodSubstitution");

            entity.Property(e => e.SubstitutionId).HasColumnName("SubstitutionID");
            entity.Property(e => e.OriginalFoodId).HasColumnName("OriginalFoodID");
            entity.Property(e => e.Reason).HasMaxLength(255);
            entity.Property(e => e.SubstituteFoodId).HasColumnName("SubstituteFoodID");

            entity.HasOne(d => d.OriginalFood).WithMany(p => p.FoodSubstitutionOriginalFoods)
                .HasForeignKey(d => d.OriginalFoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FoodSubst__Origi__7F2BE32F");

            entity.HasOne(d => d.SubstituteFood).WithMany(p => p.FoodSubstitutionSubstituteFoods)
                .HasForeignKey(d => d.SubstituteFoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FoodSubst__Subst__00200768");
        });

        modelBuilder.Entity<HealthProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK__HealthPr__290C8884661DA2E1");

            entity.ToTable("HealthProfile");

            entity.Property(e => e.ProfileId).HasColumnName("ProfileID");
            entity.Property(e => e.ActivityLevel).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DurationTarget).HasColumnType("datetime");
            entity.Property(e => e.HealthGoal).HasMaxLength(50);
            entity.Property(e => e.MedicalCondition).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.HealthProfiles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__HealthPro__UserI__46E78A0C");
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.IngredientId).HasName("PK__Ingredie__BEAEB27A76FB5519");

            entity.ToTable("Ingredient");

            entity.HasIndex(e => e.IngredientName, "UQ__Ingredie__A1B2F1CC1B2CD6D2").IsUnique();

            entity.Property(e => e.IngredientId).HasColumnName("IngredientID");
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.IngredientName).HasMaxLength(100);
            entity.Property(e => e.Unit).HasMaxLength(20);
        });

        modelBuilder.Entity<MealLog>(entity =>
        {
            entity.HasKey(e => e.MealLogId).HasName("PK__MealLog__0ED21C52672EB1C7");

            entity.ToTable("MealLog");

            entity.Property(e => e.MealLogId).HasColumnName("MealLogID");
            entity.Property(e => e.LogDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MealType).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.MealLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__MealLog__UserID__123EB7A3");
        });

        modelBuilder.Entity<MealLogDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__MealLogD__135C314D573CC488");

            entity.ToTable("MealLogDetail");

            entity.Property(e => e.DetailId).HasColumnName("DetailID");
            entity.Property(e => e.FoodId).HasColumnName("FoodID");
            entity.Property(e => e.MealLogId).HasColumnName("MealLogID");

            entity.HasOne(d => d.Food).WithMany(p => p.MealLogDetails)
                .HasForeignKey(d => d.FoodId)
                .HasConstraintName("FK__MealLogDe__FoodI__17F790F9");

            entity.HasOne(d => d.MealLog).WithMany(p => p.MealLogDetails)
                .HasForeignKey(d => d.MealLogId)
                .HasConstraintName("FK__MealLogDe__MealL__17036CC0");
        });

        modelBuilder.Entity<MealPlan>(entity =>
        {
            entity.HasKey(e => e.MealPlanId).HasName("PK__MealPlan__0620DB5631C9BBCB");

            entity.ToTable("MealPlan");

            entity.HasIndex(e => e.PlanName, "UQ__MealPlan__46E12F9E1A448082").IsUnique();

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
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.MealPlans)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__MealPlan__UserID__6B24EA82");
        });

        modelBuilder.Entity<MealPlanDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__MealPlan__135C314D289D2E7D");

            entity.ToTable("MealPlanDetail");

            entity.Property(e => e.DetailId).HasColumnName("DetailID");
            entity.Property(e => e.FoodId).HasColumnName("FoodID");
            entity.Property(e => e.FoodName).HasMaxLength(255);
            entity.Property(e => e.MealPlanId).HasColumnName("MealPlanID");
            entity.Property(e => e.MealType).HasMaxLength(50);

            entity.HasOne(d => d.Food).WithMany(p => p.MealPlanDetails)
                .HasForeignKey(d => d.FoodId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__MealPlanD__FoodI__70DDC3D8");

            entity.HasOne(d => d.MealPlan).WithMany(p => p.MealPlanDetails)
                .HasForeignKey(d => d.MealPlanId)
                .HasConstraintName("FK__MealPlanD__MealP__6FE99F9F");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E12886DD7ED");

            entity.ToTable("Notification");

            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__04E4BC85");
        });

        modelBuilder.Entity<PersonalGoal>(entity =>
        {
            entity.HasKey(e => e.GoalId).HasName("PK__Personal__8A4FFF3113D0A6B7");

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
                .HasConstraintName("FK__PersonalG__UserI__2180FB33");
        });

        modelBuilder.Entity<RecipeSuggestion>(entity =>
        {
            entity.HasKey(e => e.RecipeId).HasName("PK__RecipeSu__FDD988D0DBC187CC");

            entity.ToTable("RecipeSuggestion");

            entity.Property(e => e.RecipeId).HasColumnName("RecipeID");
            entity.Property(e => e.FoodId).HasColumnName("FoodID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Food).WithMany(p => p.RecipeSuggestions)
                .HasForeignKey(d => d.FoodId)
                .HasConstraintName("FK__RecipeSug__FoodI__5EBF139D");

            entity.HasOne(d => d.User).WithMany(p => p.RecipeSuggestions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__RecipeSug__UserI__5FB337D6");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3A081AD2FF");

            entity.ToTable("Role");

            entity.HasIndex(e => e.RoleName, "UQ__Role__8A2B616083D4237A").IsUnique();

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCAC83F47EB5");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D105341397CF63").IsUnique();

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
                .HasConstraintName("FK__User__RoleID__3F466844");
        });

        modelBuilder.Entity<UserEatingHabit>(entity =>
        {
            entity.HasKey(e => e.HabitId).HasName("PK__UserEati__C587AF03936CFDFD");

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
                .HasConstraintName("FK__UserEatin__UserI__25518C17");
        });

        modelBuilder.Entity<UserFoodPreference>(entity =>
        {
            entity.HasKey(e => e.UserFoodPreferenceId).HasName("PK__UserFood__997D6AD7FF99A850");

            entity.ToTable("UserFoodPreference");

            entity.Property(e => e.UserFoodPreferenceId).HasColumnName("UserFoodPreferenceID");
            entity.Property(e => e.FoodId).HasColumnName("FoodID");
            entity.Property(e => e.Preference).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Food).WithMany(p => p.UserFoodPreferences)
                .HasForeignKey(d => d.FoodId)
                .HasConstraintName("FK__UserFoodP__FoodI__6383C8BA");

            entity.HasOne(d => d.User).WithMany(p => p.UserFoodPreferences)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserFoodP__UserI__628FA481");
        });

        modelBuilder.Entity<UserParameter>(entity =>
        {
            entity.HasKey(e => e.UserParameterId).HasName("PK__UserPara__DD228AC28DD6A541");

            entity.ToTable("UserParameter");

            entity.Property(e => e.UserParameterId).HasColumnName("UserParameterID");
            entity.Property(e => e.Bmi).HasColumnName("BMI");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Suggestion).HasMaxLength(255);
            entity.Property(e => e.Tdee).HasColumnName("TDEE");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.UserParameters)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserParam__UserI__08B54D69");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
