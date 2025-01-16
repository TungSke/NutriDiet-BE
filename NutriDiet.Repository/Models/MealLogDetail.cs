using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class MealLogDetail
{
    public int DetailId { get; set; }

    public int MealLogId { get; set; }

    public int FoodId { get; set; }

    public double? Quantity { get; set; }

    public double? Calories { get; set; }

    public virtual Food Food { get; set; } = null!;

    public virtual MealLog MealLog { get; set; } = null!;
}
