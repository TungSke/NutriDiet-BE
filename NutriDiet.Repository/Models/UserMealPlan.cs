using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class UserMealPlan
{
    public int UserMealPlanId { get; set; }

    public int UserId { get; set; }

    public int MealPlanId { get; set; }

    public DateTime? AppliedAt { get; set; }

    public virtual MealPlan MealPlan { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
