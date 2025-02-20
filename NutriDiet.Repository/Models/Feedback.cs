using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int MealPlanId { get; set; }

    public int UserId { get; set; }

    public int? Rating { get; set; }

    public string? Message { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual MealPlan MealPlan { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
