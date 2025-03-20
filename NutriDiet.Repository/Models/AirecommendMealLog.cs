using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class AirecommendMealLog
{
    public int AirecommendMealLogId { get; set; }

    public int? MealLogId { get; set; }

    public int UserId { get; set; }

    public DateTime? RecommendedAt { get; set; }

    public string? AirecommendMealLogResponse { get; set; }

    public string Status { get; set; } = null!;

    public string? RejectionReason { get; set; }

    public string? Feedback { get; set; }

    public virtual MealLog? MealLog { get; set; }

    public virtual User User { get; set; } = null!;
}
