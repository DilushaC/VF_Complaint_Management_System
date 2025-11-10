using ComplaignManagementSystem.Data.Models;
using ComplaignManagementSystem.Presentation.Filters;
using ComplaintManagementSystem.Business.LoginHandler;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComplaignManagementSystem.Presentation.Controllers
{
    [SessionCheck]
    public class UserController : Controller
    {
        private readonly IUserService _loginService;
        private static readonly ILog log = LogManager.GetLogger(typeof(UserController));

        public UserController(IUserService loginService)
        {
            _loginService = loginService;
        }

        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserName"));
        }

        // GET: UserController
        public ActionResult Login()
        {
            Response.Cookies.Delete(".AspNetCore.Session");
            HttpContext.Session.Clear();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                UserModel user = await _loginService.ValidateUserAsync(username, password);

                if (user != null)
                {
                    var DepDetails = _loginService.GetDepartmentDetails(Convert.ToInt32(user.Dep_Id));

                    HttpContext.Session.SetString("UserName", user.UserName);
                    HttpContext.Session.SetString("UserDepName", DepDetails.Name);
                    HttpContext.Session.SetString("UserDep_Id", Convert.ToString(user.Dep_Id));
                    //HttpContext.Session.SetString("SaltKey", Convert.ToString(user.SaltKey));
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
                    if (getPermissions.Role != "User" && DepUCount != 0)
                    {
                        HttpContext.Session.SetString("DepartmentPermission", "Department User");
                    }
                    else
                    {
                        HttpContext.Session.SetString("DepartmentPermission", "");
                    }
                    if (getPermissions.Role != "User" && CentUCount != 0)
                    {
                        HttpContext.Session.SetString("CentralPermission", "Central User");
                    }
                    else
                    {
                        HttpContext.Session.SetString("CentralPermission", "");
                    }
                    var jsonData = JsonConvert.SerializeObject(getAccessPages);

                    HttpContext.Session.SetString("AccessPages", jsonData);
                    log.Info($"Success Login by : {user.UserName}.");
                    return Json(new { success = true, redirectUrl = Url.Action("Dashboard", "ComplaintManageProcess") });
                }
                else
                {
                    log.Info($"Failed login.");
                    return Json(new { success = false });
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error Login : {ex.Message}.");
                throw;
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
                var UserName = HttpContext.Session.GetString("UserName");

                _loginService.ResetPassword(UserId, SaltKey, NewPassword);
                log.Info($"Success Reset Password in {UserId}. by {UserName}.");
                return Json(new { success = true, redirectUrl = Url.Action("Login", "User") });
                //return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                //return View();
                log.Error($"Error Login : {ex.Message}.");
                return Json(new { success = false });
            }
        }

        [HttpGet]
        public IActionResult Register(IFormCollection form)
        {
            return View();
        }

        public IActionResult Create()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "User");

            var getAllDeps = _loginService.getDepList();
            var getAllBranches = _loginService.getBranchList();

            ViewBag.Dep_Id = new SelectList(getAllDeps.Result.ToList(), "Id", "Name");
            ViewBag.BranchId = new SelectList(getAllBranches.Result.ToList(), "Id", "Branch");

            return View();
        }

        public IActionResult Index()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "User");

            var List = _loginService.getAllList();

            ViewBag.UserList = List.ToList();

            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        public ActionResult CreateUser(IFormCollection collection)
        {
            try
            {
                var UserName = HttpContext.Session.GetString("UserName");
                _loginService.CreateUser(collection);
                TempData["ToastMessage"] = "SubmittedUserSuccessfully!";
                log.Info($"Success User Creation by : {UserName}.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                log.Error($"Error User Creation : {ex.Message}.");
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            // Simulate fetching from database                        
            UserModel user = await _loginService.getUserDetailId(id);
            var getAllDeps = _loginService.getDepList();
            var getAllBranches = _loginService.getBranchList();
            var hik = user.Password;
            ViewBag.Dep_Id = new SelectList(getAllDeps.Result.ToList(), "Id", "Name", user.Dep_Id);
            ViewBag.BranchId = new SelectList(getAllBranches.Result.ToList(), "Id", "Branch", user.BranchId);

            return PartialView("_EditPartial", user);
        }

        // POST: UserController/Edit/5
        [HttpPost]
        public ActionResult Edit(IFormCollection collection)
        {

            try
            {
                var UserName = HttpContext.Session.GetString("UserName");
                var ResetStatus = collection["Reset"].ToString();
                if (ResetStatus == "Yes")
                    _loginService.ResetPassword(collection);
                else
                    _loginService.UpdateUser(collection);

                if (ResetStatus == "Yes")
                    TempData["ToastMessage"] = "PasswordUpdatedSuccessfully!";
                else
                    TempData["ToastMessage"] = "UpdatedSuccessfully!";

                log.Info($"Success User Editted by : {UserName}. Record : {collection["Id"].ToString()}. ");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                log.Error($"Error User Editing : {ex.Message}.");
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public JsonResult Inactive(int id)
        {
            try
            {
                var UserName = HttpContext.Session.GetString("UserName");
                int status = 0;
                _loginService.InactiveActive(id, status);
                TempData["ToastMessage"] = "InactiveSuccessfully!";
                log.Info($"Success User Inactived by : {UserName}. Record : {id}. ");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                log.Error($"Error User Inactivation : {ex.Message}. Record : {id}.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Active(int id)
        {
            try
            {
                var UserName = HttpContext.Session.GetString("UserName");

                int status = 1;
                _loginService.InactiveActive(id, status);
                TempData["ToastMessage"] = "ActiveSuccessfully!";
                log.Info($"Success User Actived by : {UserName}. Record : {id}.");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                log.Error($"Error User Activation : {ex.Message}. Record : {id}.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> PermissionEdit(int id)
        {
            // Simulate fetching from database                        
            UserModel user = await _loginService.getUserDetailId(id);
            UserPermissionModel userP = await _loginService.getPermissionList(id);

            var getAllRoles = _loginService.getUserRoleList();

            ViewBag.UserList = user;
            if (userP != null)
                ViewBag.UserRoleId = new SelectList(getAllRoles.Result.ToList(), "Id", "Role", userP.UserRoleId);
            else
                ViewBag.UserRoleId = new SelectList(getAllRoles.Result.ToList(), "Id", "Role");

            return PartialView("_PermissionPartial", user);
        }

        [HttpPost]
        public ActionResult PermissionEdit(IFormCollection collection)
        {
            try
            {
                var UserName = HttpContext.Session.GetString("UserName");
                var UserRoleId = collection["UserRoleId"].ToString();
                var UserId = Convert.ToInt32(collection["Id"].ToString());
                var userP = _loginService.getPermissionList(Convert.ToInt32(UserId));
                var result = userP.Result;
                if (UserRoleId != "")
                    if (result == null)
                        _loginService.grantPermssion(collection);
                    else
                        _loginService.updatePermssion(collection);
                else
                    _loginService.deletePermission(collection);

                if (result == null)
                    TempData["ToastMessage"] = "PermissionGrantedSuccessfully!";
                else
                    TempData["ToastMessage"] = "PermissionUpdatedSuccessfully!";

                log.Info($"Success User Editted Permssion by : {UserName}. User Id : {UserId}. User Role Id : {UserRoleId}. ");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) 
            {
                log.Error($"Error User Activation : {ex.Message}.");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
