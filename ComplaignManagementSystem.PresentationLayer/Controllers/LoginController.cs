using ComplaintManagementSystem.Business.LoginHandler;
using Microsoft.AspNetCore.Mvc;

namespace ComplaignManagementSystem.Presentation.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Explicitly load the view from Views/User/Login.cshtml
            return View("~/Views/User/Login.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Index(string username, string password)
        {
            var user = await _loginService.ValidateUserAsync(username, password);

            if (user != null)
            {
                // Example: Set session or redirect
                HttpContext.Session.SetString("UserName", user.UserName);
                return RedirectToAction("Dashboard", "ComplaintManageProcess");
            }

            // If login failed, show same User/Login view again with error
            ViewBag.Error = "Invalid username or password.";
            return View("~/Views/User/Login.cshtml");
        }
    }
}
