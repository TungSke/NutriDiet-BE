using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class UserFoodPreference
{
    public int UserFoodPreferenceId { get; set; }

    public int UserId { get; set; }

    public int FoodId { get; set; }

    public string? Preference { get; set; }

    public virtual Food Food { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
