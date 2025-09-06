using BookFinalAPI.Data;
using BookFinalAPI.Models;
using BookFinalAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Serilog logging to console (Heroku-friendly)
// ----------------------
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// ----------------------
// Swagger with JWT support
// ----------------------
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookFinalAPI", Version = "v1" });

    // JWT Bearer auth
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and your valid JWT token."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

// ----------------------
// Database Connection
// ----------------------
var configuration = builder.Configuration;
var services = builder.Services;

// Use environment variable first, fallback to appsettings.json
var conn = Environment.GetEnvironmentVariable("DefaultConnection") 
           ?? configuration.GetConnectionString("DefaultConnection");

// Add DbContext with Pomelo MySQL provider
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(conn, ServerVersion.AutoDetect(conn)));

// ----------------------
// Identity
// ----------------------
services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ----------------------
// Dependency Injection
// ----------------------
services.AddScoped<IOTPService, OTPService>();
services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// ----------------------
// Dynamic port for Heroku
// ----------------------
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://*:{port}");

// ----------------------
// Middleware
// ----------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Uncomment if you want to seed the DB on startup
// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     await DbInitializer.SeedAsync(services);
// }

app.Run();
