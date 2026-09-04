namespace Ecommerce.DTOs;

public class ProductCompareRequestDTO
{
    public int? CustomerId { get; set; }

    public List<int>? ProductIds { get; set; }

    public string? Query { get; set; }
}
