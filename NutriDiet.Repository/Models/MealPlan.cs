using System;
using System.Collections.Generic;

namespace LogiConnect.Repository.Models;

public partial class MealPlan
{
    public int MealPlanId { get; set; }

    public int? UserId { get; set; }

    public string? PlanName { get; set; }

    public double? TotalCalories { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<MealPlanDetail> MealPlanDetails { get; set; } = new List<MealPlanDetail>();

    public virtual User? User { get; set; }
}
