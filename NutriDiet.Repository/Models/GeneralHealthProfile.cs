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

    public string? Evaluate { get; set; }

    public string? DietStyle { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Aisuggestion> Aisuggestions { get; set; } = new List<Aisuggestion>();

    public virtual ICollection<HealthcareIndicator> HealthcareIndicators { get; set; } = new List<HealthcareIndicator>();

    public virtual User User { get; set; } = null!;
}
