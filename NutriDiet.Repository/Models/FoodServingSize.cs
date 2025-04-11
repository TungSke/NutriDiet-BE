using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class FoodServingSize
{
    public int FoodId { get; set; }

    public int ServingSizeId { get; set; }

    public double? Quantity { get; set; }

    public double? Calories { get; set; }

    public double? Protein { get; set; }

    public double? Carbs { get; set; }

    public double? Fat { get; set; }

    public double? Glucid { get; set; }

    public double? Fiber { get; set; }

    public virtual Food Food { get; set; } = null!;

    public virtual ServingSize ServingSize { get; set; } = null!;
}
