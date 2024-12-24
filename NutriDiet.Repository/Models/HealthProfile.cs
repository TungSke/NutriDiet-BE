using System;
using System.Collections.Generic;

namespace LogiConnect.Repository.Models;

public partial class HealthProfile
{
    public int ProfileId { get; set; }

    public int? UserId { get; set; }

    public string? MedicalConditions { get; set; }

    public double? HeightCm { get; set; }

    public double? WeightKg { get; set; }

    public string? ActivityLevel { get; set; }

    public string? Goal { get; set; }

    public double? TargetWeight { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? User { get; set; }
}
