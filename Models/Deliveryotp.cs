using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Deliveryotp
{
    public int Otpid { get; set; }

    public int OrderId { get; set; }

    public string? Otpcode { get; set; }

    public DateTime? GeneratedTime { get; set; }

    public DateTime? ExpiryTime { get; set; }

    public string? Otpstatus { get; set; }

    public string? OTP { get; set; }

    public bool? IsVerified { get; set; }

    public DateTime? CreatedDate { get; set; }
    public virtual Order Order { get; set; } = null!;
}
