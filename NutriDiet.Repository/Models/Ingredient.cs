using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class Ingredient
{
    public int IngredientId { get; set; }

    public string IngredientName { get; set; } = null!;

    public string? Category { get; set; }

    public string Unit { get; set; } = null!;

    public double? Calories { get; set; }

    public int FoodId { get; set; }

    public virtual Food Food { get; set; } = null!;
}
