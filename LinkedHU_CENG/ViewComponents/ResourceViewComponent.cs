using Microsoft.AspNetCore.Mvc;
using LinkedHU_CENG.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkedHU_CENG.ViewComponents
{
    [ViewComponent(Name = "ResourceViewComponent")] //Solution
    public class ResourceViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public ResourceViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<Resource> resources = await _db.Resources.ToListAsync();
            ViewData["SessionUserId"] = HttpContext.Session.GetInt32("UserID");

            return View(resources);
        }
    }
}
