using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class OrderStatusController : ControllerBase
    {

        private readonly ApplicationDbContext _context;


        public OrderStatusController(ApplicationDbContext context)
        {
            _context = context;
        }




        // POST: api/OrderStatus/Add

        [HttpPost("Add")]

        public async Task<IActionResult> AddStatus(OrderStatusDTO dto)
        {


            var order = await _context.Orders
                .FindAsync(dto.OrderId);



            if (order == null)
            {
                return NotFound("Order not found");
            }





            var history = new Orderstatushistory
            {


                OrderId = dto.OrderId,


                Status = dto.Status,


                Remarks = dto.Remarks,


                CreatedDate = DateTime.Now


            };



            _context.Orderstatushistories.Add(history);



            // Update current order status

            order.OrderStatus = dto.Status;



            await _context.SaveChangesAsync();



            return Ok(new
            {

                message = "Order status updated successfully",

                status = dto.Status

            });


        }








        // GET: api/OrderStatus/Order/1

        [HttpGet("Order/{orderId}")]

        public async Task<IActionResult> GetOrderHistory(int orderId)
        {


            var history = await _context.Orderstatushistories

                .Where(o => o.OrderId == orderId)

                .OrderBy(o => o.CreatedDate)

                .ToListAsync();



            return Ok(history);


        }








        // PUT: api/OrderStatus/Update/1

        [HttpPut("Update/{orderId}")]

        public async Task<IActionResult> UpdateStatus(
            int orderId,
            string status)
        {


            var order = await _context.Orders
                .FindAsync(orderId);



            if (order == null)
            {
                return NotFound("Order not found");
            }




            order.OrderStatus = status;



            var history = new Orderstatushistory
            {

                OrderId = orderId,

                Status = status,

                Remarks = "Status changed",

                CreatedDate = DateTime.Now

            };



            _context.Orderstatushistories.Add(history);



            await _context.SaveChangesAsync();



            return Ok(new
            {

                message = "Order status changed"

            });


        }



    }

}