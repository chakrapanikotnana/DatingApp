using System.Text;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DatingAppConn"));
});
builder.Services.AddCors();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    var tokenKey = builder.Configuration["TokenKey"] ?? throw new Exception("Token Key not found in the app settings");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();
app.UseCors(p => p.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200"));
app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(context);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}

app.Run();
