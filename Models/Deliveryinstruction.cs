using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Deliveryinstruction
{
    public int InstructionId { get; set; }

    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public string? InstructionMessage { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
