using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? Age { get; set; }

    public string? Gender { get; set; }

    public string? Avatar { get; set; }

    public string Status { get; set; } = null!;

    public int? RoleId { get; set; }

    public virtual ICollection<Allergy> Allergies { get; set; } = new List<Allergy>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<FoodSubstitution> FoodSubstitutions { get; set; } = new List<FoodSubstitution>();

    public virtual ICollection<HealthProfile> HealthProfiles { get; set; } = new List<HealthProfile>();

    public virtual ICollection<MealPlan> MealPlans { get; set; } = new List<MealPlan>();

    public virtual ICollection<Recommendation> Recommendations { get; set; } = new List<Recommendation>();

    public virtual Role? Role { get; set; }
}
