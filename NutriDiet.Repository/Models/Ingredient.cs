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

    public virtual ICollection<UserIngreDientPreference> UserIngreDientPreferences { get; set; } = new List<UserIngreDientPreference>();

    public virtual ICollection<Allergy> Allergies { get; set; } = new List<Allergy>();

    public virtual ICollection<Disease> Diseases { get; set; } = new List<Disease>();

    public virtual ICollection<Food> Foods { get; set; } = new List<Food>();
}
