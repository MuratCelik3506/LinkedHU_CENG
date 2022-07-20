using LinkedHU_CENG.Models;
using Microsoft.AspNetCore.Mvc;
namespace LinkedHU_CENG.Controllers

{
    public class AdvertisementController : Controller
    {
        private readonly ApplicationDbContext db;
        public AdvertisementController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["StateAdvertisement"] = TempData["StateAdvertisement"];
            return View();
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserID") != null)
            {
                ViewData["isBanUserValid"] = 1;
                BannedUser banUser = db.BannedUsers.Find(HttpContext.Session.GetInt32("UserID"));
                if (banUser != null)
                {
                    ViewData["isBanUserValid"] = 0;
                    TempData["StateAdvertisement"] = 3;
                    return RedirectToAction("Index", "Advertisement");
                }
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult Create(Advertisement advertisement)
        {
            if (HttpContext.Session.GetString("UserID") != null)
            {
                if (ModelState.IsValid)
                {
                    var userId = HttpContext.Session.GetInt32("UserID");
                    advertisement.UserId = userId;
                    var user = db.Users.Find(userId);
                    advertisement.UserName = user.Name + " " + user.Surname;
                    advertisement.IsActive = true;

                    db.Advertisements.Add(advertisement);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Advertisement");
                }
                else
                {
                    ModelState.AddModelError("", "Some Error Occured!");
                }
                return View(advertisement);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Update(int? id)
        {
            if (HttpContext.Session.GetString("UserID") != null)
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }
                var advertisement = db.Advertisements.Find(id);

                if (advertisement == null)
                {
                    return NotFound();
                }

                return View(advertisement);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpPost]
        public ActionResult Update(Advertisement advertisement)
        {
            if (HttpContext.Session.GetString("UserID") != null)
            {
                if (ModelState.IsValid)
                {
                    db.Advertisements.Update(advertisement);
                    db.SaveChanges();
                    return RedirectToAction("Update", "Advertisement");
                }
                else
                {
                    ModelState.AddModelError("", "Some error occured!");
                }
                return View(advertisement);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        public ActionResult Delete(int? id)
        {
            if (HttpContext.Session.GetString("UserID") != null)
            {
                var advertisement = db.Advertisements.Find(id);
                if (advertisement == null)
                {
                    return NotFound();
                }
                db.Advertisements.Remove(advertisement);
                db.SaveChanges();

                return RedirectToAction("Index", "Advertisement");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult GetApplicantsDetails(Advertisement advertisement)
        {

            if (HttpContext.Session.GetString("UserID") != null)
            {
                List<Application> applications = db.Applications.Where(a => a.AdvertisementId == advertisement.AdvertisementId).ToList();
                ViewData["Application"] = applications;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Close(int id)
        {
            if (HttpContext.Session.GetString("UserID") != null)
            {
                var advertisement = db.Advertisements.Find(id);
                if (advertisement == null)
                {
                    return NotFound();
                }
                advertisement.IsActive = false;
                db.SaveChanges();

                return RedirectToAction("Index", "Advertisement");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
