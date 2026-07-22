using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Warehousemanager
{
    public int WarehouseManagerId { get; set; }

    public int WarehouseId { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
