using ComplaignManagementSystem.Data.Models;
using ComplaignManagementSystem.Presentation.Filters;
using ComplaintManagementSystem.Business.ComplaintManageProcessHandler;
using ComplaintManagementSystem.Business.DepartmentHandler;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Threading.Tasks;

namespace ComplaignManagementSystem.Presentation.Controllers
{
    [SessionCheck]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _department;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DepartmentController(IDepartmentService departmentService, IWebHostEnvironment webHostEnvironment)
        {
            _department = departmentService;
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
                var dep = _department.getAllList();
                
                ViewBag.DepartmentList = dep.ToList();

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
                _department.CreateDepartment(collection);
                TempData["ToastMessage"] = "SubmittedDepSuccessfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: DepartmentController/Details/5
        public async Task<IActionResult> DepartmentDetails(int id)
        {
            DepartmentModel department = _department.getDepListId(id);
            return PartialView("_DetailsDepPartial", department);
        }

        public async Task<IActionResult> Edit(int id)
        {
            // Simulate fetching from database                        
            DepartmentModel department = _department.getDepListId(id);
            return PartialView("_EditDepPartial", department);
        }

        // POST: DepartmentController/Edit/5
        [HttpPost]
        public ActionResult Edit(IFormCollection collection)
        {
            try
            {
                _department.UpdateDepartment(collection);
                TempData["ToastMessage"] = "UpdatedDepSuccessfully!";
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
                _department.DeleteDepartment(id);
                TempData["ToastMessage"] = "DeletedDepSuccessfully!";
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
