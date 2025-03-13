using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class Ingredient
{
    public int IngredientId { get; set; }

    public string IngredientName { get; set; } = null!;

    public double? Calories { get; set; }

    public double? Protein { get; set; }

    public double? Carbs { get; set; }

    public double? Fat { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<FoodIngredient> FoodIngredients { get; set; } = new List<FoodIngredient>();

    public virtual ICollection<UserIngreDientPreference> UserIngreDientPreferences { get; set; } = new List<UserIngreDientPreference>();
}
