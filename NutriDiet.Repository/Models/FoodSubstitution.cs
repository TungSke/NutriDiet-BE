using System;
using System.Collections.Generic;

namespace LogiConnect.Repository.Models;

public partial class FoodSubstitution
{
    public int SubstitutionId { get; set; }

    public int? UserId { get; set; }

    public int? OriginalFoodId { get; set; }

    public int? SubstituteFoodId { get; set; }

    public string? Reason { get; set; }

    public virtual Food? OriginalFood { get; set; }

    public virtual Food? SubstituteFood { get; set; }

    public virtual User? User { get; set; }
}
