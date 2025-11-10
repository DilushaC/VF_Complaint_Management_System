using ComplaignManagementSystem.Data.Context;
using ComplaignManagementSystem.Presentation.Filters;
using ComplaintManagementSystem.Business.ComplaintManageProcessHandler;
using ComplaintManagementSystem.Business.ConncetionHandler;
using ComplaintManagementSystem.Business.DepartmentHandler;
using ComplaintManagementSystem.Business.Helpers;
using ComplaintManagementSystem.Business.LoginHandler; 
using ComplaintManagementSystem.Business.MethodHandler;
using ComplaintManagementSystem.Business.NatureHandler;
using ComplaintManagementSystem.Business.PageCapabilityHandler;
using ComplaintManagementSystem.Business.PageHandler;
using ComplaintManagementSystem.Business.UserRoleHandler;
using log4net;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<SessionCheckAttribute>();
});

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<DapperContext>();

builder.Services.AddScoped<_ConnectionService>();

builder.Services.AddScoped<PasswordHelper>();

builder.Services.AddScoped<IComplaintManageProcessService, ComplaintManageProcessService>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IDepartmentService, DepartmentService>();

builder.Services.AddScoped<INatureService, NatureService>();

builder.Services.AddScoped<IMethodService, MethodService>();

builder.Services.AddScoped<IUserRoleService, UserRoleService>();

builder.Services.AddScoped<IPageService, PageService>();

builder.Services.AddScoped<IPageCapabilityService, PageCapabilityService>();

builder.Logging.ClearProviders(); // Optional: clear default providers
builder.Logging.AddLog4Net("log4net.config");

builder.Host.ConfigureLogging(logging =>
{
    logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.None);
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(1); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

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
    pattern: "{controller=User}/{action=Login}/{id?}");

app.Run();
