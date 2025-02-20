using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class MealPlanDetail
{
    public int MealPlanDetailId { get; set; }

    public int MealPlanId { get; set; }

    public int? FoodId { get; set; }

    public string? FoodName { get; set; }

    public double? Quantity { get; set; }

    public string? MealType { get; set; }

    public int DayNumber { get; set; }

    public double? TotalCalories { get; set; }

    public double? TotalCarbs { get; set; }

    public double? TotalFat { get; set; }

    public double? TotalProtein { get; set; }

    public virtual Food? Food { get; set; }

    public virtual MealPlan MealPlan { get; set; } = null!;
}
