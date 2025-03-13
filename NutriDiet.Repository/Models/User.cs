using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Phone { get; set; }

    public int? Age { get; set; }

    public string? Gender { get; set; }

    public string? Location { get; set; }

    public string? Avatar { get; set; }

    public string? FcmToken { get; set; }

    public string? Status { get; set; }

    public int RoleId { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public virtual ICollection<GeneralHealthProfile> GeneralHealthProfiles { get; set; } = new List<GeneralHealthProfile>();

    public virtual ICollection<HealthcareIndicator> HealthcareIndicators { get; set; } = new List<HealthcareIndicator>();

    public virtual ICollection<MealLog> MealLogs { get; set; } = new List<MealLog>();

    public virtual ICollection<MealPlan> MealPlans { get; set; } = new List<MealPlan>();

    public virtual ICollection<MyFood> MyFoods { get; set; } = new List<MyFood>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<PersonalGoal> PersonalGoals { get; set; } = new List<PersonalGoal>();

    public virtual ICollection<RecipeSuggestion> RecipeSuggestions { get; set; } = new List<RecipeSuggestion>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<UserFoodPreference> UserFoodPreferences { get; set; } = new List<UserFoodPreference>();

    public virtual ICollection<UserIngreDientPreference> UserIngreDientPreferences { get; set; } = new List<UserIngreDientPreference>();

    public virtual ICollection<UserPackage> UserPackages { get; set; } = new List<UserPackage>();

    public virtual ICollection<Allergy> Allergies { get; set; } = new List<Allergy>();

    public virtual ICollection<Disease> Diseases { get; set; } = new List<Disease>();
}
