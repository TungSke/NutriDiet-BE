using System;
using System.Collections.Generic;

namespace NutriDiet.Service.ModelDTOs.Request;

public class SystemConfigurationRequest
{
    public string Name { get; set; } = null!;

    public double? MinValue { get; set; }

    public double? MaxValue { get; set; }

    public string? Unit { get; set; }

    public bool? IsActive { get; set; }

    public string? Description { get; set; }
}