using ComplaignManagementSystem.Data.Context;
using ComplaignManagementSystem.Data.Models;
using ComplaintManagementSystem.Business.Authentication;
using ComplaintManagementSystem.Business.ComplaintManageProcessHandler;
using ComplaintManagementSystem.Business.ConncetionHandler;
using ComplaintManagementSystem.Business.DepartmentHandler;
//using ComplaintManagementSystem.Business.EmailHandler;
using ComplaintManagementSystem.Business.EmailHandler;
using ComplaintManagementSystem.Business.Helpers;
using ComplaintManagementSystem.Business.LoginHandler;
using ComplaintManagementSystem.Business.MethodHandler;
using ComplaintManagementSystem.Business.NatureHandler;
using ComplaintManagementSystem.Business.PageCapabilityHandler;
using ComplaintManagementSystem.Business.PageHandler;
using ComplaintManagementSystem.Business.UserRoleHandler;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ComplaignManagementSystem.EmailService
{
    public class Program
    {
        public static void Main(string[] args)
        {

            Console.WriteLine("Hello, World!");

            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            IConfiguration config = builder.Build();

            var serviceProvider = new ServiceCollection()
               .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
               .AddScoped<DapperContext>()
               .AddScoped<_ConnectionService>()
               .AddScoped<PasswordHelper>()
               .AddScoped<ADAuthentication>()
               .AddScoped<IComplaintManageProcessService, ComplaintManageProcessService>()
               .AddScoped<IUserService, UserService>()
               .AddScoped<IDepartmentService, DepartmentService>()
               .AddScoped<INatureService, NatureService>()
               .AddScoped<IMethodService, MethodService>()
               .AddScoped<IUserRoleService, UserRoleService>()
               .AddScoped<IPageService, PageService>()
               .AddScoped<IPageCapabilityService, PageCapabilityService>()
               .AddScoped<IEmailService, ComplaintManagementSystem.Business.EmailHandler.EmailService>()
               .AddScoped<IEmailTemplateRenderer, EmailTemplateRenderer>()
               .AddScoped<ComplaintEmailService>()
               .AddSingleton(config)
               .BuildServiceProvider();

            try
            {

                //string Time;
                //Time = "03:02 PM";
                //string currentTime = DateTime.Now.ToString("hh:mm tt");

                var console = serviceProvider.GetRequiredService<ComplaintEmailService>();
                Console.WriteLine("start send Approval Email!");
                console.SendApprovalEmail();
                Console.WriteLine("start send Centrail Email!");
                console.SendCentrailEmail();

                //console.SendEmail2();

                //if (Time == System.DateTime.Now.ToString("hh:mm tt"))
                //    console.SendEmail();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
