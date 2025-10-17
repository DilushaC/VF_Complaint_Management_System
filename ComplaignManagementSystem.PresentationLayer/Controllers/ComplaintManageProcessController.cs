using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComplaignManagementSystem.Presentation.Controllers
{
    public class ComplaintManageProcessController : Controller
    {
        // GET: ComplaintManageProcessController
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        // GET: ComplaintManageProcessController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ComplaintManageProcessController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ComplaintManageProcessController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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
