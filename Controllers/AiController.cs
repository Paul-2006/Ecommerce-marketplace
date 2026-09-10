using Ecommerce.DTOs;
using Ecommerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AiController : ControllerBase
{
    private readonly IGeminiAiService _aiService;

    public AiController(IGeminiAiService aiService)
    {
        _aiService = aiService;
    }

    /// <summary>
    /// Processes customer natural language queries grounded against database products via Gemini AI API.
    /// </summary>
    [HttpPost("chat")]
    public async Task<IActionResult> ProcessChat([FromBody] AiQueryDTO queryDto)
    {
        if (queryDto == null)
        {
            return BadRequest(new { message = "Query payload is required." });
        }

        var result = await _aiService.ProcessCustomerQueryAsync(queryDto);
        return Ok(result);
    }

    /// <summary>
    /// Compares selected products side-by-side using catalog grounding.
    /// </summary>
    [HttpPost("compare")]
    public async Task<IActionResult> CompareProducts([FromBody] List<int> productIds)
    {
        if (productIds == null || !productIds.Any())
        {
            return BadRequest(new { message = "At least one product ID is required." });
        }

        var result = await _aiService.CompareProductsAsync(productIds);
        return Ok(result);
    }

    /// <summary>
    /// Summarizes customer reviews for a specific product.
    /// </summary>
    [HttpGet("summarize-reviews/{productId}")]
    public async Task<IActionResult> SummarizeReviews(int productId)
    {
        var summary = await _aiService.SummarizeProductReviewsAsync(productId);
        return Ok(new { productId, summary });
    }
}
