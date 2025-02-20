using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class MealPlan
{
    public int MealPlanId { get; set; }

    public string PlanName { get; set; } = null!;

    public string? HealthGoal { get; set; }

    public int? Duration { get; set; }

    public string? Status { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<FeedbackMealPlan> FeedbackMealPlans { get; set; } = new List<FeedbackMealPlan>();

    public virtual ICollection<MealPlanDetail> MealPlanDetails { get; set; } = new List<MealPlanDetail>();

    public virtual ICollection<UserMealPlan> UserMealPlans { get; set; } = new List<UserMealPlan>();
}
