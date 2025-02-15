using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class MyFood
{
    public int MyFoodId { get; set; }

    public int UserId { get; set; }

    public string FoodName { get; set; } = null!;

    public string? ServingSize { get; set; }

    public double? Calories { get; set; }

    public double? Protein { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
