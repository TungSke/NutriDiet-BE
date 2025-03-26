using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class Allergy
{
    public int AllergyId { get; set; }

    public string AllergyName { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
