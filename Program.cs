using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Services DI Registration
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddSingleton<IOtpService, OtpService>();
builder.Services.AddSingleton<IUserLoginLogger, UserLoginLogger>();

builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler =
    System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// Database Connection with Resilient Fallback
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connStr = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
    bool connected = false;

    if (!string.IsNullOrWhiteSpace(connStr))
    {
        try
        {
            options.UseMySql(connStr, ServerVersion.AutoDetect(connStr));
            connected = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DB NOTICE] MySQL connection unavailable ({ex.Message}). Using In-Memory Database for API resilience.");
        }
    }

    if (!connected)
    {
        options.UseInMemoryDatabase("EcommerceMarketplaceDb");
    }
});

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        )
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("RoleId", "1"));
    options.AddPolicy("SellerOnly", policy => policy.RequireClaim("RoleId", "2"));
    options.AddPolicy("WarehouseOnly", policy => policy.RequireClaim("RoleId", "3"));
    options.AddPolicy("DeliveryOnly", policy => policy.RequireClaim("RoleId", "4"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireClaim("RoleId", "5"));
});

// CORS - Allow All Origins for Frontend (Local and Production/Vercel)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Token like this: Bearer your_token_here"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

// Middleware Pipeline
app.UseCors("AllowReact");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable HTTPS Redirection only in non-development to avoid CORS 307 redirect issues on localhost:5151
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[DB INIT] {ex.Message}");
    }

    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var adminEmail = configuration["AdminSeed:Email"];
    var adminPassword = configuration["AdminSeed:Password"];
    var adminUsername = configuration["AdminSeed:Username"] ?? "Admin";

    try
    {
        if (!string.IsNullOrWhiteSpace(adminEmail) &&
            !string.IsNullOrWhiteSpace(adminPassword) &&
            !context.Users.Any(u => u.RoleId == 1))
        {
            context.Users.Add(new User
            {
                Username = adminUsername,
                Email = adminEmail,
                PasswordHash = PasswordHasher.Hash(adminPassword),
                RoleId = 1,
                AccountStatus = "Active",
                CreatedDate = DateTime.Now
            });

            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ADMIN SEED NOTICE] {ex.Message}");
    }
}

app.Run();
