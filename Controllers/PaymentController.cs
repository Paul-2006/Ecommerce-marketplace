using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Ecommerce.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class PaymentController : ControllerBase
    {

        private readonly ApplicationDbContext _context;


        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }



        // POST api/Payment/Create

        [HttpPost("Create")]

        public async Task<IActionResult> CreatePayment(
            PaymentDTO dto)
        {


            var order = await _context.Orders
                .FirstOrDefaultAsync(
                    o => o.OrderId == dto.OrderId
                );


            if (order == null)
            {
                return NotFound("Order not found");
            }



            var payment = new Payment
            {

                OrderId = dto.OrderId,

                PaymentMethod = dto.PaymentMethod,

                TransactionId = dto.TransactionId,

                PaymentStatus = "Completed",

                PaymentDate = DateTime.Now

            };



            _context.Payments.Add(payment);



            order.OrderStatus = "Confirmed";


            await _context.SaveChangesAsync();



            return Ok(new
            {
                message = "Payment completed",
                paymentId = payment.PaymentId
            });

        }




        // GET api/Payment/Order/1

        [HttpGet("Order/{orderId}")]

        public async Task<IActionResult> GetPayment(
            int orderId)
        {

            var payment =
                await _context.Payments
                .FirstOrDefaultAsync(
                    p => p.OrderId == orderId
                );


            if (payment == null)
            {
                return NotFound(
                    "Payment not found"
                );
            }


            return Ok(payment);

        }

    }

}