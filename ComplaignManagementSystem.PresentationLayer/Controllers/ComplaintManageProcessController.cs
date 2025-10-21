using ComplaignManagementSystem.Data.Models;
using ComplaintManagementSystem.Business.ComplaintManageProcessHandler;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ComplaignManagementSystem.Presentation.Controllers
{
    public class ComplaintManageProcessController : Controller
    {
        private readonly IComplaintManageProcessService _complainProcess;

        public ComplaintManageProcessController(IComplaintManageProcessService complainProcess)
        {
            _complainProcess = complainProcess;
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        // GET: ComplaintManageProcessController/Create
        public ActionResult Create()
        {
            var getAllDeps = _complainProcess.getDepList();
            var getAllMethods = _complainProcess.getMethodList();
            ViewBag.ComplaintMethod_Id = new SelectList(getAllMethods.Result.ToList(), "Id", "Method");
            ViewBag.Dep_Id = new SelectList(getAllDeps.Result.ToList(), "Id", "Name");

            return View();
        }

        // POST: ComplaintManageProcessController/Create
        [HttpPost]
        public ActionResult SubmitComplain(IFormCollection collection, IFormFile file)
        {
            try
            {
                _complainProcess.CreateComplaint(collection, file);
                TempData["ToastMessage"] = "Complaint submitted successfully!";
                TempData["ToastType"] = "success";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public IActionResult GetNaturesByDepartment(int depId)
        {
            var getAllMethods = _complainProcess.GetNaturesByDepartment(depId);
            // Return the filtered list as a JSON object
            return Json(getAllMethods.ToList());
        }

        // GET: ComplaintManageProcessController
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string searchString = null)
        {
            try
            {
                PaginationResultsModel<Complaint_ManageProcessModel> paginationResult = await _complainProcess.getComplaintList(pageNumber, pageSize, searchString);
                ViewBag.ComplainLists = paginationResult.Items;

                //Also pass the total count for building the pagination links
                ViewBag.TotalCount = paginationResult.TotalCount;
                ViewBag.PageSize = pageSize;
                ViewBag.PageNumber = pageNumber;
                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // GET: ComplaintManageProcessController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        

        // GET: ComplaintManageProcessController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ComplaintManageProcessController/Edit/5
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

        // GET: ComplaintManageProcessController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ComplaintManageProcessController/Delete/5
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
