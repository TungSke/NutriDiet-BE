using System;
using System.Collections.Generic;

namespace LogiConnect.Repository.Models;

public partial class Recommendation
{
    public int RecommendationId { get; set; }

    public int? UserId { get; set; }

    public string? RecommendationText { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
