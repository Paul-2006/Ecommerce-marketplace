using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Deliverylocationtracking
{
    public int LocationId { get; set; }

    public int DeliveryPartnerId { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public DateTime? RecordedTime { get; set; }

    public int OrderId { get; set; }

    public DateTime? TrackingTime { get; set; }
    public virtual Deliverypartner DeliveryPartner { get; set; } = null!;
}
