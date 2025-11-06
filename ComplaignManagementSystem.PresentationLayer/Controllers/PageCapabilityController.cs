using ComplaignManagementSystem.Data.Models;
using ComplaignManagementSystem.Presentation.Filters;
using ComplaintManagementSystem.Business.NatureHandler;
using ComplaintManagementSystem.Business.PageCapabilityHandler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ComplaignManagementSystem.Presentation.Controllers
{
    [SessionCheck]
    public class PageCapabilityController : Controller
    {
        private readonly IPageCapabilityService _pagecapability;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PageCapabilityController(IPageCapabilityService pageCapability, IWebHostEnvironment webHostEnvironment)
        {
            _pagecapability = pageCapability;
            _webHostEnvironment = webHostEnvironment;
        }
        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserName"));
        }

        public IActionResult Index()
        {
            try
            {
                if (!IsUserLoggedIn())
                    return RedirectToAction("Login", "User");
                var PageCapabilities = _pagecapability.getAllList();
                var getAllRoles = _pagecapability.getUserRoleList();
                var getAllPages = _pagecapability.getPageList();

                ViewBag.UserRoleId = new SelectList(getAllRoles.Result.ToList(), "Id", "Role");
                ViewBag.PageId = new SelectList(getAllPages.Result.ToList(), "Id", "Page");
                ViewBag.PageCapabilityList = PageCapabilities.ToList();

                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                _pagecapability.Create(collection);
                TempData["ToastMessage"] = "SubmittedSuccessfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public JsonResult CheckAvailability(int roleId, int pageId)
        {
            try
            {
                bool status = _pagecapability.CheckAvailability(roleId, pageId);
                //TempData["ToastMessage"] = "DeletedNatureSuccessfully!";
                return Json(new { status });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            // Simulate fetching from database                        
            PageCapabilityModel pageCapability = _pagecapability.getPageCapListId(id);
            var getAllRoles = _pagecapability.getUserRoleList();
            var getAllPages = _pagecapability.getPageList();

            ViewBag.UserRoleId = new SelectList(getAllRoles.Result.ToList(), "Id", "Role", pageCapability.UserRoleId);
            ViewBag.PageId = new SelectList(getAllPages.Result.ToList(), "Id", "Page", pageCapability.PageId);
            return PartialView("_EditPartial", pageCapability);
        }

        // POST: DepartmentController/Edit/5
        [HttpPost]
        public ActionResult Edit(IFormCollection collection)
        {
            try
            {
                _pagecapability.Update(collection);
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
                _pagecapability.Delete(id);
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
