using Ecommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminUserActivityController : ControllerBase
    {
        private readonly IUserLoginLogger _loginLogger;

        public AdminUserActivityController(IUserLoginLogger loginLogger)
        {
            _loginLogger = loginLogger;
        }

        // GET: api/AdminUserActivity/RecentLogins
        [HttpGet("RecentLogins")]
        public IActionResult GetRecentLogins([FromQuery] int limit = 50)
        {
            // Verify Admin Role claim (RoleId 1 or Role Admin)
            var roleClaim = User.FindFirst("RoleId")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (roleClaim != "1" && roleClaim != "Admin")
            {
                return StatusCode(403, new { message = "ACCESS RESTRICTED: Only authenticated administrators can access user login activity details." });
            }

            var logs = _loginLogger.GetRecentLogins(limit);
            return Ok(logs);
        }

        // GET: api/AdminUserActivity/UnreadCustomerLogins
        [HttpGet("UnreadCustomerLogins")]
        public IActionResult GetUnreadCustomerLogins()
        {
            var roleClaim = User.FindFirst("RoleId")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (roleClaim != "1" && roleClaim != "Admin")
            {
                return StatusCode(403, new { message = "ACCESS RESTRICTED: Only authenticated administrators can access customer login notifications." });
            }

            var unread = _loginLogger.GetUnreadCustomerLogins();
            return Ok(unread);
        }

        // POST: api/AdminUserActivity/MarkRead/logId
        [HttpPost("MarkRead/{logId}")]
        public IActionResult MarkAsRead(string logId)
        {
            var roleClaim = User.FindFirst("RoleId")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (roleClaim != "1" && roleClaim != "Admin")
            {
                return StatusCode(403, new { message = "ACCESS RESTRICTED: Only authenticated administrators can modify notification status." });
            }

            _loginLogger.MarkAsRead(logId);
            return Ok(new { message = "Login notification marked as read." });
        }
    }
}
