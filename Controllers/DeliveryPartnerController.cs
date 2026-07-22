using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryPartnerController : ControllerBase
    {

        private readonly ApplicationDbContext _context;


        public DeliveryPartnerController(ApplicationDbContext context)
        {
            _context = context;
        }





        // POST: api/DeliveryPartner/Add

        [HttpPost("Add")]
        public async Task<IActionResult> AddPartner(
            DeliveryPartnerDTO dto)
        {

            var partner = new Deliverypartner
            {

                PartnerName = dto.PartnerName,

                PhoneNumber = dto.PhoneNumber,

                VehicleNumber = dto.VehicleNumber,

                Status = dto.Status,


                // double to decimal conversion
                Latitude = (decimal?)dto.Latitude,

                Longitude = (decimal?)dto.Longitude,


                CreatedDate = DateTime.Now

            };



            _context.Deliverypartners.Add(partner);


            await _context.SaveChangesAsync();



            return Ok(new
            {
                message = "Delivery partner added",

                partnerId = partner.DeliveryPartnerId
            });

        }









        // GET: api/DeliveryPartner

        [HttpGet]
        public async Task<IActionResult> GetPartners()
        {

            var partners =
                await _context.Deliverypartners
                .ToListAsync();


            return Ok(partners);

        }









        // GET: api/DeliveryPartner/Available

        [HttpGet("Available")]
        public async Task<IActionResult> GetAvailablePartners()
        {

            var partners =
                await _context.Deliverypartners

                .Where(p =>
                    p.Status == "Available"
                )

                .ToListAsync();


            return Ok(partners);

        }









        // PUT: api/DeliveryPartner/UpdateStatus/{id}

        [HttpPut("UpdateStatus/{id}")]

        public async Task<IActionResult> UpdateStatus(
            int id,
            string status)
        {

            var partner =
                await _context.Deliverypartners
                .FindAsync(id);



            if (partner == null)
            {
                return NotFound(
                    "Partner not found"
                );
            }



            partner.Status = status;



            await _context.SaveChangesAsync();



            return Ok(new
            {
                message = "Partner status updated"
            });

        }









        // PUT: api/DeliveryPartner/Location/{id}

        [HttpPut("Location/{id}")]

        public async Task<IActionResult> UpdateLocation(
            int id,
            double latitude,
            double longitude)
        {

            var partner =
                await _context.Deliverypartners
                .FindAsync(id);



            if (partner == null)
            {
                return NotFound(
                    "Partner not found"
                );
            }




            partner.Latitude = (decimal?)latitude;


            partner.Longitude = (decimal?)longitude;



            await _context.SaveChangesAsync();



            return Ok(new
            {
                message = "Location updated"
            });

        }









        // DELETE: api/DeliveryPartner/{id}

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeletePartner(
            int id)
        {


            var partner =
                await _context.Deliverypartners
                .FindAsync(id);



            if (partner == null)
            {
                return NotFound(
                    "Partner not found"
                );
            }




            _context.Deliverypartners.Remove(partner);



            await _context.SaveChangesAsync();



            return Ok(new
            {
                message = "Delivery partner deleted"
            });

        }


    }
}