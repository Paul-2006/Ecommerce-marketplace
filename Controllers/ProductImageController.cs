using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;


namespace Ecommerce.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class ProductImageController : ControllerBase
    {


        private readonly ApplicationDbContext _context;


        public ProductImageController(ApplicationDbContext context)
        {
            _context = context;
        }





        [HttpPost("Upload")]

        public async Task<IActionResult> Upload(
        int productId,
        IFormFile image)
        {


            if (image == null)
            {
                return BadRequest("Image is required");
            }



            string folderPath =
            Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "images"
            );



            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }




            string fileName =
            Guid.NewGuid().ToString()
            +
            Path.GetExtension(image.FileName);



            string filePath =
            Path.Combine(folderPath, fileName);



            using (var stream =
            new FileStream(filePath, FileMode.Create))
            {

                await image.CopyToAsync(stream);

            }




            var productImage = new Productimage
            {

                ProductId = productId,

                ImageUrl = "/images/" + fileName

            };



            _context.Productimages.Add(productImage);


            await _context.SaveChangesAsync();



            return Ok(new
            {

                message = "Image uploaded successfully",

                imageUrl = productImage.ImageUrl

            });


        }


    }

}