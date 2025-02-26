using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class MealLog
{
    public int MealLogId { get; set; }

    public int UserId { get; set; }

    public DateTime? LogDate { get; set; }

    public double? TotalCalories { get; set; }

    public double? TotalProtein { get; set; }

    public double? TotalCarbs { get; set; }

    public double? TotalFat { get; set; }

    public virtual ICollection<MealLogDetail> MealLogDetails { get; set; } = new List<MealLogDetail>();

    public virtual User User { get; set; } = null!;
}
