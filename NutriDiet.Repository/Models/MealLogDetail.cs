using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class MealLogDetail
{
    public int DetailId { get; set; }

    public int MealLogId { get; set; }

    public int? FoodId { get; set; }

    public string FoodName { get; set; } = null!;

    public string? MealType { get; set; }

    public double? Quantity { get; set; }

    public string? ImageUrl { get; set; }

    public double? Calories { get; set; }

    public string? ServingSize { get; set; }

    public double? Protein { get; set; }

    public double? Carbs { get; set; }

    public double? Fat { get; set; }

    public virtual Food? Food { get; set; }

    public virtual MealLog MealLog { get; set; } = null!;
}
