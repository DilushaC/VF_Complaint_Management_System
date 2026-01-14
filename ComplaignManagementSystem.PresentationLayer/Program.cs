using ComplaignManagementSystem.Data.Context;
using ComplaignManagementSystem.Presentation.Filters;
using ComplaintManagementSystem.Business.Authentication;
using ComplaintManagementSystem.Business.ComplaintManageProcessHandler;
using ComplaintManagementSystem.Business.ConncetionHandler;
using ComplaintManagementSystem.Business.DepartmentHandler;
using ComplaintManagementSystem.Business.EmailHandler;
using ComplaintManagementSystem.Business.Helpers;
using ComplaintManagementSystem.Business.LoginHandler; 
using ComplaintManagementSystem.Business.MethodHandler;
using ComplaintManagementSystem.Business.NatureHandler;
using ComplaintManagementSystem.Business.PageCapabilityHandler;
using ComplaintManagementSystem.Business.PageHandler;
using ComplaintManagementSystem.Business.UserRoleHandler;
using DinkToPdf;
using DinkToPdf.Contracts;
using log4net;
using Microsoft.AspNetCore.Http;
using QuestPDF.Infrastructure;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<SessionCheckAttribute>();
});
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
//var context = new CustomAssemblyLoadContext();

//context.LoadUnmanagedLibrary(Path.Combine(
//    builder.Environment.ContentRootPath,
//    "wwwroot", "wkhtmltopdf", "libwkhtmltox.dll"));

//QuestPDF.Settings.License = LicenseType.Community;


var dllPath = Path.Combine(AppContext.BaseDirectory, "wkhtmltopdf", "libwkhtmltox.dll");
System.Runtime.InteropServices.NativeLibrary.Load(dllPath);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<DapperContext>();

builder.Services.AddScoped<_ConnectionService>();

builder.Services.AddScoped<PasswordHelper>();

builder.Services.AddScoped<ADAuthentication>();

builder.Services.AddScoped<IComplaintManageProcessService, ComplaintManageProcessService>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IDepartmentService, DepartmentService>();

builder.Services.AddScoped<INatureService, NatureService>();

builder.Services.AddScoped<IMethodService, MethodService>();

builder.Services.AddScoped<IUserRoleService, UserRoleService>();

builder.Services.AddScoped<IPageService, PageService>();

builder.Services.AddScoped<IPageCapabilityService, PageCapabilityService>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IEmailTemplateRenderer, EmailTemplateRenderer>();

builder.Logging.ClearProviders(); // Optional: clear default providers
builder.Logging.AddLog4Net("log4net.config");

builder.Host.ConfigureLogging(logging =>
{
    logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.None);
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
    //options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
    //        ? CookieSecurePolicy.None
    //        : CookieSecurePolicy.Always;
    //options.Cookie.SameSite = SameSiteMode.Strict;
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
app.UseHsts();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}");

app.Run();
