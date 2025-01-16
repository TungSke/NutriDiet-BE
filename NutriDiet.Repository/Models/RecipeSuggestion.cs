using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class RecipeSuggestion
{
    public int RecipeId { get; set; }

    public int UserId { get; set; }

    public int FoodId { get; set; }

    public string Description { get; set; } = null!;

    public virtual Food Food { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
