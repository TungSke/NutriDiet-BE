using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class HealthcareIndicator
{
    public int HealthcareIndicatorId { get; set; }

    public int UserId { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public double? CurrentValue { get; set; }

    public double? MinValue { get; set; }

    public double? MediumValue { get; set; }

    public double? MaxValue { get; set; }

    public bool? Active { get; set; }

    public string? Aisuggestion { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
