using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class AirecommendMealPlan
{
    public int AirecommendMealPlanId { get; set; }

    public int? MealPlanId { get; set; }

    public int UserId { get; set; }

    public DateTime? RecommendedAt { get; set; }

    public string? AirecommendMealPlanResponse { get; set; }

    public string Status { get; set; } = null!;

    public string? RejectionReason { get; set; }

    public string? Feedback { get; set; }

    public virtual MealPlan? MealPlan { get; set; }

    public virtual User User { get; set; } = null!;
}
