using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Productspecification
{
    public int SpecificationId { get; set; }

    public int ProductId { get; set; }

    public string? SpecificationName { get; set; }

    public string? SpecificationValue { get; set; }

    public virtual Product Product { get; set; } = null!;
}
