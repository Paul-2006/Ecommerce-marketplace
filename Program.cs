using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.Services;
using Ecommerce.Services.Verification;
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

// Verification Services DI Registration
builder.Services.AddScoped<IGstVerificationService, GstVerificationService>();
builder.Services.AddScoped<IPanVerificationService, PanVerificationService>();
builder.Services.AddScoped<IDrivingLicenceVerificationService, DrivingLicenceVerificationService>();
builder.Services.AddScoped<IVehicleVerificationService, VehicleVerificationService>();
builder.Services.AddScoped<IDocumentVerificationService, DocumentVerificationService>();
builder.Services.AddScoped<IDigiLockerService, DigiLockerService>();

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
        options.UseInMemoryDatabase("EcommerceMarketplaceDb")
               .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning));
    }
});

// JWT Authentication
var rawJwtKey = builder.Configuration["Jwt:Key"] ?? "ThisIsMySecretKeyForEcommerceApplication12345";
if (rawJwtKey.Length < 32) rawJwtKey = rawJwtKey.PadRight(32, 'X');

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
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "EcommerceAPI",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "EcommerceClient",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(rawJwtKey))
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

// Dynamic CORS - Supports Configured Production Frontend Domains and Development Fallback
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        var origins = builder.Configuration.GetSection("AllowedOrigins:Origins").Get<string[]>()
                      ?? builder.Configuration["FrontendUrl"]?.Split(',', StringSplitOptions.RemoveEmptyEntries);

        if (origins != null && origins.Length > 0)
        {
            policy.WithOrigins(origins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
        else
        {
            policy.SetIsOriginAllowed(_ => true)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
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

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ecommerce API v1");
    c.RoutePrefix = "swagger";
});

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

// Health Check Endpoints
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", environment = app.Environment.EnvironmentName, timestamp = DateTime.UtcNow }));
app.MapGet("/api/health", () => Results.Ok(new { status = "Healthy", environment = app.Environment.EnvironmentName, timestamp = DateTime.UtcNow }));

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

    try
    {
        DbSeeder.SeedAll(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ADMIN SEED NOTICE] {ex.Message}");
    }
}

app.Run();
