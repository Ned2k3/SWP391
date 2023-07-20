using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ProjectSWP391.Models;
using System.Configuration;
using System.Net;
using X.PagedList.Mvc.Core;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddCors();
builder.Services.AddDbContext<SWP391_V4Context>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));


builder.Services.AddSession(cfg =>
{
    cfg.Cookie.Name = "SWP391";
    cfg.IdleTimeout = new TimeSpan(0, 60, 0);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie("Auth", options =>
{
    options.LoginPath = "/Authentication/Login";
    options.AccessDeniedPath = "/error/http403";
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            if (!context.Request.Path.StartsWithSegments("/error"))
            {
                context.Response.Redirect("/error/http401");
            }
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseSession();
app.UseStaticFiles();

app.UseCors(opt => opt.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "/{controller=CustomerManagement}/{action=LandingPage}/{id?}");

app.Run();
