﻿using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class MealPlan
{
    public int MealPlanId { get; set; }

    public int UserId { get; set; }

    public string PlanName { get; set; } = null!;

    public string? HealthGoal { get; set; }

    public int? Duration { get; set; }

    public string? Status { get; set; }

    public string? Aiwarning { get; set; }

    public DateTime? StartAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AirecommendMealPlan> AirecommendMealPlans { get; set; } = new List<AirecommendMealPlan>();

    public virtual ICollection<MealPlanDetail> MealPlanDetails { get; set; } = new List<MealPlanDetail>();

    public virtual User User { get; set; } = null!;
}
