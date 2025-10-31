using ComplaignManagementSystem.Data.Models;
using ComplaignManagementSystem.Presentation.Filters;
using ComplaintManagementSystem.Business.DepartmentHandler;
using ComplaintManagementSystem.Business.MethodHandler;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ComplaignManagementSystem.Presentation.Controllers
{
    [SessionCheck]
    public class MethodController : Controller
    {
        private readonly IMethodService _method;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MethodController(IMethodService methodService, IWebHostEnvironment webHostEnvironment)
        {
            _method = methodService;
            _webHostEnvironment = webHostEnvironment;
        }

        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserName"));
        }

        // GET: DepartmentController
        public ActionResult Index()
        {
            try
            {
                if (!IsUserLoggedIn())
                    return RedirectToAction("Login", "User");
                var methods = _method.getAllList();

                ViewBag.MethodList = methods.ToList();

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
                _method.CreateMethod(collection);
                TempData["ToastMessage"] = "SubmittedMethodSuccessfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: DepartmentController/Details/5
        public async Task<IActionResult> MethodDetails(int id)
        {
            Complaint_Method_MasterModel methods = _method.getMethodListId(id);
            return PartialView("_DetailsMethodPartial", methods);
        }

        public async Task<IActionResult> Edit(int id)
        {
            // Simulate fetching from database                        
            Complaint_Method_MasterModel department = _method.getMethodListId(id);
            return PartialView("_EditMethodPartial", department);
        }

        // POST: DepartmentController/Edit/5
        [HttpPost]
        public ActionResult Edit(IFormCollection collection)
        {
            try
            {
                _method.UpdateMethod(collection);
                TempData["ToastMessage"] = "UpdatedMethodSuccessfully!";
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
                _method.DeleteMethod(id);
                TempData["ToastMessage"] = "DeletedMethodSuccessfully!";
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
