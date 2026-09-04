using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductComparisonController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductComparisonController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("Compare")]
    public async Task<IActionResult> Compare(ProductCompareRequestDTO dto)
    {
        var productIds = dto.ProductIds?.Distinct().Take(4).ToList() ?? new List<int>();
        var query = dto.Query?.Trim() ?? "";

        var productsQuery = _context.Products
            .Include(p => p.Sellerproducts)
            .Include(p => p.Productreviews)
            .Include(p => p.Productspecifications)
            .Include(p => p.Productimages)
            .AsQueryable();

        if (productIds.Count > 0)
        {
            productsQuery = productsQuery.Where(p => productIds.Contains(p.ProductId));
        }
        else if (!string.IsNullOrWhiteSpace(query))
        {
            productsQuery = productsQuery.Where(p =>
                p.ProductName.Contains(query) ||
                (p.Brand != null && p.Brand.Contains(query)) ||
                (p.Description != null && p.Description.Contains(query)));
        }
        else
        {
            return BadRequest("Select products or enter a product name to compare.");
        }

        var products = await productsQuery.Take(4).ToListAsync();

        if (products.Count == 0)
        {
            return NotFound("No matching products found.");
        }

        if (dto.CustomerId.HasValue)
        {
            var existingProductIds = await _context.Productcomparisons
                .Where(c => c.CustomerId == dto.CustomerId.Value)
                .Select(c => c.ProductId)
                .ToListAsync();

            foreach (var product in products.Where(p => !existingProductIds.Contains(p.ProductId)))
            {
                _context.Productcomparisons.Add(new Productcomparison
                {
                    CustomerId = dto.CustomerId.Value,
                    ProductId = product.ProductId,
                    AddedDate = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
        }

        var comparedProducts = products.Select(p =>
        {
            var offers = p.Sellerproducts.Where(sp => sp.ProductStatus != "Blocked").ToList();
            var bestOffer = offers.OrderBy(sp => sp.Price).FirstOrDefault();
            var ratings = p.Productreviews
                .Where(r => r.Rating.HasValue)
                .Select(r => r.Rating!.Value)
                .ToList();
            var averageRating = ratings.Count == 0
                ? 0
                : Math.Round(ratings.Average(), 1);

            return new
            {
                productId = p.ProductId,
                productName = p.ProductName,
                brand = p.Brand,
                description = p.Description,
                warranty = p.Warranty,
                image = p.Productimages.Select(i => i.ImageUrl).FirstOrDefault(),
                bestPrice = bestOffer?.Price,
                totalStock = offers.Sum(sp => sp.StockQuantity ?? 0),
                sellers = offers.Count,
                averageRating,
                specifications = p.Productspecifications.Select(s => new
                {
                    s.SpecificationName,
                    s.SpecificationValue
                })
            };
        }).ToList();

        var recommendation = comparedProducts
            .OrderByDescending(p => p.averageRating)
            .ThenBy(p => p.bestPrice ?? decimal.MaxValue)
            .ThenByDescending(p => p.totalStock)
            .First();

        return Ok(new
        {
            message = $"Best match: {recommendation.productName}. It has rating {recommendation.averageRating}, price {recommendation.bestPrice}, and stock {recommendation.totalStock}.",
            products = comparedProducts
        });
    }
}
