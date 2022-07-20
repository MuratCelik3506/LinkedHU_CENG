using Microsoft.AspNetCore.Mvc;
using LinkedHU_CENG.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkedHU_CENG.ViewComponents
{
    [ViewComponent(Name = "AnnouncementViewComponent")] //Solution
    public class AnnouncementViewComponent: ViewComponent
    {

        private readonly ApplicationDbContext _db;

        public AnnouncementViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {

            IEnumerable<Announcement> mc = await _db.Announcements.ToListAsync();
            ViewData["SessionUserId"] = HttpContext.Session.GetInt32("UserID");
            return View(mc);
        }
    }
}
