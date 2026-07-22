using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Deliveryassignment
{
    public int DeliveryAssignmentId { get; set; }

    public int OrderId { get; set; }

    public int DeliveryPartnerId { get; set; }

    public DateTime? AssignedDate { get; set; }

    public string? Status { get; set; }
    public string? DeliveryStatus { get; set; }

    public virtual Deliverypartner DeliveryPartner { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
