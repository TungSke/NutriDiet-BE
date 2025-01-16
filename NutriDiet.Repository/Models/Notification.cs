using System;
using System.Collections.Generic;

namespace NutriDiet.Repository.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? Date { get; set; }

    public virtual User User { get; set; } = null!;
}
