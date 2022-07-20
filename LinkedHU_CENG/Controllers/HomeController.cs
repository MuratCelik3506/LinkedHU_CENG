using LinkedHU_CENG.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LinkedHU_CENG.Controllers
{
    public class HomeController : Controller  
    {  
        private readonly ApplicationDbContext _db;
        private readonly ILogger<HomeController> _logger;
         
        public HomeController(ApplicationDbContext db, ILogger<HomeController> logger)  
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index()

        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                ViewData["ForgetPassword"] = TempData["ForgetPassword"];
                ViewData["changePasswordToHome"] = TempData["changePasswordToHome"];
                return View("WelcomePage");
            }
            else
            {
                ViewData["isBanUserValid"] = 1;
                BannedUser banUser = _db.BannedUsers.Find(HttpContext.Session.GetInt32("UserID"));
                if (banUser != null)
                {
                    ViewData["isBanUserValid"] = 0;
                }
                else
                {
                    User user = _db.Users.Find(HttpContext.Session.GetInt32("UserID"));
                    ViewData["UserRole"] = user.Role;
                }
                return View("LoggedIn"); ;
            }
        }

        public IActionResult Privacy()
        {
            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                return View("PrivacyLoggedIn");
            }
            else
            {
                return View();
            }
           
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] 
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}