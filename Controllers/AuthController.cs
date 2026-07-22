using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ================= REGISTER =================

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingUser != null)
            {
                return BadRequest("Email already exists");
            }

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                PhoneNumber = dto.PhoneNumber,
                RoleId = dto.RoleId,
                AccountStatus = "Active",
                CreatedDate = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // CUSTOMER (RoleId = 5)
            if (user.RoleId == 5)
            {
                var customer = new Customer
                {
                    UserId = user.UserId,
                    FirstName = dto.Username,
                    LastName = ""
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                var cart = new Cart
                {
                    CustomerId = customer.CustomerId,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // SELLER (RoleId = 2)
            else if (user.RoleId == 2)
            {
                var seller = new Seller
                {
                    UserId = user.UserId,
                    BusinessName = "",
                    BusinessAddress = "",
                    Gstnumber = "",
                    ApprovalStatus = "Pending",
                    ComplaintCount = 0,
                    CreatedDate = DateTime.Now
                };

                _context.Sellers.Add(seller);
                await _context.SaveChangesAsync();
            }

            // DELIVERY PARTNER (RoleId = 4)
            else if (user.RoleId == 4)
            {
                var partner = new Deliverypartner
                {
                    UserId = user.UserId,
                    VehicleNumber = "",
                    AvailabilityStatus = "Offline"
                };

                _context.Deliverypartners.Add(partner);
                await _context.SaveChangesAsync();
            }

            // WAREHOUSE MANAGER (RoleId = 3)
            else if (user.RoleId == 3)
            {
                var manager = new Warehousemanager
                {
                    UserId = user.UserId,
                    WarehouseId = 1
                };

                _context.Warehousemanagers.Add(manager);
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                message = "Registration successful",
                userId = user.UserId
            });
        }

        // ================= LOGIN =================

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return Unauthorized("Invalid email or password");

            if (user.PasswordHash != HashPassword(dto.Password))
                return Unauthorized("Invalid email or password");

            var token = GenerateToken(user);

            return Ok(new
            {
                message = "Login successful",
                userId = user.UserId,
                roleId = user.RoleId,
                token
            });
        }

        // ================= JWT =================

        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("RoleId", user.RoleId.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ================= HASH PASSWORD =================

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();

            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }
    }
}