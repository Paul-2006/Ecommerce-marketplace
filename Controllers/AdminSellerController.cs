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
    [Authorize(Policy = "AdminOnly")]
    public class AdminSellerController : ControllerBase
    {

        private readonly ApplicationDbContext _context;


        public AdminSellerController(ApplicationDbContext context)
        {
            _context = context;
        }








        // GET: api/AdminSeller/Pending

        [HttpGet("Pending")]

        public async Task<IActionResult> GetPendingSellers()
        {


            var sellers =
                await _context.Sellers

                .Where(s =>
                    s.Status == "Pending"
                )

                .ToListAsync();



            return Ok(sellers);

        }









        // PUT: api/AdminSeller/UpdateStatus

        [HttpPut("UpdateStatus")]

        public async Task<IActionResult> UpdateSellerStatus(
            SellerApprovalDTO dto)
        {


            var seller =
                await _context.Sellers

                .FindAsync(dto.SellerId);



            if (seller == null)
            {
                return NotFound(
                    "Seller not found"
                );
            }







            seller.Status =
                dto.Status;







            var history =
                new Sellerstatushistory
                {

                    SellerId =
                        dto.SellerId,


                    Status =
                        dto.Status,


                    Remarks =
                        dto.Remarks,


                    ChangedDate =
                        DateTime.Now

                };





            _context.Sellerstatushistories
                .Add(history);




            await _context.SaveChangesAsync();




            return Ok(new
            {

                message =
                "Seller status updated",


                status =
                dto.Status

            });


        }









        // GET: api/AdminSeller/History/1

        [HttpGet("History/{sellerId}")]

        public async Task<IActionResult> GetSellerHistory(
            int sellerId)
        {



            var history =
                await _context.Sellerstatushistories

                .Where(s =>
                    s.SellerId == sellerId
                )

                .OrderByDescending(s =>
                    s.ChangedDate
                )

                .ToListAsync();



            return Ok(history);


        }



    }

}