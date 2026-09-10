using Ecommerce.Data;
using Ecommerce.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Ecommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProfileController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("Me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = GetUserId();

        if (userId == null)
        {
            return Unauthorized("Invalid token.");
        }

        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId.Value);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        return Ok(new
        {
            user.UserId,
            user.Username,
            user.Email,
            user.PhoneNumber,
            user.RoleId,
            role = user.Role.RoleName,
            user.AccountStatus,
            user.CreatedDate,
            customer = await _context.Customers
                .Where(c => c.UserId == user.UserId)
                .Select(c => new
                {
                    c.CustomerId,
                    c.FirstName,
                    c.LastName,
                    c.DateOfBirth
                })
                .FirstOrDefaultAsync(),
            seller = await _context.Sellers
                .Where(s => s.UserId == user.UserId)
                .Select(s => new
                {
                    s.SellerId,
                    s.BusinessName,
                    s.BusinessAddress,
                    s.Gstnumber,
                    s.ApprovalStatus,
                    s.Status,
                    s.ComplaintCount,
                    s.CreatedDate
                })
                .FirstOrDefaultAsync(),
            deliveryPartner = await _context.Deliverypartners
                .Where(d => d.UserId == user.UserId)
                .Select(d => new
                {
                    d.DeliveryPartnerId,
                    d.PartnerName,
                    d.PhoneNumber,
                    d.VehicleNumber,
                    d.AvailabilityStatus,
                    d.Status,
                    currentLatitude = d.CurrentLatitude ?? d.Latitude,
                    currentLongitude = d.CurrentLongitude ?? d.Longitude,
                    d.CreatedDate
                })
                .FirstOrDefaultAsync(),
            warehouseManager = await _context.Warehousemanagers
                .Where(w => w.UserId == user.UserId)
                .Select(w => new
                {
                    w.WarehouseManagerId,
                    w.WarehouseId,
                    w.Warehouse.WarehouseName,
                    w.Warehouse.Location,
                    w.Warehouse.ContactNumber,
                    w.Warehouse.Capacity,
                    w.Warehouse.Status
                })
                .FirstOrDefaultAsync()
        });
    }

    [HttpPut("Me")]
    public async Task<IActionResult> UpdateMyProfile(ProfileUpdateDTO dto)
    {
        var userId = GetUserId();

        if (userId == null)
        {
            return Unauthorized("Invalid token.");
        }

        var user = await _context.Users.FindAsync(userId.Value);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        if (!string.IsNullOrWhiteSpace(dto.Username))
        {
            user.Username = dto.Username;
        }

        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            user.PhoneNumber = dto.PhoneNumber;
        }

        if (user.RoleId == 5)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == user.UserId);

            if (customer != null)
            {
                customer.FirstName = dto.FirstName ?? customer.FirstName;
                customer.LastName = dto.LastName ?? customer.LastName;
                customer.DateOfBirth = dto.DateOfBirth ?? customer.DateOfBirth;
            }
        }
        else if (user.RoleId == 2)
        {
            var seller = await _context.Sellers
                .FirstOrDefaultAsync(s => s.UserId == user.UserId);

            if (seller != null)
            {
                seller.BusinessName = dto.BusinessName ?? seller.BusinessName;
                seller.BusinessAddress = dto.BusinessAddress ?? seller.BusinessAddress;
                seller.Gstnumber = dto.Gstnumber ?? seller.Gstnumber;
            }
        }
        else if (user.RoleId == 4)
        {
            var partner = await _context.Deliverypartners
                .FirstOrDefaultAsync(d => d.UserId == user.UserId);

            if (partner != null)
            {
                partner.PartnerName = dto.PartnerName ?? partner.PartnerName;
                partner.PhoneNumber = dto.PhoneNumber ?? partner.PhoneNumber;
                partner.VehicleNumber = dto.VehicleNumber ?? partner.VehicleNumber;
            }
        }
        else if (user.RoleId == 3)
        {
            var manager = await _context.Warehousemanagers
                .Include(w => w.Warehouse)
                .FirstOrDefaultAsync(w => w.UserId == user.UserId);

            if (manager?.Warehouse != null)
            {
                manager.Warehouse.WarehouseName = dto.WarehouseName ?? manager.Warehouse.WarehouseName;
                manager.Warehouse.Location = dto.WarehouseLocation ?? manager.Warehouse.Location;
                manager.Warehouse.ContactNumber = dto.WarehouseContactNumber ?? manager.Warehouse.ContactNumber;
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Profile updated successfully"
        });
    }

    private int? GetUserId()
    {
        var userId = User.FindFirstValue("UserId");

        return int.TryParse(userId, out var id)
            ? id
            : null;
    }
}
