namespace Ecommerce.DTOs;

public class AiQueryDTO
{
    public string Query { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public decimal? MaxPrice { get; set; }
    public List<int>? PreferredProductIds { get; set; }
    public List<ChatMessageDTO>? History { get; set; }
}

public class ChatMessageDTO
{
    public string Sender { get; set; } = "user"; // "user" or "assistant"
    public string Text { get; set; } = string.Empty;
}
