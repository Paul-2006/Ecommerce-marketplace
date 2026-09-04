using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> UpdateLocation(DeliveryLocationDTO dto)
        {
            var partnerExists = await _context.Deliverypartners
                .AnyAsync(p => p.DeliveryPartnerId == dto.DeliveryPartnerId);

            if (!partnerExists)
            {
                return BadRequest("Delivery partner not found");
            }

            var location = new Deliverylocationtracking
            {
                DeliveryPartnerId = dto.DeliveryPartnerId,
                OrderId = dto.OrderId,
                Latitude = (decimal)dto.Latitude,
                Longitude = (decimal)dto.Longitude,
                TrackingTime = DateTime.Now
            };

            _context.Deliverylocationtrackings.Add(location);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Location updated successfully" });
        }

        // GET: api/DeliveryLocation/Order/1
        [HttpGet("Order/{orderId}")]
        public async Task<IActionResult> GetOrderLocation(int orderId)
        {
            var locations = await _context.Deliverylocationtrackings
                .Where(l => l.OrderId == orderId)
                .OrderByDescending(l => l.TrackingTime)
                .ToListAsync();

            return Ok(locations);
        }

        // GET: api/DeliveryLocation/Latest/1
        [HttpGet("Latest/{orderId}")]
        public async Task<IActionResult> GetLatestLocation(int orderId)
        {
            var location = await _context.Deliverylocationtrackings
                .Where(l => l.OrderId == orderId)
                .OrderByDescending(l => l.TrackingTime)
                .FirstOrDefaultAsync();

            if (location == null)
            {
                return NotFound("Location not available");
            }

            return Ok(location);
        }

        // POST: api/DeliveryLocation/ValidateDistance
        [HttpPost("ValidateDistance")]
        public IActionResult ValidateDeliveryDistance([FromBody] DistanceValidationDTO dto)
        {
            if (dto == null) return BadRequest("Invalid location payload");

            double distanceKm = CalculateHaversineDistanceKm(dto.CustomerLat, dto.CustomerLng, dto.HubLat, dto.HubLng);
            bool isEligible = distanceKm <= 10.0;

            if (!isEligible)
            {
                return BadRequest(new
                {
                    isEligible = false,
                    distanceKm = Math.Round(distanceKm, 2),
                    message = $"Delivery location ({Math.Round(distanceKm, 2)} km) exceeds maximum allowed 10 km radius from fulfillment hub."
                });
            }

            return Ok(new
            {
                isEligible = true,
                distanceKm = Math.Round(distanceKm, 2),
                message = $"Delivery location is within valid 10 km radius ({Math.Round(distanceKm, 2)} km)."
            });
        }

        public static double CalculateHaversineDistanceKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371.0;
            double dLat = (lat2 - lat1) * (Math.PI / 180.0);
            double dLon = (lon2 - lon1) * (Math.PI / 180.0);
            double a = Math.Sin(dLat / 2.0) * Math.Sin(dLat / 2.0) +
                       Math.Cos(lat1 * (Math.PI / 180.0)) * Math.Cos(lat2 * (Math.PI / 180.0)) *
                       Math.Sin(dLon / 2.0) * Math.Sin(dLon / 2.0);
            double c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
            return R * c;
        }
    }

    public class DistanceValidationDTO
    {
        public double CustomerLat { get; set; }
        public double CustomerLng { get; set; }
        public double HubLat { get; set; } = 12.9716;
        public double HubLng { get; set; } = 77.5946;
    }
}