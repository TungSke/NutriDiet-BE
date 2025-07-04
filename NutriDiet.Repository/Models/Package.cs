﻿using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class Package
{
    public int PackageId { get; set; }

    public string PackageName { get; set; } = null!;

    public string? PackageType { get; set; }

    public double? Price { get; set; }

    public int? Duration { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<UserPackage> UserPackagePackages { get; set; } = new List<UserPackage>();

    public virtual ICollection<UserPackage> UserPackagePreviousPackages { get; set; } = new List<UserPackage>();
}
