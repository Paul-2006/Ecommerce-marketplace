using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class CustomerAddressController : ControllerBase
    {

        private readonly ApplicationDbContext _context;


        public CustomerAddressController(ApplicationDbContext context)
        {
            _context = context;
        }



        // POST: api/CustomerAddress

        [HttpPost]

        public async Task<IActionResult> AddAddress(CustomerAddressDTO dto)
        {

            var address = new Customeraddress
            {

                CustomerId = dto.CustomerId,

                FullName = dto.FullName,

                PhoneNumber = dto.PhoneNumber,

                AddressLine = dto.AddressLine,

                City = dto.City,

                State = dto.State,

                Pincode = dto.Pincode,

                IsDefault = dto.IsDefault

            };


            _context.Customeraddresses.Add(address);


            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "Address added successfully",
                addressId = address.AddressId
            });

        }





        // GET: api/CustomerAddress/Customer/1

        [HttpGet("Customer/{customerId}")]

        public async Task<IActionResult> GetCustomerAddresses(int customerId)
        {

            var addresses = await _context.Customeraddresses

                .Where(a => a.CustomerId == customerId)

                .ToListAsync();


            return Ok(addresses);

        }





        // GET: api/CustomerAddress/1

        [HttpGet("{id}")]

        public async Task<IActionResult> GetAddress(int id)
        {

            var address = await _context.Customeraddresses
                .FindAsync(id);


            if (address == null)
            {
                return NotFound("Address not found");
            }


            return Ok(address);

        }





        // PUT: api/CustomerAddress/1

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateAddress(
            int id,
            CustomerAddressDTO dto)
        {


            var address = await _context.Customeraddresses
                .FindAsync(id);



            if (address == null)
            {
                return NotFound("Address not found");
            }



            address.FullName = dto.FullName;

            address.PhoneNumber = dto.PhoneNumber;

            address.AddressLine = dto.AddressLine;

            address.City = dto.City;

            address.State = dto.State;

            address.Pincode = dto.Pincode;

            address.IsDefault = dto.IsDefault;



            await _context.SaveChangesAsync();



            return Ok(new
            {
                message = "Address updated successfully"
            });

        }





        // DELETE: api/CustomerAddress/1

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteAddress(int id)
        {


            var address = await _context.Customeraddresses
                .FindAsync(id);



            if (address == null)
            {
                return NotFound("Address not found");
            }



            _context.Customeraddresses.Remove(address);


            await _context.SaveChangesAsync();



            return Ok(new
            {
                message = "Address deleted successfully"
            });

        }



    }

}