using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class GeneralHealthProfile
{
    public int ProfileId { get; set; }

    public int UserId { get; set; }

    public double? Height { get; set; }

    public double? Weight { get; set; }

    public string? ActivityLevel { get; set; }

    public string? Aisuggestion { get; set; }

    public bool? IsActive { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
