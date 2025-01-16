using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class UserParameter
{
    public int UserParameterId { get; set; }

    public int UserId { get; set; }

    public double Bmi { get; set; }

    public double Tdee { get; set; }

    public double CaloriesRequirement { get; set; }

    public double WaterRequirement { get; set; }

    public string? Suggestion { get; set; }

    public DateTime? Date { get; set; }

    public virtual User User { get; set; } = null!;
}
