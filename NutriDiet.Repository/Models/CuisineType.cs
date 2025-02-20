using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class CuisineType
{
    public int CuisineId { get; set; }

    public string CuisineName { get; set; } = null!;

    public virtual ICollection<RecipeSuggestion> RecipeSuggestions { get; set; } = new List<RecipeSuggestion>();
}
