using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Ecommerce.Controllers
{

    [Route("api/[controller]")]
    [Route("api/Product")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 0, [FromQuery] string? search = null, [FromQuery] string? category = null)
        {
            var query = _context.Sellerproducts
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(category) && category != "All")
            {
                query = query.Where(sp => sp.Product.Category != null && sp.Product.Category.CategoryName.ToLower() == category.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower().Trim();
                query = query.Where(sp => sp.Product.ProductName.ToLower().Contains(s) || 
                                          (sp.Product.Brand != null && sp.Product.Brand.ToLower().Contains(s)));
            }

            var projected = query.Select(sp => new
            {
                sellerProductId = sp.SellerProductId,
                productId = sp.Product.ProductId,
                productName = sp.Product.ProductName,
                description = sp.Product.Description,
                brand = sp.Product.Brand,
                warranty = sp.Product.Warranty,
                category = sp.Product.Category != null ? sp.Product.Category.CategoryName : "General",
                price = sp.Price,
                stock = sp.StockQuantity,
                image = sp.Product.Productimages.Select(i => i.ImageUrl).FirstOrDefault()
            });

            if (pageSize > 0)
            {
                var pagedItems = await projected.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                return Ok(pagedItems);
            }

            var sellerProducts = await projected.ToListAsync();

            if (sellerProducts.Count > 0)
            {
                return Ok(sellerProducts);
            }

            var directProductsQuery = _context.Products.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(category) && category != "All")
            {
                directProductsQuery = directProductsQuery.Where(p => p.Category != null && p.Category.CategoryName.ToLower() == category.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower().Trim();
                directProductsQuery = directProductsQuery.Where(p => p.ProductName.ToLower().Contains(s) || (p.Brand != null && p.Brand.ToLower().Contains(s)));
            }

            var directProjected = directProductsQuery.Select(p => new
            {
                sellerProductId = p.ProductId,
                productId = p.ProductId,
                productName = p.ProductName,
                description = p.Description,
                brand = p.Brand,
                warranty = p.Warranty,
                category = p.Category != null ? p.Category.CategoryName : "General",
                price = 9999m,
                stock = 10,
                image = p.Productimages.Select(i => i.ImageUrl).FirstOrDefault()
            });

            if (pageSize > 0)
            {
                var pagedDirect = await directProjected.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                return Ok(pagedDirect);
            }

            var directProducts = await directProjected.ToListAsync();
            return Ok(directProducts);
        }







        // GET: api/Product/{id}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products
                .AsNoTracking()
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