using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;


        public SellerController(ApplicationDbContext context)
        {
            _context = context;
        }



        // Create Seller Profile
        // POST: api/Seller
        [HttpPost]
        public async Task<IActionResult> CreateSeller(SellerDTO dto)
        {
            var userExists = await _context.Users
                .AnyAsync(u => u.UserId == dto.UserId);


            if (!userExists)
            {
                return BadRequest("User does not exist");
            }


            var sellerExists = await _context.Sellers
                .AnyAsync(s => s.UserId == dto.UserId);


            if (sellerExists)
            {
                return BadRequest("Seller profile already exists");
            }


            var seller = new Seller
            {
                UserId = dto.UserId,
                BusinessName = dto.BusinessName,
                BusinessAddress = dto.BusinessAddress,
                Gstnumber = dto.Gstnumber,
                ApprovalStatus = "Pending",
                ComplaintCount = 0,
                CreatedDate = DateTime.Now
            };


            _context.Sellers.Add(seller);

            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "Seller profile created",
                sellerId = seller.SellerId
            });
        }




        // Get All Sellers
        // GET: api/Seller
        [HttpGet]
        public async Task<IActionResult> GetSellers()
        {
            var sellers = await _context.Sellers
                .Select(s => new
                {
                    s.SellerId,
                    s.UserId,
                    s.BusinessName,
                    s.BusinessAddress,
                    s.Gstnumber,
                    s.ApprovalStatus
                })
                .ToListAsync();


            return Ok(sellers);
        }





        // Get Seller By Id
        // GET: api/Seller/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSeller(int id)
        {
            var seller = await _context.Sellers
                .FirstOrDefaultAsync(s => s.SellerId == id);


            if (seller == null)
            {
                return NotFound("Seller not found");
            }


            return Ok(seller);
        }





        // Update Seller
        // PUT: api/Seller/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSeller(
            int id,
            SellerDTO dto)
        {
            var seller = await _context.Sellers
                .FindAsync(id);


            if (seller == null)
            {
                return NotFound("Seller not found");
            }


            seller.BusinessName = dto.BusinessName;
            seller.BusinessAddress = dto.BusinessAddress;
            seller.Gstnumber = dto.Gstnumber;


            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "Seller updated successfully"
            });
        }




        // Delete Seller
        // DELETE: api/Seller/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeller(int id)
        {
            var seller = await _context.Sellers
                .FindAsync(id);


            if (seller == null)
            {
                return NotFound("Seller not found");
            }


            _context.Sellers.Remove(seller);

            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "Seller deleted"
            });
        }
    }
}