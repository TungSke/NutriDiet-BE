using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class UserParameter
{
    public int UserParameterId { get; set; }

    public int UserId { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public double? MinValue { get; set; }

    public double? MaxValue { get; set; }

    public bool? Active { get; set; }

    public string? Aisuggestion { get; set; }

    public DateTime? Date { get; set; }

    public virtual User User { get; set; } = null!;
}
