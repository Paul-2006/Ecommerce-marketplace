using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class DeliveryLocationController : ControllerBase
    {

        private readonly ApplicationDbContext _context;


        public DeliveryLocationController(ApplicationDbContext context)
        {
            _context = context;
        }





        // POST: api/DeliveryLocation/Update

        [HttpPost("Update")]

        public async Task<IActionResult> UpdateLocation(
            DeliveryLocationDTO dto)
        {


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





            var location = new Deliverylocationtracking
            {

                DeliveryPartnerId =
                    dto.DeliveryPartnerId,


                OrderId =
                    dto.OrderId,


                Latitude =
                    (decimal)dto.Latitude,


                Longitude =
                    (decimal)dto.Longitude,


                TrackingTime =
                    DateTime.Now

            };





            _context.Deliverylocationtrackings.Add(location);


            await _context.SaveChangesAsync();




            return Ok(new
            {

                message =
                "Location updated successfully"

            });

        }









        // GET: api/DeliveryLocation/Order/1

        [HttpGet("Order/{orderId}")]

        public async Task<IActionResult> GetOrderLocation(
            int orderId)
        {


            var locations =
                await _context.Deliverylocationtrackings

                .Where(l =>
                    l.OrderId == orderId
                )

                .OrderByDescending(l =>
                    l.TrackingTime
                )

                .ToListAsync();



            return Ok(locations);

        }











        // GET: api/DeliveryLocation/Latest/1

        [HttpGet("Latest/{orderId}")]

        public async Task<IActionResult> GetLatestLocation(
            int orderId)
        {


            var location =
                await _context.Deliverylocationtrackings

                .Where(l =>
                    l.OrderId == orderId
                )

                .OrderByDescending(l =>
                    l.TrackingTime
                )

                .FirstOrDefaultAsync();





            if (location == null)
            {
                return NotFound(
                    "Location not available"
                );
            }



            return Ok(location);

        }



    }

}