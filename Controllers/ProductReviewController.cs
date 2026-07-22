using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Ecommerce.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class ProductReviewController : ControllerBase
    {


        private readonly ApplicationDbContext _context;



        public ProductReviewController(ApplicationDbContext context)
        {
            _context = context;
        }








        // POST: api/ProductReview/Add

        [HttpPost("Add")]

        public async Task<IActionResult> AddReview(
            ProductReviewDTO dto)
        {



            if (dto.Rating < 1 || dto.Rating > 5)
            {
                return BadRequest(
                    "Rating must be between 1 and 5"
                );
            }






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







            var review = new Productreview
            {


                CustomerId = dto.CustomerId,


                ProductId = dto.ProductId,


                Rating = dto.Rating,


                ReviewText = dto.ReviewText,


                CreatedDate = DateTime.Now


            };




            _context.Productreviews.Add(review);



            await _context.SaveChangesAsync();




            return Ok(new
            {

                message = "Review added successfully",

                reviewId = review.ReviewId

            });


        }









        // GET: api/ProductReview/Product/1

        [HttpGet("Product/{productId}")]

        public async Task<IActionResult> GetProductReviews(
            int productId)
        {


            var reviews = await _context.Productreviews

                .Where(r =>
                    r.ProductId == productId
                )

                .Include(r => r.Customer)

                .Select(r => new
                {

                    reviewId = r.ReviewId,


                    customer =
                        r.Customer.FirstName,


                    rating = r.Rating,


                    reviewText = r.ReviewText,


                    date = r.CreatedDate


                })

                .ToListAsync();



            return Ok(reviews);


        }








        // GET: api/ProductReview/Product/1/Rating

        [HttpGet("Product/{productId}/Rating")]

        public async Task<IActionResult> GetAverageRating(
            int productId)
        {



            var rating =
                await _context.Productreviews

                .Where(r =>
                    r.ProductId == productId
                )

                .AverageAsync(r =>
                    (double)r.Rating
                );



            return Ok(new
            {

                productId = productId,


                averageRating = Math.Round(
                    rating,
                    2
                )

            });


        }



    }

}