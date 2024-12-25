using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class MealPlanDetail
{
    public int DetailId { get; set; }

    public int? MealPlanId { get; set; }

    public int? FoodId { get; set; }

    public double? Quantity { get; set; }

    public string? MealType { get; set; }

    public virtual Food? Food { get; set; }

    public virtual MealPlan? MealPlan { get; set; }
}
