using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Warehousenotification
{
    public int NotificationId { get; set; }

    public int WarehouseId { get; set; }

    public int OrderId { get; set; }

    public string? Message { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
