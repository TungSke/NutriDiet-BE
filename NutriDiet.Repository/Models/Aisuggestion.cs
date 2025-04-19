using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class Aisuggestion
{
    public int AisuggestionId { get; set; }

    public int ProfileId { get; set; }

    public string Content { get; set; } = null!;

    public string Type { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual GeneralHealthProfile Profile { get; set; } = null!;
}
