using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly ApplicationDbContext _context;


        public FeedbackController(ApplicationDbContext context)
        {
            _context = context;
        }



        // Add Feedback
        // POST: api/Feedback/Add
        [HttpPost("Add")]
        public async Task<IActionResult> AddFeedback(
            FeedbackDTO dto)
        {
            var customerExists =
                await _context.Customers
                .AnyAsync(c => c.CustomerId == dto.CustomerId);


            if (!customerExists)
            {
                return BadRequest("Customer not found");
            }



            var feedback = new Customerfeedback
            {
                CustomerId = dto.CustomerId,
                OrderId = dto.OrderId,
                SellerId = dto.SellerId,
                FeedbackType = dto.FeedbackType,
                Rating = dto.Rating,
                ComplaintDescription = dto.ComplaintDescription,
                FeedbackStatus = "Pending",
                CreatedDate = DateTime.Now
            };


            _context.Customerfeedbacks.Add(feedback);

            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "Feedback submitted",
                feedbackId = feedback.FeedbackId
            });
        }





        // Get Customer Feedback
        // GET: api/Feedback/Customer/1
        [HttpGet("Customer/{customerId}")]
        public async Task<IActionResult> GetCustomerFeedback(
            int customerId)
        {
            var feedbacks = await _context.Customerfeedbacks
                .Where(f => f.CustomerId == customerId)
                .Select(f => new
                {
                    f.FeedbackId,
                    f.FeedbackType,
                    f.Rating,
                    f.ComplaintDescription,
                    f.FeedbackStatus,
                    f.CreatedDate
                })
                .ToListAsync();


            return Ok(feedbacks);
        }





        // Update Feedback Status
        // PUT: api/Feedback/Status/1
        [HttpPut("Status/{id}")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            string status)
        {
            var feedback =
                await _context.Customerfeedbacks
                .FindAsync(id);


            if (feedback == null)
            {
                return NotFound("Feedback not found");
            }


            feedback.FeedbackStatus = status;


            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "Feedback status updated"
            });
        }
    }
}