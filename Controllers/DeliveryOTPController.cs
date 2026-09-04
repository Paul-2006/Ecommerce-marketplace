using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Ecommerce.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class DeliveryOTPController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;

        public DeliveryOTPController(
            ApplicationDbContext context,
            IOtpService otpService,
            IEmailService emailService)
        {
            _context = context;
            _otpService = otpService;
            _emailService = emailService;
        }








        // POST: api/DeliveryOTP/Generate

        [HttpPost("Generate")]

        public async Task<IActionResult> GenerateOTP(
            int orderId)
        {



            var order = await _context.Orders
                .Include(o => o.Customer)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return BadRequest("Order not found");
            }

            string customerEmail = order.Customer?.User?.Email ?? "customer@example.com";
            string customerName = order.Customer?.FirstName ?? "Customer";

            var (success, message, otpCode) = await _otpService.GenerateOtpAsync(customerEmail, $"DeliveryOrder-{orderId}");
            if (!success)
            {
                return BadRequest(message);
            }

            // Also persist record in DB for auditing
            var deliveryOTP = new Deliveryotp
            {
                OrderId = orderId,
                OTP = "PROTECTED_HASH",
                IsVerified = false,
                CreatedDate = DateTime.Now
            };

            _context.Deliveryotps.Add(deliveryOTP);
            await _context.SaveChangesAsync();

            await _emailService.SendOtpEmailAsync(customerEmail, customerName, otpCode);

            return Ok(new
            {
                message = "Delivery verification OTP dispatched to customer email address."
            });


        }









        // POST: api/DeliveryOTP/Verify
        [HttpPost("Verify")]
        public async Task<IActionResult> VerifyOTP(DeliveryOTPDTO dto)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(o => o.OrderId == dto.OrderId);

            if (order == null)
            {
                return BadRequest("Order not found");
            }

            string customerEmail = order.Customer?.User?.Email ?? "customer@example.com";
            var (success, message) = await _otpService.VerifyOtpAsync(customerEmail, dto.OTP, $"DeliveryOrder-{dto.OrderId}");

            if (!success)
            {
                // Fallback check against legacy db records if present
                var legacyRecord = await _context.Deliveryotps
                    .FirstOrDefaultAsync(o => o.OrderId == dto.OrderId && o.OTP == dto.OTP && o.IsVerified != true);
                
                if (legacyRecord == null)
                {
                    return BadRequest(message);
                }

                legacyRecord.IsVerified = true;
            }

            order.OrderStatus = "Delivered";
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "OTP verified. Order status updated to Delivered."
            });
        }






    }

}