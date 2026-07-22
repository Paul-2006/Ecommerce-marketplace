using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Ecommerce.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class NotificationController : ControllerBase
    {

        private readonly ApplicationDbContext _context;



        public NotificationController(ApplicationDbContext context)
        {
            _context = context;
        }







        // POST: api/Notification/Create

        [HttpPost("Create")]

        public async Task<IActionResult> CreateNotification(
            NotificationDTO dto)
        {


            var notification =
                new Notification
                {

                    UserId = dto.UserId,


                    Message = dto.Message,


                    Type = dto.Type,


                    IsRead = false,


                    CreatedDate = DateTime.Now

                };





            _context.Notifications.Add(notification);



            await _context.SaveChangesAsync();




            return Ok(new
            {

                message =
                "Notification created",


                notificationId =
                notification.NotificationId

            });


        }









        // GET: api/Notification/User/1

        [HttpGet("User/{userId}")]

        public async Task<IActionResult> GetUserNotifications(
            int userId)
        {


            var notifications =
                await _context.Notifications

                .Where(n =>
                    n.UserId == userId
                )

                .OrderByDescending(n =>
                    n.CreatedDate
                )

                .ToListAsync();



            return Ok(notifications);


        }









        // PUT: api/Notification/Read/1

        [HttpPut("Read/{id}")]

        public async Task<IActionResult> MarkAsRead(
            int id)
        {


            var notification =
                await _context.Notifications
                .FindAsync(id);



            if (notification == null)
            {
                return NotFound(
                    "Notification not found"
                );
            }




            notification.IsRead = true;



            await _context.SaveChangesAsync();




            return Ok(new
            {

                message =
                "Notification marked as read"

            });


        }









        // DELETE: api/Notification/1

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteNotification(
            int id)
        {


            var notification =
                await _context.Notifications
                .FindAsync(id);



            if (notification == null)
            {
                return NotFound(
                    "Notification not found"
                );
            }




            _context.Notifications.Remove(notification);



            await _context.SaveChangesAsync();




            return Ok(new
            {

                message =
                "Notification deleted"

            });


        }



    }

}