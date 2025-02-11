using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class UserParameter
{
    public int UserParameterId { get; set; }

    public int UserId { get; set; }

    public double? Tdee { get; set; }

    public double? Bmi { get; set; }

    public double? DailyCalorie { get; set; }

    public string? FoodsAvoid { get; set; }

    public string? Aisuggestion { get; set; }

    public DateTime? TargetDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? Active { get; set; }

    public virtual User User { get; set; } = null!;
}
