using LinkedHU_CENG.Models;
using LinkedHU_CENG.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LinkedHU_CENG.Controllers
{
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CommentController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return PartialView(_db.Comments.ToList());
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                ViewData["isBanUserValid"] = 1;
                BannedUser banUser = _db.BannedUsers.Find(HttpContext.Session.GetInt32("UserID"));
                if (banUser != null)
                {
                    ViewData["isBanUserValid"] = 0;
                }
                return PartialView();
            }
            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public IActionResult Create(PostCommentViewModel viewModel)
        {
            Comment comment = viewModel.comment;
            int postID = viewModel.postId;

            if (comment.Content != null)
            {
                var userId = HttpContext.Session.GetInt32("UserID");
                comment.UserId = userId;
                var user = _db.Users.Find(userId);
                comment.UserName = user.Name + " " + user.Surname;
                comment.UserProfilePicture = user.ProfilePicturePath;
                comment.PostId = postID;

                _db.Comments.Add(comment);
                _db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Some Error Occured!");
            }
            return View();
        }


        public ActionResult Delete(int? id)
        {
            var comment = _db.Comments.Find(id);
            if (comment == null)
            {
                return NotFound();
            }
            _db.Comments.Remove(comment);
            _db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}
