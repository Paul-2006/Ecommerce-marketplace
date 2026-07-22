using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public int RoleId { get; set; }

    public string? AccountStatus { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<Adminaction> Adminactions { get; set; } = new List<Adminaction>();

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<Deliverypartner> Deliverypartners { get; set; } = new List<Deliverypartner>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Productapproval> Productapprovals { get; set; } = new List<Productapproval>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Seller> Sellers { get; set; } = new List<Seller>();

    public virtual ICollection<Warehousemanager> Warehousemanagers { get; set; } = new List<Warehousemanager>();
}
