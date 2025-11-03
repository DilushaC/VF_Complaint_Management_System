using ComplaignManagementSystem.Data.Models;
using ComplaignManagementSystem.Presentation.Filters;
using ComplaintManagementSystem.Business.PageHandler;
using ComplaintManagementSystem.Business.UserRoleHandler;
using Microsoft.AspNetCore.Mvc;

namespace ComplaignManagementSystem.Presentation.Controllers
{
    [SessionCheck]

    public class PageController : Controller
    {
        private readonly IPageService _page;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PageController(IPageService pageService, IWebHostEnvironment webHostEnvironment)
        {
            _page = pageService;
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
                var List = _page.getAllList();

                ViewBag.PageList = List.ToList();
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
                _page.Create(collection);
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
            PageModel pModel = _page.getListId(id);
            return PartialView("_DetailsPartial", pModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            // Simulate fetching from database                        
            PageModel pModel = _page.getListId(id);
            return PartialView("_EditPartial", pModel);
        }

        [HttpPost]
        public ActionResult Edit(IFormCollection collection)
        {
            try
            {
                _page.Update(collection);
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
                _page.Delete(id);
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
