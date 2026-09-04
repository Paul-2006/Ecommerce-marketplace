using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Ecommerce.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class ProductController : ControllerBase
    {


        private readonly ApplicationDbContext _context;



        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }





        // GET: api/Product

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {

            var products = await _context.Sellerproducts

            .Include(sp => sp.Product)

            .ThenInclude(p => p.Productimages)

            .Select(sp => new
            {

                sellerProductId = sp.SellerProductId,


                productId = sp.Product.ProductId,


                productName = sp.Product.ProductName,


                description = sp.Product.Description,


                brand = sp.Product.Brand,


                warranty = sp.Product.Warranty,


                price = sp.Price,


                stock = sp.StockQuantity,


                image = sp.Product.Productimages
            .Select(i => i.ImageUrl)
            .FirstOrDefault()


            })

            .ToListAsync();



            return Ok(products);

        }







        // GET: api/Product/{id}

        [HttpGet("{id}")]

        public async Task<IActionResult> GetProduct(int id)
        {


            var product = await _context.Products

            .Include(p => p.Productimages)

            .Where(p => p.ProductId == id)


            .Select(p => new

            {


                productId = p.ProductId,


                categoryId = p.CategoryId,


                productName = p.ProductName,


                description = p.Description,


                brand = p.Brand,


                warranty = p.Warranty,


                createdDate = p.CreatedDate,


                approvalStatus = p.ApprovalStatus,

                price = p.Sellerproducts
            .OrderBy(sp => sp.Price)
            .Select(sp => (decimal?)sp.Price)
            .FirstOrDefault(),

                stock = p.Sellerproducts
            .OrderBy(sp => sp.Price)
            .Select(sp => sp.StockQuantity)
            .FirstOrDefault(),



                image = p.Productimages
            .Select(i => i.ImageUrl)
            .FirstOrDefault()



            })


            .FirstOrDefaultAsync();



            if (product == null)
            {

                return NotFound("Product not found");

            }



            return Ok(product);


        }








        // POST: api/Product

        [HttpPost]

        public async Task<IActionResult> AddProduct(ProductDTO dto)
        {


            if (dto.ApprovalStatus != "Pending" &&
               dto.ApprovalStatus != "Approved" &&
               dto.ApprovalStatus != "Rejected")
            {

                return BadRequest(
                "ApprovalStatus must be Pending, Approved, or Rejected"
                );

            }



            var product = new Product
            {

                CategoryId = dto.CategoryId,


                ProductName = dto.ProductName,


                Description = dto.Description,


                Brand = dto.Brand,


                Warranty = dto.Warranty,


                CreatedDate = DateTime.Now,


                ApprovalStatus = dto.ApprovalStatus

            };



            _context.Products.Add(product);


            await _context.SaveChangesAsync();



            return Ok(product);


        }








        // PUT: api/Product/{id}

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateProduct(
        int id,
        ProductDTO dto)
        {


            var product =
            await _context.Products.FindAsync(id);



            if (product == null)
            {

                return NotFound("Product not found");

            }



            product.CategoryId = dto.CategoryId;


            product.ProductName = dto.ProductName;


            product.Description = dto.Description;


            product.Brand = dto.Brand;


            product.Warranty = dto.Warranty;


            product.ApprovalStatus = dto.ApprovalStatus;



            await _context.SaveChangesAsync();



            return Ok(product);


        }







        // DELETE: api/Product/{id}

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteProduct(int id)
        {


            var product =
            await _context.Products.FindAsync(id);



            if (product == null)
            {

                return NotFound("Product not found");

            }



            _context.Products.Remove(product);



            await _context.SaveChangesAsync();



            return Ok(
            "Product deleted successfully"
            );


        }



    }


}