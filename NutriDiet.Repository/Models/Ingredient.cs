using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class Ingredient
{
    public int IngredientId { get; set; }

    public string IngredientName { get; set; } = null!;

    public string? Category { get; set; }

    public string Unit { get; set; } = null!;

    public virtual ICollection<FoodIngredient> FoodIngredients { get; set; } = new List<FoodIngredient>();
}
