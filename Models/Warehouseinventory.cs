using System;

namespace Ecommerce.Models;

public partial class Warehouseinventory
{
    public int WarehouseInventoryId { get; set; }

    public int WarehouseId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public DateTime? UpdatedDate { get; set; }


    public virtual Warehouse Warehouse { get; set; } = null!;


    public virtual Product Product { get; set; } = null!;
}