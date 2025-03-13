using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class UserIngreDientPreference
{
    public int UserIngreDientPreferenceId { get; set; }

    public int UserId { get; set; }

    public int IngredientId { get; set; }

    public int? Level { get; set; }

    public virtual Ingredient Ingredient { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
