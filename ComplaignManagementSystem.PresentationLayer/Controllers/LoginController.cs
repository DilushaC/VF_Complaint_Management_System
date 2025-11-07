using ComplaignManagementSystem.Presentation.Filters;
using ComplaintManagementSystem.Business.LoginHandler;
using Microsoft.AspNetCore.Mvc;

namespace ComplaignManagementSystem.Presentation.Controllers
{
    [SessionCheck]
    public class LoginController : Controller
    {
        private readonly IUserService _loginService;

        public LoginController(IUserService loginService)
        {
            _loginService = loginService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/User/Login.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _loginService.ValidateUserAsync(username, password);

            if (user != null)
            {
                HttpContext.Session.SetString("UserName", user.UserName);
                return RedirectToAction("Dashboard", "ComplaintManageProcess");
            }

            ViewBag.Error = "Invalid username or password.";
            return View("~/Views/User/Login.cshtml");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); 
            return RedirectToAction("Index", "Login"); 
        }
    }
}
