using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Ecommerce.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class SearchHistoryController : ControllerBase
    {

        private readonly ApplicationDbContext _context;



        public SearchHistoryController(ApplicationDbContext context)
        {
            _context = context;
        }






        // POST: api/SearchHistory/Add

        [HttpPost("Add")]

        public async Task<IActionResult> AddSearch(
            SearchHistoryDTO dto)
        {


            var search = new Searchhistory
            {

                CustomerId = dto.CustomerId,


                SearchKeyword = dto.SearchKeyword,


                SearchDate = DateTime.Now

            };



            _context.Searchhistories.Add(search);



            await _context.SaveChangesAsync();



            return Ok(new
            {

                message = "Search saved",

                searchId = search.SearchId

            });


        }








        // GET: api/SearchHistory/Customer/1

        [HttpGet("Customer/{customerId}")]

        public async Task<IActionResult> GetCustomerSearches(
            int customerId)
        {


            var searches = await _context.Searchhistories

                .Where(s =>
                    s.CustomerId == customerId
                )

                .OrderByDescending(s =>
                    s.SearchDate
                )

                .ToListAsync();



            return Ok(searches);


        }









        // DELETE: api/SearchHistory/1

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteSearch(
            int id)
        {


            var search =
                await _context.Searchhistories
                .FindAsync(id);



            if (search == null)
            {
                return NotFound(
                    "Search history not found"
                );
            }




            _context.Searchhistories.Remove(search);



            await _context.SaveChangesAsync();



            return Ok(new
            {

                message = "Search deleted"

            });


        }


    }

}