using Ecommerce.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class AdminReportController : ControllerBase
    {

        private readonly ApplicationDbContext _context;



        public AdminReportController(ApplicationDbContext context)
        {
            _context = context;
        }







        // GET: api/AdminReport/Sales

        [HttpGet("Sales")]

        public async Task<IActionResult> SalesReport()
        {


            var report =
                await _context.Orders

                .Where(o =>
                    o.OrderStatus == "Delivered"
                )

                .GroupBy(o =>
                    o.OrderDate.Value.Date
                )

                .Select(g => new
                {

                    date = g.Key,


                    orders =
                        g.Count(),


                    revenue =
                        g.Sum(o =>
                            o.TotalAmount
                        )

                })

                .ToListAsync();




            return Ok(report);


        }









        // GET: api/AdminReport/Orders

        [HttpGet("Orders")]

        public async Task<IActionResult> OrderReport()
        {


            var report =
                await _context.Orders

                .GroupBy(o =>
                    o.OrderStatus
                )

                .Select(g => new
                {

                    status =
                        g.Key,


                    count =
                        g.Count()

                })

                .ToListAsync();



            return Ok(report);


        }



    }

}