using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class FoodDetail
{
    public int FoodDetailId { get; set; }

    public int? FoodId { get; set; }

    public string? FoodDetailName { get; set; }

    public string Unit { get; set; } = null!;

    public double? Amount { get; set; }

    public string? Description { get; set; }

    public virtual Food? Food { get; set; }
}
