using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Pending")]
        public async Task<IActionResult> GetPendingProducts()
        {
            var products = await _context.Products
                .Where(p => p.ApprovalStatus == "Pending")
                .ToListAsync();

            return Ok(products);
        }

        [HttpPut("Approve")]
        public async Task<IActionResult> ApproveProduct(ProductApprovalDTO dto)
        {
            var product = await _context.Products.FindAsync(dto.ProductId);

            if (product == null)
            {
                return NotFound("Product not found");
            }

            product.ApprovalStatus = dto.ApprovalStatus;

            var approval = new Productapproval
            {
                ProductId = dto.ProductId,
                AdminId = dto.AdminId,
                ApprovalStatus = dto.ApprovalStatus,
                Remarks = dto.Remarks,
                ApprovalDate = DateTime.Now
            };

            _context.Productapprovals.Add(approval);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Product approval updated",
                status = dto.ApprovalStatus
            });
        }

        [HttpGet("History/{productId}")]
        public async Task<IActionResult> GetApprovalHistory(int productId)
        {
            var history = await _context.Productapprovals
                .Where(p => p.ProductId == productId)
                .OrderByDescending(p => p.ApprovalDate)
                .ToListAsync();

            return Ok(history);
        }
    }
}
