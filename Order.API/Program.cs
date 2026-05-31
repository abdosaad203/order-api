using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Order.API.Data;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// MySQL Connection
var connectionString =
    builder.Configuration.GetConnectionString("Default")
    ?? builder.Configuration.GetConnectionString("MySQL");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0))
    ));

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Catalog API Client
builder.Services.AddHttpClient("CatalogApi", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["CatalogApi:BaseUrl"]
        ?? "http://catalog-api-backend:8080"
    );
});

// Simple healthchecks only
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseRouting();

app.UseHttpMetrics();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");
app.MapMetrics();

app.Run();