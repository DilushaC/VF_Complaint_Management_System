using ComplaignManagementSystem.Data.Models;
using ComplaignManagementSystem.Presentation.Filters;
using ComplaintManagementSystem.Business.NatureHandler;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace ComplaignManagementSystem.Presentation.Controllers
{
    [SessionCheck]
    public class NatureController : Controller
    {
        private readonly INatureService _nature;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NatureController(INatureService natureService, IWebHostEnvironment webHostEnvironment)
        {
            _nature = natureService;
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
                var NatureList = _nature.getAllList();
                var getAllDeps = _nature.getDepList();

                ViewBag.Dep_Id = new SelectList(getAllDeps.Result.ToList(), "Id", "Name");
                ViewBag.NatureList = NatureList.ToList();

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
                _nature.CreateNature(collection);
                TempData["ToastMessage"] = "SubmittedNatureSuccessfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        //GET: DepartmentController/Details/5
        public async Task<IActionResult> NaturetDetails(int id)
        {
            NatureModel nature = _nature.getNatureListId(id);
            return PartialView("_DetailsNaturePartial", nature);
        }

        public async Task<IActionResult> Edit(int id)
        {
            // Simulate fetching from database                        
            NatureModel nature = _nature.getNatureListId(id);
            var getAllDeps = _nature.getDepList();
            ViewBag.Dep_Id = new SelectList(getAllDeps.Result.ToList(), "Id", "Name", nature.Dep_Id);
            return PartialView("_EditNaturePartial", nature);
        }

        // POST: DepartmentController/Edit/5
        [HttpPost]
        public ActionResult Edit(IFormCollection collection)
        {
            try
            {
                _nature.UpdateNature(collection);
                TempData["ToastMessage"] = "UpdatedNatureSuccessfully!";
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
                _nature.DeleteNature(id);
                TempData["ToastMessage"] = "DeletedNatureSuccessfully!";
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
