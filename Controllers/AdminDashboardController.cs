using Ecommerce.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Ecommerce.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class AdminDashboardController : ControllerBase
    {

        private readonly ApplicationDbContext _context;


        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }







        // GET: api/AdminDashboard/Stats

        [HttpGet("Stats")]

        public async Task<IActionResult> GetDashboardStats()
        {


            var totalCustomers =
                await _context.Customers
                .CountAsync();




            var totalSellers =
                await _context.Sellers
                .CountAsync();




            var totalProducts =
                await _context.Products
                .CountAsync();




            var totalOrders =
                await _context.Orders
                .CountAsync();






            var totalRevenue =
                await _context.Orders

                .Where(o =>
                    o.OrderStatus == "Delivered"
                )

                .SumAsync(o =>
                    o.TotalAmount
                );






            var pendingProducts =
                await _context.Products

                .CountAsync(p =>
                    p.ApprovalStatus == "Pending"
                );






            var pendingSellers =
                await _context.Sellers

                .CountAsync(s =>
                    s.Status == "Pending"
                );







            return Ok(new
            {

                customers =
                    totalCustomers,


                sellers =
                    totalSellers,


                products =
                    totalProducts,


                orders =
                    totalOrders,


                revenue =
                    totalRevenue,


                pendingProducts =
                    pendingProducts,


                pendingSellers =
                    pendingSellers


            });


        }



    }

}