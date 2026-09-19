using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "DeliveryOnly")]
    public class DeliveryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;


        public DeliveryController(ApplicationDbContext context)
        {
            _context = context;
        }



        // Assign Delivery Partner
        // POST: api/Delivery/Assign
        [HttpPost("Assign")]
        public async Task<IActionResult> AssignDelivery(
            DeliveryAssignmentDTO dto)
        {

            var orderExists = await _context.Orders
                .AnyAsync(o => o.OrderId == dto.OrderId);


            if (!orderExists)
            {
                return BadRequest("Order not found");
            }



            var partnerExists = await _context.Deliverypartners
                .AnyAsync(d => d.DeliveryPartnerId == dto.DeliveryPartnerId);


            if (!partnerExists)
            {
                return BadRequest("Delivery partner not found");
            }



            var delivery = new Deliveryassignment
            {
                OrderId = dto.OrderId,
                DeliveryPartnerId = dto.DeliveryPartnerId,
                AssignedDate = DateTime.Now,
                DeliveryStatus = "Assigned"
            };


            _context.Deliveryassignments.Add(delivery);

            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "Delivery assigned successfully",
                deliveryAssignmentId = delivery.DeliveryAssignmentId
            });
        }





        // Get Delivery Details By Order
        // GET: api/Delivery/Order/1
        [HttpGet("Order/{orderId}")]
        public async Task<IActionResult> GetOrderDelivery(
            int orderId)
        {

            var delivery = await _context.Deliveryassignments
                .Where(d => d.OrderId == orderId)
                .Select(d => new
                {
                    d.DeliveryAssignmentId,
                    d.OrderId,
                    d.DeliveryPartnerId,
                    d.DeliveryStatus,
                    d.AssignedDate,

                    PartnerName = d.DeliveryPartner.User.Username
                })
                .FirstOrDefaultAsync();


            if (delivery == null)
            {
                return NotFound("Delivery not found");
            }


            return Ok(delivery);
        }





        // Update Delivery Status
        // PUT: api/Delivery/Status/1
        [HttpPut("Status/{id}")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            DeliveryStatusDTO dto)
        {

            var delivery =
                await _context.Deliveryassignments
                .FindAsync(id);


            if (delivery == null)
            {
                return NotFound("Delivery assignment not found");
            }


            delivery.DeliveryStatus = dto.DeliveryStatus;


            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "Delivery status updated"
            });
        }





        // Get Deliveries Of Partner
        // GET: api/Delivery/Partner/1
        [HttpGet("Partner/{partnerId}")]
        public async Task<IActionResult> GetPartnerDeliveries(
            int partnerId)
        {

            var deliveries = await _context.Deliveryassignments
                .Where(d => d.DeliveryPartnerId == partnerId)
                .ToListAsync();


            return Ok(deliveries);
        }
    }
}