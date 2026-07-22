namespace Ecommerce.DTOs;

public class PaymentDTO
{
    public int OrderId { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string? TransactionId { get; set; }
}