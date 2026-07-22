using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Ecommerce.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class ProductRecommendationController : ControllerBase
    {

        private readonly ApplicationDbContext _context;



        public ProductRecommendationController(
            ApplicationDbContext context)
        {
            _context = context;
        }







        // POST: api/ProductRecommendation/Add

        [HttpPost("Add")]

        public async Task<IActionResult> AddRecommendation(
            ProductRecommendationDTO dto)
        {


            var productExists =
                await _context.Products

                .AnyAsync(p =>
                    p.ProductId == dto.ProductId
                );



            if (!productExists)
            {
                return BadRequest(
                    "Product not found"
                );
            }







            var recommendation =
                new Productrecommendation
                {

                    CustomerId = dto.CustomerId,


                    ProductId = dto.ProductId,


                    Reason = dto.Reason,


                    CreatedDate = DateTime.Now

                };





            _context.Productrecommendations
                .Add(recommendation);



            await _context.SaveChangesAsync();




            return Ok(new
            {

                message = "Recommendation added",

                recommendationId =
                    recommendation.ProductRecommendationId

            });


        }









        // GET: api/ProductRecommendation/Customer/1

        [HttpGet("Customer/{customerId}")]

        public async Task<IActionResult> GetRecommendations(
            int customerId)
        {



            var recommendations =
                await _context.Productrecommendations

                .Where(r =>
                    r.CustomerId == customerId
                )

                .Include(r => r.Product)

                .Select(r => new
                {

                    recommendationId =
                        r.ProductRecommendationId,


                    productId =
                        r.ProductId,


                    productName =
                        r.Product.ProductName,


                    description =
                        r.Product.Description,


                    brand =
                        r.Product.Brand,


                    reason =
                        r.Reason


                })

                .ToListAsync();




            return Ok(recommendations);


        }









        // DELETE: api/ProductRecommendation/1

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteRecommendation(
            int id)
        {


            var recommendation =
                await _context.Productrecommendations
                .FindAsync(id);



            if (recommendation == null)
            {
                return NotFound(
                    "Recommendation not found"
                );
            }




            _context.Productrecommendations
                .Remove(recommendation);



            await _context.SaveChangesAsync();




            return Ok(new
            {

                message =
                "Recommendation deleted"

            });


        }



    }

}