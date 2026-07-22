using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Warehouse
{
    public int WarehouseId { get; set; }

    public string? WarehouseName { get; set; }

    public string? Location { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public int? Capacity { get; set; }

    public string? ContactNumber { get; set; }

    public DateTime? CreatedDate { get; set; }
    public string? Status { get; set; }

    public virtual ICollection<Warehousemanager> Warehousemanagers { get; set; } = new List<Warehousemanager>();

    public virtual ICollection<Warehousenotification> Warehousenotifications { get; set; } = new List<Warehousenotification>();
}
