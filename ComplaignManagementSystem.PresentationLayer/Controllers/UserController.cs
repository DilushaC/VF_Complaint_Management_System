using ComplaignManagementSystem.Data.Models;
using ComplaignManagementSystem.Presentation.Filters;
using ComplaintManagementSystem.Business.LoginHandler;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ComplaignManagementSystem.Presentation.Controllers
{
    [SessionCheck]
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

                var DepDetails = _loginService.GetDepartmentDetails(Convert.ToInt32(user.Dep_Id));

                HttpContext.Session.SetString("UserName", user.UserName);
                HttpContext.Session.SetString("UserDepName", DepDetails.Name);
                HttpContext.Session.SetString("UserDep_Id", Convert.ToString(user.Dep_Id));
                HttpContext.Session.SetString("SaltKey", Convert.ToString(user.SaltKey));
                HttpContext.Session.SetString("UserId", Convert.ToString(user.Id));

                if (user.IsReset == false)
                {
                    //return RedirectToAction("Reset");
                    return Json(new { success = false, redirectUrl = Url.Action("Reset", "User") });
                }

                UserPermissionModel getPermissions = _loginService.getAccessPerimissions(user);
                var getAccessPages = _loginService.getAccessPages(user, getPermissions);

                var DepUCount = getAccessPages.Where(a => a.Page == "Department Master" && a.Active == true).Count();
                var CentUCount = getAccessPages.Where(a => a.Page == "Central Master" && a.Active == true).Count();

                HttpContext.Session.SetString("UserPermission", getPermissions.Role);
                if(getPermissions.Role != "User" && DepUCount != 0)
                {
                    HttpContext.Session.SetString("DepartmentPermission", "Department User");
                }
                else
                {
                    HttpContext.Session.SetString("DepartmentPermission", "");
                }
                if(getPermissions.Role != "User" && CentUCount != 0)
                {
                    HttpContext.Session.SetString("CentralPermission", "Central User");
                }
                else
                {
                    HttpContext.Session.SetString("CentralPermission", "");
                }



                var jsonData = JsonConvert.SerializeObject(getAccessPages);

                HttpContext.Session.SetString("AccessPages", jsonData);
                return Json(new { success = true, redirectUrl = Url.Action("Dashboard", "ComplaintManageProcess") });
            }
            else
            {
                return Json(new { success = false});
            }

        }

        public ActionResult Reset()
        {
            //HttpContext.Session.Clear();
            return View();
        }


        [HttpPost]
        public IActionResult ResetPassword(string NewPassword, string ConPassword)
        {
            try
            {
                var UserId = HttpContext.Session.GetString("UserId");
                var SaltKey = HttpContext.Session.GetString("SaltKey");

                _loginService.ResetPassword(UserId, SaltKey, NewPassword);
                return Json(new { success = true, redirectUrl = Url.Action("Login", "User") });
                //return RedirectToAction(nameof(Login));
            }
            catch
            {
                //return View();
                return Json(new { success = false});
            }
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
