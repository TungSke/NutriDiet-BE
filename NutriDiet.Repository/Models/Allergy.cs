using System;
using System.Collections.Generic;

namespace LogiConnect.Repository.Models;

public partial class Allergy
{
    public int AllergyId { get; set; }

    public int? UserId { get; set; }

    public string FoodName { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? User { get; set; }
}
