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
    [Authorize(Policy = "SellerOnly")]
    public class SellerProductController : ControllerBase
    {

        private readonly ApplicationDbContext _context;



        public SellerProductController(ApplicationDbContext context)
        {
            _context = context;
        }







        // POST: api/SellerProduct/Add

        [HttpPost("Add")]

        public async Task<IActionResult> AddSellerProduct(
            SellerProductDTO dto)
        {


            var productExists =
                await _context.Products

                .AnyAsync(p =>
                    p.ProductId == dto.ProductId
                );



            if (!productExists)
            {
                return BadRequest(
                    "Product does not exist"
                );
            }





            var sellerProduct =
                new Sellerproduct
                {

                    SellerId = dto.SellerId,


                    ProductId = dto.ProductId,


                    Price = dto.Price,


                    StockQuantity = dto.StockQuantity

                };





            _context.Sellerproducts
                .Add(sellerProduct);



            await _context.SaveChangesAsync();




            return Ok(new
            {

                message =
                "Product added to seller store",


                sellerProductId =
                sellerProduct.SellerProductId

            });


        }









        // GET: api/SellerProduct/Seller/1

        [HttpGet("Seller/{sellerId}")]

        public async Task<IActionResult> GetSellerProducts(
            int sellerId)
        {



            var products =
                await _context.Sellerproducts

                .Where(s =>
                    s.SellerId == sellerId
                )

                .Include(s => s.Product)

                .Select(s => new
                {

                    sellerProductId =
                        s.SellerProductId,


                    productId =
                        s.ProductId,


                    productName =
                        s.Product.ProductName,


                    price =
                        s.Price,


                    stock =
                        s.StockQuantity

                })

                .ToListAsync();




            return Ok(products);


        }









        // PUT: api/SellerProduct/Update/1

        [HttpPut("Update/{id}")]

        public async Task<IActionResult> UpdateSellerProduct(
            int id,
            SellerProductDTO dto)
        {


            var sellerProduct =
                await _context.Sellerproducts
                .FindAsync(id);



            if (sellerProduct == null)
            {
                return NotFound(
                    "Seller product not found"
                );
            }




            sellerProduct.Price =
                dto.Price;


            sellerProduct.StockQuantity =
                dto.StockQuantity;




            await _context.SaveChangesAsync();




            return Ok(new
            {

                message =
                "Seller product updated"

            });


        }









        // DELETE: api/SellerProduct/Delete/1

        [HttpDelete("Delete/{id}")]

        public async Task<IActionResult> DeleteSellerProduct(
            int id)
        {


            var sellerProduct =
                await _context.Sellerproducts
                .FindAsync(id);



            if (sellerProduct == null)
            {
                return NotFound(
                    "Seller product not found"
                );
            }




            _context.Sellerproducts
                .Remove(sellerProduct);



            await _context.SaveChangesAsync();




            return Ok(new
            {

                message =
                "Seller product removed"

            });


        }

        // PUT: api/SellerProduct/ToggleStatus/1
        [HttpPut("ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var sellerProduct = await _context.Sellerproducts.FindAsync(id);
            if (sellerProduct == null)
            {
                return NotFound("Seller product not found");
            }

            sellerProduct.ProductStatus = sellerProduct.ProductStatus == "Active" ? "Inactive" : "Active";
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Product status updated to {sellerProduct.ProductStatus}",
                status = sellerProduct.ProductStatus
            });
        }
    }
}