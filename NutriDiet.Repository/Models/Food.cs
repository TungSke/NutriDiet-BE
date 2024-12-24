using System;
using System.Collections.Generic;

namespace LogiConnect.Repository.Models;

public partial class Food
{
    public int FoodId { get; set; }

    public string FoodName { get; set; } = null!;

    public string? FoodType { get; set; }

    public string? Description { get; set; }

    public string? ServingSize { get; set; }

    public virtual ICollection<FoodDetail> FoodDetails { get; set; } = new List<FoodDetail>();

    public virtual ICollection<FoodSubstitution> FoodSubstitutionOriginalFoods { get; set; } = new List<FoodSubstitution>();

    public virtual ICollection<FoodSubstitution> FoodSubstitutionSubstituteFoods { get; set; } = new List<FoodSubstitution>();

    public virtual ICollection<MealPlanDetail> MealPlanDetails { get; set; } = new List<MealPlanDetail>();
}
