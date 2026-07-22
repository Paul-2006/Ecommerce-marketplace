namespace Ecommerce.DTOs;

public class SellerStatusDTO
{
    public int SellerId { get; set; }

    public int AdminId { get; set; }

    public string? NewStatus { get; set; }

    public string? Reason { get; set; }
}