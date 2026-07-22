using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Searchhistory
{
    public int SearchId { get; set; }

    public int CustomerId { get; set; }

    public string? SearchText { get; set; }

    public string? SearchType { get; set; }

    public DateTime? SearchedDate { get; set; }

    public string? SearchKeyword { get; set; }

    public DateTime? SearchDate { get; set; }
    public virtual Customer Customer { get; set; } = null!;
}
