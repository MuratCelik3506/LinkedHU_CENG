using LinkedHU_CENG.Models;
using Microsoft.AspNetCore.Mvc;

namespace LinkedHU_CENG.Controllers
{
    public class FollowController : Controller
    {

        private readonly ApplicationDbContext db;

        public FollowController(ApplicationDbContext context)
        {
            this.db = context;
        }



        public ActionResult Following(int id)
        {
            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                Follow newFollow = new Follow();
                newFollow.FollowerId = (int)HttpContext.Session.GetInt32("UserID");
                System.Diagnostics.Debug.WriteLine(newFollow.FollowerId);
                newFollow.FollowingId = id;
                db.Add(newFollow);
                db.SaveChanges();
                TempData["IsFollow"] = 1;
                TempData["FollowingId"] = id;
                return RedirectToAction("ViewProfileToFollow", "User");
            }

            return RedirectToAction("Index", "Home");
        }


        public ActionResult UnFollowing(int id)
        {
            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                Follow deleteFollow = db.Follows.Where(x => (x.FollowingId == id) && (x.FollowerId == HttpContext.Session.GetInt32("UserID"))).FirstOrDefault();
                db.Follows.Remove(deleteFollow);
                db.SaveChanges();
                TempData["IsFollow"] = 0;
                TempData["FollowingId"] = id;
                return RedirectToAction("ViewProfileToFollow", "User");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
