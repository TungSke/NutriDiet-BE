using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class PersonalGoal
{
    public int GoalId { get; set; }

    public int UserId { get; set; }

    public string GoalType { get; set; } = null!;

    public string GoalDescription { get; set; } = null!;

    public DateTime? StartDate { get; set; }

    public DateTime TargetDate { get; set; }

    public string? Status { get; set; }

    public double? ProgressPercentage { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
