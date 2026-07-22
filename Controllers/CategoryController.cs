using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: api/Category
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new
                {
                    c.CategoryId,
                    c.CategoryName,
                    c.Description
                })
                .ToListAsync();

            return Ok(categories);
        }


        // GET: api/Category/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories
                .Where(c => c.CategoryId == id)
                .Select(c => new
                {
                    c.CategoryId,
                    c.CategoryName,
                    c.Description
                })
                .FirstOrDefaultAsync();


            if (category == null)
            {
                return NotFound("Category not found");
            }


            return Ok(category);
        }


        // POST: api/Category
        [HttpPost]
        public async Task<IActionResult> AddCategory(Category category)
        {
            _context.Categories.Add(category);

            await _context.SaveChangesAsync();

            return Ok(category);
        }


        // PUT: api/Category/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, Category category)
        {
            var existingCategory = await _context.Categories.FindAsync(id);


            if (existingCategory == null)
            {
                return NotFound("Category not found");
            }


            existingCategory.CategoryName = category.CategoryName;
            existingCategory.Description = category.Description;


            await _context.SaveChangesAsync();


            return Ok(existingCategory);
        }


        // DELETE: api/Category/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);


            if (category == null)
            {
                return NotFound("Category not found");
            }


            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();


            return Ok("Category deleted successfully");
        }
    }
}