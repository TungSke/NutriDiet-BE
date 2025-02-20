using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class Airecommendation
{
    public int RecommendationId { get; set; }

    public int UserId { get; set; }

    public DateTime? RecommendedAt { get; set; }

    public string? RecommendationText { get; set; }

    public virtual User User { get; set; } = null!;
}
