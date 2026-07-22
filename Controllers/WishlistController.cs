using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Ecommerce.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class WishlistController : ControllerBase
    {

        private readonly ApplicationDbContext _context;



        public WishlistController(ApplicationDbContext context)
        {
            _context = context;
        }







        // POST: api/Wishlist/Add

        [HttpPost("Add")]

        public async Task<IActionResult> AddToWishlist(
            WishlistDTO dto)
        {


            var wishlist = await _context.Wishlists

                .FirstOrDefaultAsync(
                    w => w.CustomerId == dto.CustomerId
                );



            if (wishlist == null)
            {

                wishlist = new Wishlist
                {

                    CustomerId = dto.CustomerId,

                    CreatedDate = DateTime.Now

                };


                _context.Wishlists.Add(wishlist);


                await _context.SaveChangesAsync();

            }






            var exists = await _context.Wishlistitems

                .AnyAsync(w =>
                    w.WishlistId == wishlist.WishlistId &&
                    w.ProductId == dto.ProductId
                );



            if (exists)
            {

                return BadRequest(
                    "Product already in wishlist"
                );

            }






            var item = new Wishlistitem
            {

                WishlistId = wishlist.WishlistId,


                ProductId = dto.ProductId

            };



            _context.Wishlistitems.Add(item);



            await _context.SaveChangesAsync();



            return Ok(new
            {

                message = "Product added to wishlist",

                wishlistItemId = item.WishlistItemId

            });


        }









        // GET: api/Wishlist/Customer/1

        [HttpGet("Customer/{customerId}")]

        public async Task<IActionResult> GetWishlist(
            int customerId)
        {


            var wishlist = await _context.Wishlists

                .Include(w => w.Wishlistitems)
                .ThenInclude(wi => wi.Product)

                .FirstOrDefaultAsync(
                    w => w.CustomerId == customerId
                );




            if (wishlist == null)
            {
                return Ok(new List<object>());
            }





            var products = wishlist.Wishlistitems

                .Select(w => new
                {

                    wishlistItemId = w.WishlistItemId,

                    productId = w.ProductId,

                    productName = w.Product.ProductName,

                    description = w.Product.Description,

                    brand = w.Product.Brand

                })

                .ToList();




            return Ok(products);


        }









        // DELETE: api/Wishlist/Remove/1

        [HttpDelete("Remove/{id}")]

        public async Task<IActionResult> RemoveWishlistItem(
            int id)
        {


            var item = await _context.Wishlistitems

                .FindAsync(id);



            if (item == null)
            {
                return NotFound(
                    "Wishlist item not found"
                );
            }




            _context.Wishlistitems.Remove(item);



            await _context.SaveChangesAsync();



            return Ok(new
            {

                message = "Removed from wishlist"

            });


        }



    }

}