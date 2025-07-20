using Microsoft.EntityFrameworkCore;
using SSMS.Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<SSMSContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddAuthentication("SSMSAuth")
    .AddCookie("SSMSAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });
// you look good!
builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
