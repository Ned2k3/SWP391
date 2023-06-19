using Microsoft.EntityFrameworkCore;
using ProjectSWP391.Models;
using System.Configuration;
using X.PagedList.Mvc.Core;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<SWP391Context>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("PRNDB")));

builder.Services.AddSession(cfg =>
{
    cfg.Cookie.Name = "SWP391";
    cfg.IdleTimeout = new TimeSpan(0, 60, 0);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseSession();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "/{controller=CustomerManagement}/{action=LandingPage}/{id?}");

app.Run();
