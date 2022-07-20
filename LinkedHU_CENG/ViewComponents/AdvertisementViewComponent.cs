using Microsoft.AspNetCore.Mvc;
using LinkedHU_CENG.Models;
using Microsoft.EntityFrameworkCore;
using LinkedHU_CENG.Models.ViewModels;

namespace LinkedHU_CENG.ViewComponents
{
    [ViewComponent(Name = "AdvertisementViewComponent")]
    public class AdvertisementViewComponent : ViewComponent
    {

        private readonly ApplicationDbContext db;

        public AdvertisementViewComponent(ApplicationDbContext db)
        {
            this.db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {

            IEnumerable<Advertisement> mc = await db.Advertisements.ToListAsync();
            List<AdvertisementViewModel> viewModels = new List<AdvertisementViewModel>();
            foreach (Advertisement advertisement in mc)
            {
                AdvertisementViewModel advertisementViewModel = new AdvertisementViewModel();
                List<Application> applications = db.Applications.Where(a => a.AdvertisementId == advertisement.AdvertisementId).ToList();
                advertisementViewModel.Applications = applications;
                advertisementViewModel.Advertisement = advertisement;
                advertisementViewModel.AdvertisementId = advertisement.AdvertisementId;
                viewModels.Add(advertisementViewModel);
            }


            ViewData["SessionUserId"] = HttpContext.Session.GetInt32("UserID");
            return View(viewModels);
        }
    }
}
