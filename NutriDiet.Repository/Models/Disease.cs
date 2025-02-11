using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class Disease
{
    public int DiseaseId { get; set; }

    public string DiseaseName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
