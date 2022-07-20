using LinkedHU_CENG.Models;
using Microsoft.AspNetCore.Mvc;

namespace LinkedHU_CENG.Controllers
{
    public class ReportController : Controller
    {

        private readonly ApplicationDbContext db;

        public ReportController(ApplicationDbContext context)
        {
            this.db = context;
        }
        public IActionResult Index()
        {

            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                return View();
            }

            return RedirectToAction("Index", "Home");
        }
    
        
        [HttpPost]
        public IActionResult ReportRequest(Report report)
        {
            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                if (ModelState.IsValid)
                {
                    report.UserId = (int)HttpContext.Session.GetInt32("UserID");
                    db.Reports.Add(report);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Report");
                }
                else
                {
                    ModelState.AddModelError("", "Some Error Occured!");
                }
                return View();
            }
                
            else
            {
                ModelState.AddModelError("", "Some Error Occured!");
            }
            return View();

        }


    }


}
