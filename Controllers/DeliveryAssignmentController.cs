using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Ecommerce.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class DeliveryAssignmentController : ControllerBase
    {

        private readonly ApplicationDbContext _context;



        public DeliveryAssignmentController(ApplicationDbContext context)
        {
            _context = context;
        }







        // POST: api/DeliveryAssignment/Assign

        [HttpPost("Assign")]

        public async Task<IActionResult> AssignDelivery(
            DeliveryAssignmentDTO dto)
        {


            var orderExists =
                await _context.Orders

                .AnyAsync(o =>
                    o.OrderId == dto.OrderId
                );



            if (!orderExists)
            {
                return BadRequest(
                    "Order not found"
                );
            }






            var partnerExists =
                await _context.Deliverypartners

                .AnyAsync(p =>
                    p.DeliveryPartnerId ==
                    dto.DeliveryPartnerId
                );



            if (!partnerExists)
            {
                return BadRequest(
                    "Delivery partner not found"
                );
            }







            var assignment =
                new Deliveryassignment
                {

                    OrderId = dto.OrderId,


                    DeliveryPartnerId =
                        dto.DeliveryPartnerId,


                    Status = dto.Status,


                    AssignedDate = DateTime.Now


                };





            _context.Deliveryassignments
                .Add(assignment);



            await _context.SaveChangesAsync();





            return Ok(new
            {

                message =
                "Delivery assigned successfully",


                assignmentId =
                assignment.DeliveryAssignmentId


            });


        }









        // GET: api/DeliveryAssignment/Partner/1

        [HttpGet("Partner/{partnerId}")]

        public async Task<IActionResult> GetPartnerDeliveries(
            int partnerId)
        {



            var deliveries =
                await _context.Deliveryassignments

                .Where(d =>
                    d.DeliveryPartnerId == partnerId
                )

                .Include(d =>
                    d.Order
                )

                .Select(d => new
                {

                    assignmentId =
                        d.DeliveryAssignmentId,


                    orderId =
                        d.OrderId,


                    status =
                        d.Status,


                    assignedDate =
                        d.AssignedDate,


                    totalAmount =
                        d.Order.TotalAmount


                })

                .ToListAsync();




            return Ok(deliveries);


        }









        // PUT: api/DeliveryAssignment/Update/1

        [HttpPut("Update/{id}")]

        public async Task<IActionResult> UpdateDeliveryStatus(
            int id,
            string status)
        {



            var delivery =
                await _context.Deliveryassignments

                .FindAsync(id);



            if (delivery == null)
            {
                return NotFound(
                    "Delivery assignment not found"
                );
            }




            delivery.Status = status;



            await _context.SaveChangesAsync();




            return Ok(new
            {

                message =
                "Delivery status updated",

                status = status

            });


        }



    }

}