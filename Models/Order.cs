using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public int AddressId { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? OrderStatus { get; set; }

    public virtual Customeraddress Address { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Customerfeedback> Customerfeedbacks { get; set; } = new List<Customerfeedback>();

    public virtual ICollection<Deliveryassignment> Deliveryassignments { get; set; } = new List<Deliveryassignment>();

    public virtual ICollection<Deliveryinstruction> Deliveryinstructions { get; set; } = new List<Deliveryinstruction>();

    public virtual ICollection<Deliveryotp> Deliveryotps { get; set; } = new List<Deliveryotp>();

    public virtual ICollection<Faileddelivery> Faileddeliveries { get; set; } = new List<Faileddelivery>();

    public virtual ICollection<Orderitem> Orderitems { get; set; } = new List<Orderitem>();

    public virtual ICollection<Orderstatushistory> Orderstatushistories { get; set; } = new List<Orderstatushistory>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Sellerorder> Sellerorders { get; set; } = new List<Sellerorder>();

    public virtual ICollection<Warehousenotification> Warehousenotifications { get; set; } = new List<Warehousenotification>();
}
