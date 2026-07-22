using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Ecommerce.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class DeliveryOTPController : ControllerBase
    {

        private readonly ApplicationDbContext _context;


        public DeliveryOTPController(ApplicationDbContext context)
        {
            _context = context;
        }








        // POST: api/DeliveryOTP/Generate

        [HttpPost("Generate")]

        public async Task<IActionResult> GenerateOTP(
            int orderId)
        {



            var orderExists =
                await _context.Orders

                .AnyAsync(o =>
                    o.OrderId == orderId
                );



            if (!orderExists)
            {
                return BadRequest(
                    "Order not found"
                );
            }







            var otp =
                new Random()
                .Next(100000, 999999)
                .ToString();





            var deliveryOTP =
                new Deliveryotp
                {

                    OrderId = orderId,


                    OTP = otp,


                    IsVerified = false,


                    CreatedDate = DateTime.Now

                };





            _context.Deliveryotps
                .Add(deliveryOTP);



            await _context.SaveChangesAsync();




            return Ok(new
            {

                message =
                "OTP generated successfully",


                otp = otp

            });


        }









        // POST: api/DeliveryOTP/Verify

        [HttpPost("Verify")]

        public async Task<IActionResult> VerifyOTP(
            DeliveryOTPDTO dto)
        {



            var otpRecord =
                await _context.Deliveryotps

                .FirstOrDefaultAsync(o =>
                    o.OrderId == dto.OrderId &&
                    o.OTP == dto.OTP
                );




            if (otpRecord == null)
            {
                return BadRequest(
                    "Invalid OTP"
                );
            }





            otpRecord.IsVerified = true;



            var order =
                await _context.Orders
                .FindAsync(dto.OrderId);



            if (order != null)
            {
                order.OrderStatus =
                    "Delivered";
            }





            await _context.SaveChangesAsync();




            return Ok(new
            {

                message =
                "OTP verified. Order delivered."

            });


        }






    }

}