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

    public virtual ICollection<Airecommendation> Airecommendations { get; set; } = new List<Airecommendation>();

    public virtual ICollection<Allergy> Allergies { get; set; } = new List<Allergy>();

    public virtual ICollection<FeedbackReply> FeedbackReplies { get; set; } = new List<FeedbackReply>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<HealthProfile> HealthProfiles { get; set; } = new List<HealthProfile>();

    public virtual ICollection<MealLog> MealLogs { get; set; } = new List<MealLog>();

    public virtual ICollection<MealPlan> MealPlans { get; set; } = new List<MealPlan>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<PersonalGoal> PersonalGoals { get; set; } = new List<PersonalGoal>();

    public virtual ICollection<RecipeSuggestion> RecipeSuggestions { get; set; } = new List<RecipeSuggestion>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<UserEatingHabit> UserEatingHabits { get; set; } = new List<UserEatingHabit>();

    public virtual ICollection<UserFoodPreference> UserFoodPreferences { get; set; } = new List<UserFoodPreference>();

    public virtual ICollection<UserParameter> UserParameters { get; set; } = new List<UserParameter>();
}
