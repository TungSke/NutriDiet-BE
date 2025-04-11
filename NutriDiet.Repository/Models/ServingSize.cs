using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class ServingSize
{
    public int ServingSizeId { get; set; }

    public string UnitName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<FoodServingSize> FoodServingSizes { get; set; } = new List<FoodServingSize>();

    public virtual ICollection<Food> Foods { get; set; } = new List<Food>();
}
