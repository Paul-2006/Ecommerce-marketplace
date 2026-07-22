using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;


        public TrackingController(ApplicationDbContext context)
        {
            _context = context;
        }



        // Add Location
        // POST api/Tracking/Add
        [HttpPost("Add")]
        public async Task<IActionResult> AddLocation(
            LocationTrackingDTO dto)
        {

            var partnerExists =
                await _context.Deliverypartners
                .AnyAsync(d =>
                d.DeliveryPartnerId == dto.DeliveryPartnerId);


            if (!partnerExists)
            {
                return BadRequest(
                    "Delivery partner not found");
            }



            var location = new Deliverylocationtracking
            {
                DeliveryPartnerId = dto.DeliveryPartnerId,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                RecordedTime = DateTime.Now
            };


            _context.Deliverylocationtrackings.Add(location);

            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "Location updated",
                locationId = location.LocationId
            });
        }





        // Get Partner Location History
        // GET api/Tracking/Partner/1
        [HttpGet("Partner/{partnerId}")]
        public async Task<IActionResult> GetLocations(
            int partnerId)
        {

            var locations =
                await _context.Deliverylocationtrackings
                .Where(l =>
                l.DeliveryPartnerId == partnerId)
                .ToListAsync();


            return Ok(locations);
        }
    }
}