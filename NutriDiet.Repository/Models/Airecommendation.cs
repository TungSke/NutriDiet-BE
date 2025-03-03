using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class Airecommendation
{
    public int RecommendationId { get; set; }

    public int UserId { get; set; }

    public DateTime? RecommendedAt { get; set; }

    public string? AirecommendationResponse { get; set; }

    public string Status { get; set; } = null!;

    public string? RejectionReason { get; set; }

    public virtual User User { get; set; } = null!;
}
