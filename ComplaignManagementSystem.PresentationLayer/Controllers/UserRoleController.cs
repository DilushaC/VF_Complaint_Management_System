using ComplaignManagementSystem.Data.Models;
using ComplaignManagementSystem.Presentation.Filters;
using ComplaintManagementSystem.Business.DepartmentHandler;
using ComplaintManagementSystem.Business.UserRoleHandler;
using Microsoft.AspNetCore.Mvc;

namespace ComplaignManagementSystem.Presentation.Controllers
{
    [SessionCheck]
    public class UserRoleController : Controller
    {
        private readonly IUserRoleService _userRole;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserRoleController(IUserRoleService userRoleService, IWebHostEnvironment webHostEnvironment)
        {
            _userRole = userRoleService;
            _webHostEnvironment = webHostEnvironment;
        }

        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserName"));
        }

        public ActionResult Index()
        {
            try
            {
                if (!IsUserLoggedIn())
                    return RedirectToAction("Login", "User");
                var dep = _userRole.getAllList();

                ViewBag.UserRoleList = dep.ToList();
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // POST: DepartmentController/Create
        [HttpPost]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                _userRole.Create(collection);
                TempData["ToastMessage"] = "SubmittedSuccessfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            UserRoleModel userrole = _userRole.getListId(id);
            return PartialView("_DetailsPartial", userrole);
        }

        public async Task<IActionResult> Edit(int id)
        {
            // Simulate fetching from database                        
            UserRoleModel userrole = _userRole.getListId(id);
            return PartialView("_EditPartial", userrole);
        }

        [HttpPost]
        public ActionResult Edit(IFormCollection collection)
        {
            try
            {
                _userRole.Update(collection);
                TempData["ToastMessage"] = "UpdatedSuccessfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                _userRole.Delete(id);
                TempData["ToastMessage"] = "DeletedSuccessfully!";
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
