using Ecommerce.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Controllers + JSON settings

builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler =
    System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});


// Database Connection

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(
            builder.Configuration.GetConnectionString("DefaultConnection")
        )
    );
});



// JWT Authentication

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;

})

.AddJwtBearer(options =>
{

    options.TokenValidationParameters =
    new TokenValidationParameters
    {

        ValidateIssuer = true,

        ValidateAudience = true,

        ValidateLifetime = true,

        ValidateIssuerSigningKey = true,


        ValidIssuer =
        builder.Configuration["Jwt:Issuer"],


        ValidAudience =
        builder.Configuration["Jwt:Audience"],


        IssuerSigningKey =
        new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"]!
            )
        )

    };

});




// CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
    policy =>
    {
        policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});



// Swagger

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{

    options.AddSecurityDefinition("Bearer",
    new OpenApiSecurityScheme
    {

        Name = "Authorization",

        Type = SecuritySchemeType.Http,

        Scheme = "Bearer",

        BearerFormat = "JWT",

        In = ParameterLocation.Header,

        Description =
        "Enter JWT Token like this: Bearer your_token_here"

    });



    options.AddSecurityRequirement(
    new OpenApiSecurityRequirement
    {

        {
            new OpenApiSecurityScheme
            {

                Reference =
                new OpenApiReference
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



// Middleware

app.UseCors("AllowReact");



if (app.Environment.IsDevelopment())
{

    app.UseSwagger();

    app.UseSwaggerUI();

}



// Keep HTTPS redirection

app.UseHttpsRedirection();



app.UseAuthentication();

app.UseAuthorization();


app.UseStaticFiles();
app.MapControllers();



app.Run();