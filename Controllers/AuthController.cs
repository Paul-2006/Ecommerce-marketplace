using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly IUserLoginLogger _loginLogger;

        private static readonly Dictionary<string, int> RoleIds = new()
        {
            ["Admin"] = 1,
            ["Seller"] = 2,
            ["Warehouse"] = 3,
            ["Delivery"] = 4,
            ["Customer"] = 5
        };

        public AuthController(
            ApplicationDbContext context,
            IConfiguration configuration,
            IOtpService otpService,
            IEmailService emailService,
            ISmsService smsService,
            IUserLoginLogger loginLogger)
        {
            _context = context;
            _configuration = configuration;
            _otpService = otpService;
            _emailService = emailService;
            _smsService = smsService;
            _loginLogger = loginLogger;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            if (dto == null) return BadRequest("Registration payload required.");

            if (dto.RoleId == RoleIds["Admin"])
            {
                return BadRequest("Admin registration is disabled. Configure the single admin in AdminSeed.");
            }

            if (!RoleIds.ContainsValue(dto.RoleId))
            {
                return BadRequest("Invalid role.");
            }

            if (string.IsNullOrWhiteSpace(dto.Email) || !System.Net.Mail.MailAddress.TryCreate(dto.Email.Trim(), out _))
            {
                return BadRequest("Please enter a valid email address.");
            }

            var cleanPhone = dto.PhoneNumber?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(cleanPhone) || cleanPhone.Length < 10)
            {
                return BadRequest("Please enter a valid phone number (at least 10 digits).");
            }

            var cleanEmail = dto.Email.Trim().ToLower();

            var emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == cleanEmail);
            if (emailExists)
            {
                return BadRequest("Email address is already registered. Please login or use Forgot Password.");
            }

            var phoneExists = await _context.Users.AnyAsync(u => u.PhoneNumber == cleanPhone);
            if (phoneExists)
            {
                return BadRequest("Phone number is already registered. Please login or use a different phone number.");
            }

            var user = new User
            {
                Username = dto.Username,
                Email = cleanEmail,
                PasswordHash = PasswordHasher.Hash(dto.Password),
                PhoneNumber = cleanPhone,
                RoleId = dto.RoleId,
                AccountStatus = "Active",
                CreatedDate = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            if (user.RoleId == RoleIds["Customer"])
            {
                var customer = new Customer
                {
                    UserId = user.UserId,
                    FirstName = dto.Username,
                    LastName = ""
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                _context.Carts.Add(new Cart
                {
                    CustomerId = customer.CustomerId,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                });
            }
            else if (user.RoleId == RoleIds["Seller"])
            {
                _context.Sellers.Add(new Seller
                {
                    UserId = user.UserId,
                    BusinessName = "",
                    BusinessAddress = "",
                    Gstnumber = "",
                    ApprovalStatus = "Pending",
                    Status = "Pending",
                    ComplaintCount = 0,
                    CreatedDate = DateTime.Now
                });
            }
            else if (user.RoleId == RoleIds["Delivery"])
            {
                _context.Deliverypartners.Add(new Deliverypartner
                {
                    UserId = user.UserId,
                    VehicleNumber = "",
                    AvailabilityStatus = "Offline"
                });
            }
            else if (user.RoleId == RoleIds["Warehouse"])
            {
                _context.Warehousemanagers.Add(new Warehousemanager
                {
                    UserId = user.UserId,
                    WarehouseId = 1
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Registration successful",
                userId = user.UserId
            });
        }

        [HttpPost("SendOtp")]
        public async Task<IActionResult> SendOtp([FromBody] OtpRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request?.Email) || !System.Net.Mail.MailAddress.TryCreate(request.Email.Trim(), out _))
            {
                return BadRequest("Please enter a valid email address.");
            }

            var cleanEmail = request.Email.Trim().ToLower();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == cleanEmail);
            
            if (user == null && request.Purpose != "Register")
            {
                return NotFound("Account not found. Please register first.");
            }

            var (success, message, otpCode) = await _otpService.GenerateOtpAsync(cleanEmail, request.Purpose ?? "Login");
            if (!success)
            {
                return BadRequest(message);
            }

            string recipientName = user?.Username ?? "User";
            await _emailService.SendOtpEmailAsync(cleanEmail, recipientName, otpCode);

            return Ok(new { message = "Security verification code sent to your registered email address." });
        }

        [HttpPost("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyDTO request)
        {
            if (string.IsNullOrWhiteSpace(request?.Email) || string.IsNullOrWhiteSpace(request?.OtpCode))
            {
                return BadRequest("Email address and verification code are required.");
            }

            var (success, message) = await _otpService.VerifyOtpAsync(request.Email.Trim(), request.OtpCode.Trim(), request.Purpose ?? "Login");
            if (!success)
            {
                return BadRequest(message);
            }

            return Ok(new { message = "Verification code confirmed successfully." });
        }

        [HttpPost("Login")]
        public Task<IActionResult> Login(LoginDTO dto)
        {
            return LoginForRole(dto, null);
        }

        [HttpPost("Login/Admin")]
        public Task<IActionResult> AdminLogin(LoginDTO dto)
        {
            return LoginForRole(dto, RoleIds["Admin"]);
        }

        [HttpPost("Login/Seller")]
        public Task<IActionResult> SellerLogin(LoginDTO dto)
        {
            return LoginForRole(dto, RoleIds["Seller"]);
        }

        [HttpPost("Login/Customer")]
        public Task<IActionResult> CustomerLogin(LoginDTO dto)
        {
            return LoginForRole(dto, RoleIds["Customer"]);
        }

        [HttpPost("Login/Delivery")]
        public Task<IActionResult> DeliveryLogin(LoginDTO dto)
        {
            return LoginForRole(dto, RoleIds["Delivery"]);
        }

        [HttpPost("Login/Warehouse")]
        public Task<IActionResult> WarehouseLogin(LoginDTO dto)
        {
            return LoginForRole(dto, RoleIds["Warehouse"]);
        }

        private async Task<IActionResult> LoginForRole(LoginDTO dto, int? requiredRoleId)
        {
            if (string.IsNullOrWhiteSpace(dto?.Email))
            {
                return BadRequest("Please enter your registered email, username, or phone number.");
            }

            var identifier = dto.Email.Trim();
            var cleanId = identifier.ToLower();
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == cleanId ||
                                          u.Username.ToLower() == cleanId ||
                                          u.PhoneNumber == identifier);

            if (user == null)
            {
                return NotFound("Account not found. Please verify your registered email, username, or phone number.");
            }

            var cleanPassword = dto.Password?.Trim() ?? "";
            var hashedPassword = PasswordHasher.Hash(cleanPassword);

            if (user.PasswordHash != hashedPassword && user.PasswordHash != dto.Password)
            {
                return Unauthorized("Incorrect password. Please verify your password and try again.");
            }

            if (requiredRoleId.HasValue && user.RoleId != requiredRoleId.Value)
            {
                return Unauthorized("This account cannot access this portal.");
            }

            if (user.AccountStatus != "Active")
            {
                return Unauthorized("Account is not active.");
            }

            var token = GenerateToken(user);

            int? customerId = null;
            int? sellerId = null;
            int? deliveryPartnerId = null;
            int? warehouseManagerId = null;

            if (user.RoleId == RoleIds["Customer"])
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.UserId);
                if (customer == null)
                {
                    customer = new Customer
                    {
                        UserId = user.UserId,
                        FirstName = user.Username,
                        LastName = ""
                    };
                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();

                    _context.Carts.Add(new Cart
                    {
                        CustomerId = customer.CustomerId,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now
                    });
                    await _context.SaveChangesAsync();
                }
                customerId = customer.CustomerId;
            }
            else if (user.RoleId == RoleIds["Seller"])
            {
                var seller = await _context.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId);
                if (seller == null)
                {
                    seller = new Seller
                    {
                        UserId = user.UserId,
                        BusinessName = user.Username,
                        BusinessAddress = "",
                        Gstnumber = "",
                        ApprovalStatus = "Approved",
                        Status = "Active",
                        ComplaintCount = 0,
                        CreatedDate = DateTime.Now
                    };
                    _context.Sellers.Add(seller);
                    await _context.SaveChangesAsync();
                }
                sellerId = seller.SellerId;
            }
            else if (user.RoleId == RoleIds["Delivery"])
            {
                var delivery = await _context.Deliverypartners.FirstOrDefaultAsync(d => d.UserId == user.UserId);
                if (delivery == null)
                {
                    delivery = new Deliverypartner
                    {
                        UserId = user.UserId,
                        VehicleNumber = "KA-01-EA-9988",
                        AvailabilityStatus = "Online"
                    };
                    _context.Deliverypartners.Add(delivery);
                    await _context.SaveChangesAsync();
                }
                deliveryPartnerId = delivery.DeliveryPartnerId;
            }
            else if (user.RoleId == RoleIds["Warehouse"])
            {
                var warehouse = await _context.Warehousemanagers.FirstOrDefaultAsync(w => w.UserId == user.UserId);
                if (warehouse == null)
                {
                    warehouse = new Warehousemanager
                    {
                        UserId = user.UserId,
                        WarehouseId = 1
                    };
                    _context.Warehousemanagers.Add(warehouse);
                    await _context.SaveChangesAsync();
                }
                warehouseManagerId = warehouse.WarehouseManagerId;
            }

            var userRoleName = GetRoleName(user.RoleId);
            _loginLogger.RecordLogin(
                userId: user.UserId,
                customerId: customerId,
                username: user.Username,
                email: user.Email,
                userRole: userRoleName,
                accountStatus: user.AccountStatus ?? "Active",
                loginStatus: "Success"
            );

            return Ok(new
            {
                message = "Login successful",
                userId = user.UserId,
                username = user.Username,
                email = user.Email,
                roleId = user.RoleId,
                role = userRoleName,
                customerId,
                sellerId,
                deliveryPartnerId,
                warehouseManagerId,
                token
            });
        }

        [HttpPost("ForgotPassword/FindAccount")]
        public async Task<IActionResult> FindAccountForForgot([FromBody] FindAccountDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Identifier))
            {
                return BadRequest("Please enter your registered email, username, or phone number.");
            }

            var cleanId = dto.Identifier.Trim().ToLower();
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == cleanId ||
                                          u.Username.ToLower() == cleanId ||
                                          u.PhoneNumber == dto.Identifier.Trim());

            if (user == null)
            {
                return NotFound("Account not found. Please verify your registered email, username, or phone number.");
            }

            string maskedEmail = MaskEmail(user.Email);
            string maskedPhone = MaskPhone(user.PhoneNumber ?? "");

            return Ok(new
            {
                message = "Account located.",
                userId = user.UserId,
                username = user.Username,
                identifier = user.Email,
                maskedEmail = maskedEmail,
                maskedPhone = maskedPhone,
                registeredEmail = maskedEmail,
                registeredPhone = maskedPhone,
                hasEmail = !string.IsNullOrWhiteSpace(user.Email),
                hasPhone = !string.IsNullOrWhiteSpace(user.PhoneNumber)
            });
        }

        [HttpPost("ForgotPassword/SendOtp")]
        public async Task<IActionResult> SendForgotOtp([FromBody] SendForgotOtpDTO dto)
        {
            if (dto == null || (string.IsNullOrWhiteSpace(dto.Identifier) && !dto.UserId.HasValue))
            {
                return BadRequest("Registered email, phone number, or user ID is required.");
            }

            User? user = null;
            if (dto.UserId.HasValue)
            {
                user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == dto.UserId.Value);
            }

            if (user == null && !string.IsNullOrWhiteSpace(dto.Identifier))
            {
                var cleanId = dto.Identifier.Trim().ToLower();
                user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == cleanId ||
                                              u.Username.ToLower() == cleanId ||
                                              u.PhoneNumber == dto.Identifier.Trim());
            }

            if (user == null)
            {
                return NotFound("Account not found. Please verify your registered details.");
            }

            var (success, message, otpCode) = await _otpService.GenerateOtpAsync(user.Email, "ResetPassword");
            if (!success)
            {
                return BadRequest(message);
            }

            string method = (!string.IsNullOrWhiteSpace(dto.Channel) ? dto.Channel : dto.DeliveryMethod)?.Trim().ToLower() ?? "email";
            string target = method == "sms" ? MaskPhone(user.PhoneNumber ?? "") : MaskEmail(user.Email);

            if (method == "sms" && !string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                await _smsService.SendOtpSmsAsync(user.PhoneNumber, user.Username, otpCode, expirationMinutes: 5);
            }
            else
            {
                await _emailService.SendOtpEmailAsync(user.Email, user.Username, otpCode, expirationMinutes: 5);
            }

            return Ok(new
            {
                message = $"Verification OTP sent via {method.ToUpper()} to {target}.",
                maskedTarget = target,
                devOtpCode = otpCode
            });
        }

        [HttpPost("ForgotPassword/VerifyOtp")]
        public async Task<IActionResult> VerifyForgotOtp([FromBody] VerifyForgotOtpDTO dto)
        {
            string otp = (!string.IsNullOrWhiteSpace(dto?.OtpCode) ? dto.OtpCode : dto?.Otp)?.Trim() ?? "";
            if (dto == null || (string.IsNullOrWhiteSpace(dto.Identifier) && !dto.UserId.HasValue) || string.IsNullOrWhiteSpace(otp))
            {
                return BadRequest("User identification and 6-digit OTP code are required.");
            }

            User? user = null;
            if (dto.UserId.HasValue)
            {
                user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == dto.UserId.Value);
            }

            if (user == null && !string.IsNullOrWhiteSpace(dto.Identifier))
            {
                var cleanId = dto.Identifier.Trim().ToLower();
                user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == cleanId || u.PhoneNumber == dto.Identifier.Trim());
            }

            if (user == null)
            {
                return NotFound("Account not found.");
            }

            var (success, message) = await _otpService.VerifyOtpAsync(user.Email, otp, "ResetPassword");
            if (!success)
            {
                return BadRequest(message);
            }

            return Ok(new
            {
                message = "OTP verified successfully. You may now create a new password.",
                resetToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user.UserId}:{user.Email}:{DateTime.UtcNow.Ticks}"))
            });
        }

        [HttpPost("ForgotPassword/ResetPassword")]
        public async Task<IActionResult> ResetPasswordWithOtp([FromBody] ResetPasswordDTO dto)
        {
            string otp = (!string.IsNullOrWhiteSpace(dto?.OtpCode) ? dto.OtpCode : dto?.ResetToken)?.Trim() ?? "";
            if (dto == null || (string.IsNullOrWhiteSpace(dto.Identifier) && !dto.UserId.HasValue) || string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                return BadRequest("All reset fields (User, OTP / Reset Token, and New Password) are required.");
            }

            if (dto.NewPassword.Length < 6)
            {
                return BadRequest("New password must be at least 6 characters long.");
            }

            User? user = null;
            if (dto.UserId.HasValue)
            {
                user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == dto.UserId.Value);
            }

            if (user == null && !string.IsNullOrWhiteSpace(dto.Identifier))
            {
                var cleanId = dto.Identifier.Trim().ToLower();
                user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == cleanId || u.PhoneNumber == dto.Identifier.Trim());
            }

            if (user == null)
            {
                return NotFound("Account not found.");
            }

            if (!string.IsNullOrWhiteSpace(dto.OtpCode))
            {
                var (success, message) = await _otpService.VerifyOtpAsync(user.Email, dto.OtpCode.Trim(), "ResetPassword");
                if (!success)
                {
                    return BadRequest(message);
                }
            }

            user.PasswordHash = PasswordHasher.Hash(dto.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password successfully reset! You may now sign in with your new password." });
        }

        private static string MaskEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@")) return "e***l@domain.com";
            var parts = email.Split('@');
            string name = parts[0];
            string domain = parts[1];
            if (name.Length <= 2) return $"{name[0]}***@{domain}";
            return $"{name[0]}***{name[^1]}@{domain}";
        }

        private static string MaskPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone) || phone.Length < 6) return "+91 ******1234";
            return $"{phone[..3]}******{phone[^4..]}";
        }

        private string GenerateToken(User user)
        {
            var roleName = GetRoleName(user.RoleId);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("RoleId", user.RoleId.ToString()),
                new Claim(ClaimTypes.Role, roleName)
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

        private static string GetRoleName(int roleId)
        {
            return RoleIds.FirstOrDefault(role => role.Value == roleId).Key ?? "User";
        }
    }
}
