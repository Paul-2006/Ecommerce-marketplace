using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Orderstatushistory
{
    public int HistoryId { get; set; }

    public int OrderId { get; set; }

    public string? Status { get; set; }

    public DateTime? UpdatedTime { get; set; }

    public string? Remarks { get; set; }

    public DateTime? CreatedDate { get; set; }
    public virtual Order Order { get; set; } = null!;
}
