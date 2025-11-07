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

        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserName"));
        }

        public ActionResult Dashboard()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "User");

            ViewBag.CurrYear = System.DateTime.Now.Year;
            return View();
        }

        public ActionResult Create()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "User");
            var getAllDeps = _complainProcess.getDepList();
            var getAllMethods = _complainProcess.getMethodList();
            ViewBag.ComplaintMethod_Id = new SelectList(getAllMethods.Result.ToList(), "Id", "Method");
            ViewBag.Dep_Id = new SelectList(getAllDeps.Result.ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult SaveComplaint(IFormCollection collection, IFormFile file)
        {
            try
            {
                if (!IsUserLoggedIn())
                    return RedirectToAction("Login", "User");

                _complainProcess.CreateComplaint(collection, file);
                TempData["ToastMessage"] = "SubmittedSuccessfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult SubmitAndSendComplain(IFormCollection collection, IFormFile file)
        {
            try
            {
                if (!IsUserLoggedIn())
                    return RedirectToAction("Login", "User");

                _complainProcess.CreateAndSendComplaint(collection, file);
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
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string searchString = null, string ComplaintMethod_Id = null, string priority = null)
        {
            try
            {
                if (!IsUserLoggedIn())
                    return RedirectToAction("Login", "User");

                PaginationResultsModel<ComplaintMaster> paginationResult = await _complainProcess.getComplaintList(pageNumber, pageSize, searchString, ComplaintMethod_Id, priority);
                ViewBag.ComplainLists = paginationResult.Items;
                var getAllMethods = _complainProcess.getMethodList();
                if (ComplaintMethod_Id == null)
                    ViewBag.ComplaintMethod_Id = new SelectList(getAllMethods.Result.ToList(), "Id", "Method");
                else
                    ViewBag.ComplaintMethod_Id = new SelectList(getAllMethods.Result.ToList(), "Id", "Method", Convert.ToInt32(ComplaintMethod_Id));
                //Also pass the total count for building the pagination links
                ViewBag.TotalCount = paginationResult.TotalCount;
                ViewBag.PageSize = pageSize;
                ViewBag.PageNumber = pageNumber;
                ViewBag.SearchString = searchString;
                ViewBag.Priority = priority;

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
                if (!IsUserLoggedIn())
                    return RedirectToAction("Login", "User");

                _complainProcess.UpdateComplaint(collection, file);
                TempData["ToastMessage"] = "EditedSuccessfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult UpdateSendComplaint(IFormCollection collection, IFormFile file)
        {
            try
            {
                if (!IsUserLoggedIn())
                    return RedirectToAction("Login", "User");

                _complainProcess.UpdateSendComplaint(collection, file);

                var ResolvedStatus = collection["ResolvedStatus"].ToString();

                if (ResolvedStatus == "No")
                {
                    TempData["ToastMessage"] = "sentDepSuccessfully!";
                }
                else
                {
                    TempData["ToastMessage"] = "resolvedSuccessfully!";
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<IActionResult> DepartmentProcess(int pageNumber = 1, int pageSize = 10, string searchString = null, string ComplaintMethod_Id = null, string priority = null)
        {
            try
            {
                if (!IsUserLoggedIn())
                    return RedirectToAction("Login", "User");

                PaginationResultsModel<ComplaintMaster> paginationResult = await _complainProcess.getDepComplaintList(pageNumber, pageSize, searchString, ComplaintMethod_Id, priority);
                ViewBag.ComplainLists = paginationResult.Items.OrderByDescending(a => a.Status == "Sent Department").ToList();
                var getAllMethods = _complainProcess.getMethodList();
                if (ComplaintMethod_Id == null)
                    ViewBag.ComplaintMethod_Id = new SelectList(getAllMethods.Result.ToList(), "Id", "Method");
                else
                    ViewBag.ComplaintMethod_Id = new SelectList(getAllMethods.Result.ToList(), "Id", "Method", Convert.ToInt32(ComplaintMethod_Id));
                //Also pass the total count for building the pagination links
                ViewBag.TotalCount = paginationResult.TotalCount;
                ViewBag.PageSize = pageSize;
                ViewBag.PageNumber = pageNumber;
                ViewBag.SearchString = searchString;
                ViewBag.Priority = priority;

                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IActionResult> ComplaintForwardDetails(int id)
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
            return PartialView("_DepartmentForwardPartial", complaintData);
        }




        [HttpPost]
        public JsonResult ForwardToCentral(int Id, string remark)
        {
            try
            {
                _complainProcess.UpdateForwardToCentral(Id, remark);
                TempData["ToastMessage"] = "SentToCentralSuccess!";
                return Json(new { success = true, message = "Sent to central successfully." });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        public async Task<IActionResult> ComplaintResolveDetails(int id)
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
            return PartialView("_DepartmentResolvePartial", complaintData);
        }



        [HttpPost]
        public JsonResult DepComplainResolve(int Id, string Remark)
        {
            try
            {
                _complainProcess.ComplainResolve(Id, Remark);
                TempData["ToastMessage"] = "resolvedSuccessfully!";
                return Json(new { success = true, message = "Complain Resolve successfully." });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        //------------------------ Central Process ------------------------------------>    

        public async Task<IActionResult> CentralProcess(int pageNumber = 1, int pageSize = 10, string searchString = null, string ComplaintMethod_Id = null, string priority = null)
        {
            try
            {
                PaginationResultsModel<ComplaintMaster> paginationResult = await _complainProcess.getCentralComplaintList(pageNumber, pageSize, searchString, ComplaintMethod_Id, priority);
                ViewBag.ComplainLists = paginationResult.Items.OrderByDescending(a => a.Status == "Sent Central").ToList();
                var getAllMethods = _complainProcess.getMethodList();
                if (ComplaintMethod_Id == null)
                    ViewBag.ComplaintMethod_Id = new SelectList(getAllMethods.Result.ToList(), "Id", "Method");
                else
                    ViewBag.ComplaintMethod_Id = new SelectList(getAllMethods.Result.ToList(), "Id", "Method", Convert.ToInt32(ComplaintMethod_Id));
                //Also pass the total count for building the pagination links
                ViewBag.TotalCount = paginationResult.TotalCount;
                ViewBag.PageSize = pageSize;
                ViewBag.PageNumber = pageNumber;
                ViewBag.SearchString = searchString;
                ViewBag.Priority = priority;

                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> CentralComplaintForwardDetails(int id)
        {
            Complaint_ManageProcessModel complaint = _complainProcess.getComplainProcessUsingId(id);
            var getAllDeps = _complainProcess.getDepList();

            ComplaintMaster complaintData = await _complainProcess.getComplainUsingId(id);
            if (complaintData == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(complaintData.AttachmentPath))
            {
                complaintData.AttachmentPath = Path.GetFileName(complaintData.AttachmentPath);
            }

            ViewBag.Dep_Id = new SelectList(getAllDeps.Result.ToList(), "Id", "Name", complaint.Dep_Id);
            return PartialView("_CentralForwardPartial", complaintData);
        }



        [HttpPost]
        public JsonResult ForwardToDepartment(int Id, int Department, string Remark)
        {
            try
            {
                _complainProcess.UpdateForwardToDepartment(Id, Department, Remark);
                TempData["ToastMessage"] = "sentDepSuccessfully!";
                return Json(new { success = true, message = "Sent to Department successfully." });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        public JsonResult CentralComplainResolve(int Id, string Remark)
        {
            try
            {
                _complainProcess.ComplainResolve(Id, Remark);
                TempData["ToastMessage"] = "resolvedSuccessfully!";
                return Json(new { success = true, message = "Complain Resolve successfully." });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> CentralComplaintResolveDetails(int id)
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
            return PartialView("_CentralResolvePartial", complaintData);
        }




        // -------Dashboard----------------------------------------------------------->

        //public async Task<IActionResult> DashboardComplaintCount()
        //{
        //    // Get complaint counts
        //    var complaintCounts = await _complainProcess.GetDashboardComplaintCounts();
        //    ViewBag.TotalComplaintCount = complaintCounts.TotalCount;
        //    ViewBag.PendingComplaintCount = complaintCounts.PendingCount;
        //    ViewBag.ResolveComplaintCount = complaintCounts.ResolveCount;

        //    return View();
        //}

        public async Task<IActionResult> DashboardComplaintCount()
        {
            try
            {
                var complaintCounts = await _complainProcess.GetDashboardComplaintCounts();

                return Json(new
                {
                    total = complaintCounts.TotalCount,
                    pending = complaintCounts.PendingCount,
                    resolved = complaintCounts.ResolveCount,
                    fromCreatedDate = complaintCounts.FromCreatedDate,
                    toCreatedDate = complaintCounts.ToCreatedDate,
                    methods = complaintCounts.ComplaintMethodCounts,
                    department = complaintCounts.ComplaintDepartmentCounts
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        //------------------------ Complaint History ------------------------------------>    

        public async Task<IActionResult> ComplaintHistoryProcess(int pageNumber = 1, int pageSize = 10, string searchString = null)
        {
            try
            {
                var getAllDeps = _complainProcess.getComplainNumberList();
                ViewBag.Complaint = new SelectList(getAllDeps.Result.ToList(), "Id", "Refference");
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public async Task<IActionResult> GetComplaintHistoryDetails(int ComplainNo)
        //{
        //    //List<Complaint_ManageProcessModel> complaintData = await _complainProcess.GetComplaintHistoryDetails(id);
        //    //if (complaintData == null)
        //    //{
        //    //    return NotFound(); 
        //    //}                      
        //    //return PartialView("_DepartmentResolvePartial", complaintData);

        //    var getAllDeps = _complainProcess.GetComplaintHistoryDetails(ComplainNo);
        //    ViewBag.ABC = new SelectList(getAllDeps.Result.ToList(), "ForwordUser", "Dep");
        //    return View();
        //}

        public async Task<IActionResult> GetComplaintHistoryDetails(int id)
        {
            var getAllDeps = await _complainProcess.GetComplaintHistoryDetails(id);

            var result = getAllDeps.Select(x => new
            {
                ForwordUser = x.ForwordUser,
                Dep = x.Dep,
                CreatedDate = x.CreatedDate,
                MatrixOrder = x.MatrixOrder,
                IsResolved = x.IsResolved,
                ResolvedDateTime = x.ResolvedDateTime,
                ResolvedRemark = x.ResolvedRemark,
                ResolvedUser = x.ResolvedUser,
                DepartmentName = x.DepartmentName,
                IsSentDep = x.IsSentDep,
                IsSentCentral = x.IsSentCentral,
                Remark = x.Remark
            }).ToList();

            return Json(result);
        }




    }
}
