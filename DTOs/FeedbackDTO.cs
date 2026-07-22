namespace Ecommerce.DTOs;

public class FeedbackDTO
{
    public int CustomerId { get; set; }

    public int? OrderId { get; set; }

    public int? SellerId { get; set; }

    public string? FeedbackType { get; set; }

    public int? Rating { get; set; }

    public string? ComplaintDescription { get; set; }
}