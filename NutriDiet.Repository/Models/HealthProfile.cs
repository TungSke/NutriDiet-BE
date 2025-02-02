using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class HealthProfile
{
    public int ProfileId { get; set; }

    public int UserId { get; set; }

    public string? MedicalCondition { get; set; }

    public double? Height { get; set; }

    public double? Weight { get; set; }

    public string? ActivityLevel { get; set; }

    public string? HealthGoal { get; set; }

    public double? TargetWeight { get; set; }

    public DateTime? DurationTarget { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
