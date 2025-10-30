using ComplaignManagementSystem.Data.Models;
using ComplaignManagementSystem.Presentation.Filters;
using ComplaintManagementSystem.Business.ComplaintManageProcessHandler;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ComplaignManagementSystem.Presentation.Controllers
{
    [SessionCheck]
    public class ComplaintManageProcessController : Controller
    {
        private readonly IComplaintManageProcessService _complainProcess;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ComplaintManageProcessController(IComplaintManageProcessService complainProcess, IWebHostEnvironment webHostEnvironment)
        {
            _complainProcess = complainProcess;
            _webHostEnvironment = webHostEnvironment;
        }

        public ActionResult Dashboard()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        public ActionResult Create()
        {
            var getAllDeps = _complainProcess.getDepList();
            var getAllMethods = _complainProcess.getMethodList();
            ViewBag.ComplaintMethod_Id = new SelectList(getAllMethods.Result.ToList(), "Id", "Method");
            ViewBag.Dep_Id = new SelectList(getAllDeps.Result.ToList(), "Id", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult SubmitComplain(IFormCollection collection, IFormFile file)
        {
            try
            {
                _complainProcess.CreateComplaint(collection, file);
                TempData["ToastMessage"] = "SubmittedSuccessfully!";
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
                PaginationResultsModel<ComplaintMaster> paginationResult = await _complainProcess.getComplaintList(pageNumber, pageSize, searchString);
                ViewBag.ComplainLists = paginationResult.Items;

                //Also pass the total count for building the pagination links
                ViewBag.TotalCount = paginationResult.TotalCount;
                ViewBag.PageSize = pageSize;
                ViewBag.PageNumber = pageNumber;
                //TempData["ToastMessage"] = "EditedSuccessfully!";

                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> ComplaintDetails(int id)
        {
            // SQL query to retrieve the master data for the given complaint ID
            
            ComplaintMaster complaintData = await _complainProcess.getComplainUsingId(id);
            if (complaintData == null)
            {
                return NotFound(); // Or return an error partial view
            }
            if (!string.IsNullOrEmpty(complaintData.AttachmentPath))
            {
                complaintData.AttachmentPath = Path.GetFileName(complaintData.AttachmentPath);
            }
            // Return the data to the partial view
            return PartialView("_ComplaintDetailsPartial", complaintData);
        }

        public IActionResult DownloadAttachment(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return NotFound("Filename is not specified.");
            }

            var path = Path.Combine(_webHostEnvironment.WebRootPath, "Attachments", fileName);

            if (!System.IO.File.Exists(path))
            {
                return NotFound("File not found.");
            }

            var mimeType = "application/octet-stream"; // A generic MIME type for file downloads
            var fileBytes = System.IO.File.ReadAllBytes(path);

            return File(fileBytes, mimeType, fileName);
        }

        public async Task<IActionResult> EditComplaint(int id)
        {
            // Simulate fetching from database
            Complaint_ManageProcessModel complaint = _complainProcess.getComplainProcessUsingId(id); // Replace with real data fetch
            var getAllDeps = _complainProcess.getDepList();
            var getAllMethods = _complainProcess.getMethodList();
            ComplaintMaster complaintData = await _complainProcess.getComplainUsingId(id);
            if (complaintData == null)
            {
                return NotFound(); // Or return an error partial view
            }
            if (!string.IsNullOrEmpty(complaintData.AttachmentPath))
            {
                complaint.AttachmentPath = Path.GetFileName(complaintData.AttachmentPath);
            }

            ViewBag.ComplaintMethod_Id = new SelectList(getAllMethods.Result.ToList(), "Id", "Method", complaint.ComplaintMethod_Id);
            ViewBag.Dep_Id = new SelectList(getAllDeps.Result.ToList(), "Id", "Name", complaint.Dep_Id);
            ViewBag.Nature_Id = new SelectList(getAllDeps.Result.ToList(), "Id", "Nature", complaint.Nature_Id);
            if (complaint == null)
            {
                return NotFound();
            }

            return PartialView("_EditComplaintPartial", complaint);
        }

        [HttpPost]
        public JsonResult DeleteAttachment(int id)
        {
            try
            {
                //string filePath = Path.Combine($"wwwroot/Attachments/_{id}");
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Attachments", $"_{id}.pdf");
                if (System.IO.File.Exists(filePath))
                {
                    //System.IO.File.Delete(filePath);
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        public ActionResult UpdateComplaint(IFormCollection collection, IFormFile file)
        {
            try
            {
                _complainProcess.UpdateComplaint(collection, file);
                TempData["ToastMessage"] = "EditedSuccessfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
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
