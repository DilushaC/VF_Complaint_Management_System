using ComplaignManagementSystem.Data.Context;
using ComplaintManagementSystem.Business.ComplaintManageProcessHandler;
using ComplaintManagementSystem.Business.ConncetionHandler;
using ComplaintManagementSystem.Business.Helpers;
using ComplaintManagementSystem.Business.LoginHandler; 
using Microsoft.AspNetCore.Http;
using ComplaignManagementSystem.Presentation.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<SessionCheckAttribute>();
});

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<DapperContext>();

builder.Services.AddScoped<_ConnectionService>();

builder.Services.AddScoped<IComplaintManageProcessService, ComplaintManageProcessService>();

builder.Services.AddScoped<PasswordHelper>();
builder.Services.AddScoped<ILoginService, LoginService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(1); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
