﻿using System;
using System.Collections.Generic;

namespace NutriDiet.Service.ModelDTOs.Response;

public class SystemConfigurationResponse
{
    public int ConfigId { get; set; }

    public string Name { get; set; } = null!;

    public double? MinValue { get; set; }

    public double? MaxValue { get; set; }

    public string? Unit { get; set; }

    public bool? IsActive { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}