using Ecommerce.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Summary")]
        public async Task<IActionResult> GetSummary()
        {
            var today = DateTime.Today;

            var totalUsers = await _context.Users.CountAsync();
            var totalCustomers = await _context.Customers.CountAsync();
            var totalSellers = await _context.Sellers.CountAsync();
            var pendingSellers = await _context.Sellers.CountAsync(s =>
                s.Status == "Pending" || s.ApprovalStatus == "Pending");
            var totalProducts = await _context.Products.CountAsync();
            var pendingProducts = await _context.Products.CountAsync(p =>
                p.ApprovalStatus == "Pending");
            var totalOrders = await _context.Orders.CountAsync();
            var deliveredOrders = await _context.Orders.CountAsync(o =>
                o.OrderStatus == "Delivered");
            var todayOrders = await _context.Orders.CountAsync(o =>
                o.OrderDate.HasValue && o.OrderDate.Value.Date == today);
            var totalRevenue = await _context.Orders
                .Where(o => o.OrderStatus == "Delivered")
                .SumAsync(o => o.TotalAmount ?? 0);
            var lowStockProducts = await _context.Sellerproducts
                .CountAsync(sp => (sp.StockQuantity ?? 0) <= 5);

            return Ok(new
            {
                totalUsers,
                totalCustomers,
                totalSellers,
                pendingSellers,
                totalProducts,
                pendingProducts,
                totalOrders,
                deliveredOrders,
                todayOrders,
                totalRevenue,
                lowStockProducts
            });
        }

        [HttpGet("RecentOrders")]
        public async Task<IActionResult> GetRecentOrders()
        {
            var orders = await _context.Orders
                .AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .Select(o => new
                {
                    orderId = o.OrderId,
                    customer = o.Customer.User.Username,
                    status = o.OrderStatus,
                    totalAmount = o.TotalAmount,
                    orderDate = o.OrderDate
                })
                .ToListAsync();

            return Ok(orders);
        }
    }
}
