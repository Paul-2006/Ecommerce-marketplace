using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Deliverypartnerzone
{
    public int Id { get; set; }

    public int DeliveryPartnerId { get; set; }

    public int ZoneId { get; set; }

    public virtual Deliverypartner DeliveryPartner { get; set; } = null!;

    public virtual Deliveryzone Zone { get; set; } = null!;
}
