using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class FoodIngredient
{
    public int FoodId { get; set; }

    public int IngredientId { get; set; }

    public double? Quantity { get; set; }

    public string Unit { get; set; } = null!;

    public virtual Food Food { get; set; } = null!;

    public virtual Ingredient Ingredient { get; set; } = null!;
}
