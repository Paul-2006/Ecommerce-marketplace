using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }


        // POST: api/Cart/AddItem

        [HttpPost("AddItem")]
        public async Task<IActionResult> AddItem(CartItemDTO dto)
        {

            var cartExists = await _context.Carts
                .AnyAsync(c => c.CartId == dto.CartId);


            if (!cartExists)
            {
                return BadRequest("Cart does not exist");
            }


            // Check Product exists

            var productExists = await _context.Products
                .AnyAsync(p => p.ProductId == dto.ProductId);


            if (!productExists)
            {
                return BadRequest("Product does not exist");
            }



            // Check existing cart item

            var existingItem = await _context.Cartitems
                .FirstOrDefaultAsync(c =>
                    c.CartId == dto.CartId &&
                    c.ProductId == dto.ProductId);



            if (existingItem != null)
            {

                existingItem.Quantity += dto.Quantity;

                await _context.SaveChangesAsync();


                return Ok(new
                {
                    message = "Cart quantity updated",
                    cartItemId = existingItem.CartItemId
                });

            }



            Cartitem cartItem = new Cartitem
            {
                CartId = dto.CartId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };


            _context.Cartitems.Add(cartItem);

            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "Item added to cart",
                cartItemId = cartItem.CartItemId
            });

        }




        // GET: api/Cart/{cartId}

        [HttpGet("{cartId}")]
        public async Task<IActionResult> GetCart(int cartId)
        {

            var items = await _context.Cartitems
                .Where(c => c.CartId == cartId)
                .Include(c => c.SellerProduct)
.ThenInclude(sp => sp.Product)
                .Select(c => new
                {
                    c.CartItemId,
                    c.ProductId,
                    c.Quantity,

                    ProductName = c.Product.ProductName,

                    Description = c.Product.Description
                })
                .ToListAsync();



            return Ok(items);

        }




        // GET: api/Cart/Customer/{customerId}

        [HttpGet("Customer/{customerId}")]
        public async Task<IActionResult> GetCustomerCart(int customerId)
        {

            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);



            if (cart == null)
            {
                return NotFound("Cart not found");
            }



            return Ok(new
            {
                cartId = cart.CartId
            });

        }





        // PUT: api/Cart/UpdateQuantity/{id}

        [HttpPut("UpdateQuantity/{id}")]
        public async Task<IActionResult> UpdateQuantity(
            int id,
            int quantity)
        {

            var item = await _context.Cartitems
                .FindAsync(id);



            if (item == null)
            {
                return NotFound("Cart item not found");
            }



            item.Quantity = quantity;


            await _context.SaveChangesAsync();



            return Ok(new
            {
                message = "Quantity updated"
            });

        }





        // DELETE: api/Cart/RemoveItem/{id}

        [HttpDelete("RemoveItem/{id}")]
        public async Task<IActionResult> RemoveItem(int id)
        {

            var item = await _context.Cartitems
                .FindAsync(id);



            if (item == null)
            {
                return NotFound("Cart item not found");
            }



            _context.Cartitems.Remove(item);


            await _context.SaveChangesAsync();



            return Ok(new
            {
                message = "Item removed"
            });

        }

    }
}