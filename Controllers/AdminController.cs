using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;


        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        

        // Approve / Reject Product
        // POST: api/Admin/ProductApproval
        [HttpPost("ProductApproval")]
        public async Task<IActionResult> ProductApproval(
            ProductApprovalDTO dto)
        {
            var product = await _context.Products
                .FindAsync(dto.ProductId);


            if (product == null)
            {
                return BadRequest("Product not found");
            }


            product.ApprovalStatus = dto.ApprovalStatus;



            var approval = new Productapproval
            {
                ProductId = dto.ProductId,
                AdminId = dto.AdminId,
                ApprovalStatus = dto.ApprovalStatus,
                Remarks = dto.Remarks,
                ApprovalDate = DateTime.Now
            };


            _context.Productapprovals.Add(approval);



            var action = new Adminaction
            {
                AdminId = dto.AdminId,
                ActionType = "Product Approval",
                Description = dto.Remarks,
                ActionDate = DateTime.Now
            };


            _context.Adminactions.Add(action);


            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "Product approval updated"
            });
        }





        // Change Seller Status
        // POST: api/Admin/SellerStatus
        [HttpPost("SellerStatus")]
        public async Task<IActionResult> SellerStatus(
            SellerStatusDTO dto)
        {

            var seller = await _context.Sellers
                .FindAsync(dto.SellerId);


            if (seller == null)
            {
                return BadRequest("Seller not found");
            }



            var history = new Sellerstatushistory
            {
                SellerId = dto.SellerId,
                PreviousStatus = seller.ApprovalStatus,
                NewStatus = dto.NewStatus,
                Reason = dto.Reason,
                ChangedDate = DateTime.Now
            };


            seller.ApprovalStatus = dto.NewStatus;



            _context.Sellerstatushistories.Add(history);



            var action = new Adminaction
            {
                AdminId = dto.AdminId,
                ActionType = "Seller Status Change",
                Description = dto.Reason,
                ActionDate = DateTime.Now
            };


            _context.Adminactions.Add(action);


            await _context.SaveChangesAsync();



            return Ok(new
            {
                message = "Seller status updated"
            });
        }





        // View Admin Actions
        // GET: api/Admin/Actions/1
        [HttpGet("Actions/{adminId}")]
        public async Task<IActionResult> GetActions(
            int adminId)
        {

            var actions = await _context.Adminactions
                .Where(a => a.AdminId == adminId)
                .ToListAsync();


            return Ok(actions);
        }

        // GET: api/Admin/Users
        [HttpGet("Users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Select(u => new
                {
                    userId = u.UserId,
                    username = u.Username,
                    email = u.Email,
                    phoneNumber = u.PhoneNumber,
                    role = u.Role.RoleName,
                    status = u.AccountStatus,
                    createdDate = u.CreatedDate
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/Admin/Sellers
        [HttpGet("Sellers")]
        public async Task<IActionResult> GetSellers()
        {
            var sellers = await _context.Sellers
                .Include(s => s.User)
                .Select(s => new
                {
                    sellerId = s.SellerId,
                    businessName = s.BusinessName,
                    email = s.User.Email,
                    phoneNumber = s.User.PhoneNumber,
                    approvalStatus = s.ApprovalStatus,
                    status = s.Status,
                    complaintCount = s.ComplaintCount,
                    createdDate = s.CreatedDate
                })
                .ToListAsync();

            return Ok(sellers);
        }
    }
}
