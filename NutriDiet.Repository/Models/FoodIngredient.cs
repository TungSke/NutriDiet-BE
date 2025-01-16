using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class FoodIngredient
{
    public int FoodIngredientId { get; set; }

    public int FoodId { get; set; }

    public int IngredientId { get; set; }

    public double? Amount { get; set; }

    public string? Notes { get; set; }

    public virtual Food Food { get; set; } = null!;

    public virtual Ingredient Ingredient { get; set; } = null!;
}
