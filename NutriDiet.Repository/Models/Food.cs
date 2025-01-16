using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class Food
{
    public int FoodId { get; set; }

    public string FoodName { get; set; } = null!;

    public string? MealType { get; set; }

    public string? ImageUrl { get; set; }

    public string? FoodType { get; set; }

    public string? Description { get; set; }

    public string? ServingSize { get; set; }

    public double? Calories { get; set; }

    public double? Protein { get; set; }

    public double? Carbs { get; set; }

    public double? Fat { get; set; }

    public double? Glucid { get; set; }

    public double? Fiber { get; set; }

    public string? Others { get; set; }

    public virtual ICollection<FoodIngredient> FoodIngredients { get; set; } = new List<FoodIngredient>();

    public virtual ICollection<FoodSubstitution> FoodSubstitutionOriginalFoods { get; set; } = new List<FoodSubstitution>();

    public virtual ICollection<FoodSubstitution> FoodSubstitutionSubstituteFoods { get; set; } = new List<FoodSubstitution>();

    public virtual ICollection<MealLogDetail> MealLogDetails { get; set; } = new List<MealLogDetail>();

    public virtual ICollection<MealPlanDetail> MealPlanDetails { get; set; } = new List<MealPlanDetail>();

    public virtual ICollection<RecipeSuggestion> RecipeSuggestions { get; set; } = new List<RecipeSuggestion>();

    public virtual ICollection<UserFoodPreference> UserFoodPreferences { get; set; } = new List<UserFoodPreference>();
}
