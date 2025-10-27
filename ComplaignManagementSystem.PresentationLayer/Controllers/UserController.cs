using ComplaignManagementSystem.Data.Models;
using ComplaintManagementSystem.Business.LoginHandler;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ComplaignManagementSystem.Presentation.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _loginService;

        public UserController(IUserService loginService)
        {
            _loginService = loginService;
        }
        // GET: UserController
        public ActionResult Login()        
        {
            HttpContext.Session.Clear();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            UserModel user = await _loginService.ValidateUserAsync(username, password);

            if (user != null)
            {

                UserPermissionModel getPermissions = _loginService.getAccessPerimissions(user);
                var getAccessPages = _loginService.getAccessPages(user, getPermissions);

                HttpContext.Session.SetString("UserName", user.UserName);
                HttpContext.Session.SetString("UserDep_Id", Convert.ToString(user.Dep_Id));
                var DepUCount = getAccessPages.Where(a => a.Page == "Department Master" && a.Active == true).Count();
                var CentUCount = getAccessPages.Where(a => a.Page == "Central Master" && a.Active == true).Count();

                HttpContext.Session.SetString("UserPermission", getPermissions.Role);
                if(DepUCount != 0)
                {
                    HttpContext.Session.SetString("DepartmentPermission", "Department User");
                }
                else
                {
                    HttpContext.Session.SetString("DepartmentPermission", null);
                }
                if(CentUCount != 0)
                {
                    HttpContext.Session.SetString("CentralPermission", "Central User");
                }
                else
                {
                    HttpContext.Session.SetString("CentralPermission", null);
                }



                var jsonData = JsonConvert.SerializeObject(getAccessPages);

                HttpContext.Session.SetString("AccessPages", jsonData);

                //HttpContext.Session.SetString("AccessPages", getAccessPages);
                //return RedirectToAction("Dashboard", "ComplaintManageProcess");
                return Json(new { success = true, redirectUrl = Url.Action("Dashboard", "ComplaintManageProcess") });
            }
            else
            {
                return Json(new { success = false});
            }

            //    ViewBag.Error = "Invalid username or password.";
            //return View("~/Views/User/Login.cshtml");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Save new user to database
                return RedirectToAction("Index", "Login");
            }

            ViewBag.Error = "Please fill all required fields correctly.";
            return View(model);
        }




        // GET: UserController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
