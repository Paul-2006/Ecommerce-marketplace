using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Faileddelivery
{
    public int FailedDeliveryId { get; set; }

    public int OrderId { get; set; }

    public int DeliveryPartnerId { get; set; }

    public string? Reason { get; set; }

    public int? AttemptNumber { get; set; }

    public DateTime? FailedDate { get; set; }

    public virtual Deliverypartner DeliveryPartner { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
