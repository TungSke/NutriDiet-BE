using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class RecipeSuggestion
{
    public int RecipeId { get; set; }

    public int UserId { get; set; }

    public int FoodId { get; set; }

    public int CuisineId { get; set; }

    public string? Airequest { get; set; }

    public string? Airesponse { get; set; }

    public string Aimodel { get; set; } = null!;

    public string? RejectionReason { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual CuisineType Cuisine { get; set; } = null!;

    public virtual Food Food { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
