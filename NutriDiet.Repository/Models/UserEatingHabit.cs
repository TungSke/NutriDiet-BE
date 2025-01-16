using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class UserEatingHabit
{
    public int HabitId { get; set; }

    public int UserId { get; set; }

    public string HabitType { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
