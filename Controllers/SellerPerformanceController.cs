using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SellerPerformanceController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SellerPerformanceController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/SellerPerformance
    [HttpGet]
    public async Task<IActionResult> GetPerformance()
    {
        var performance = await _context.Sellerperformances
            .Include(p => p.Seller)
            .Select(p => new
            {
                performanceId = p.PerformanceId,
                sellerId = p.SellerId,
                totalOrders = p.TotalOrders,
                completedOrders = p.CompletedOrders,
                failedDeliveries = p.FailedDeliveries,
                averageRating = p.AverageRating,
                totalComplaints = p.TotalComplaints,
                lastUpdated = p.LastUpdated
            })
            .ToListAsync();

        return Ok(performance);
    }

    // GET: api/SellerPerformance/Seller/1
    [HttpGet("Seller/{sellerId}")]
    public async Task<IActionResult> GetSellerPerformance(int sellerId)
    {
        var performance = await _context.Sellerperformances
            .FirstOrDefaultAsync(p => p.SellerId == sellerId);

        if (performance == null)
            return NotFound("Performance record not found");

        return Ok(performance);
    }

    // POST: api/SellerPerformance/Add
    [HttpPost("Add")]
    public async Task<IActionResult> AddPerformance(Sellerperformance performance)
    {
        performance.LastUpdated = DateTime.Now;

        _context.Sellerperformances.Add(performance);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Performance added",
            id = performance.PerformanceId
        });
    }

    // PUT: api/SellerPerformance/Update/1
    [HttpPut("Update/{id}")]
    public async Task<IActionResult> UpdatePerformance(int id, Sellerperformance data)
    {
        var performance = await _context.Sellerperformances.FindAsync(id);

        if (performance == null)
            return NotFound("Performance not found");

        performance.TotalOrders = data.TotalOrders;
        performance.CompletedOrders = data.CompletedOrders;
        performance.FailedDeliveries = data.FailedDeliveries;
        performance.AverageRating = data.AverageRating;
        performance.TotalComplaints = data.TotalComplaints;
        performance.LastUpdated = DateTime.Now;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Performance updated" });
    }

    // DELETE: api/SellerPerformance/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePerformance(int id)
    {
        var performance = await _context.Sellerperformances.FindAsync(id);

        if (performance == null)
            return NotFound("Performance not found");

        _context.Sellerperformances.Remove(performance);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Performance deleted" });
    }
}