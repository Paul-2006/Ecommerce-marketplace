using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Adminaction
{
    public int ActionId { get; set; }

    public int AdminId { get; set; }

    public string? ActionType { get; set; }

    public string? Description { get; set; }

    public DateTime? ActionDate { get; set; }

    public virtual User Admin { get; set; } = null!;
}
