using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Ecommerce.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class ProductsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;


        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }



        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products.ToListAsync();

            return Ok(products);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);


            if (product == null)
                return NotFound();


            return Ok(product);
        }



        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return Ok(product);

        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {

            if (id != product.ProductId)
                return BadRequest();


            _context.Entry(product).State =
                EntityState.Modified;


            await _context.SaveChangesAsync();


            return Ok(product);

        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {

            var product = await _context.Products.FindAsync(id);


            if (product == null)
                return NotFound();


            _context.Products.Remove(product);


            await _context.SaveChangesAsync();


            return Ok("Deleted");

        }

    }

}