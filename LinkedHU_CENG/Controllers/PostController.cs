using LinkedHU_CENG.Models;
using Microsoft.AspNetCore.Mvc;

namespace LinkedHU_CENG.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment webHostEnvironment;

        public PostController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            this.webHostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return PartialView(_db.Posts.ToList());
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
                return View();

            }
            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public IActionResult Create(Post post)
        {
            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                if (ModelState.IsValid)
                {
                    var userId = HttpContext.Session.GetInt32("UserID");
                    post.UserId = userId;
                    var user = _db.Users.Find(userId);
                    post.UserName = user.Name + " " + user.Surname;
                    post.UserProfilePicture = user.ProfilePicturePath;

                    if (post.PostUpload != null)
                    {
                        string uniqueFileName = UploadedFile(post);
                        string[] name = uniqueFileName.Split(".");

                        if (name[1] == "mp4")
                        {
                            post.PostVideoPath = uniqueFileName;
                        }
                        else if (name[1] == "pdf")
                        {
                            post.PostPdfPath = uniqueFileName;
                        }
                        else
                        {
                            post.PostImagePath = uniqueFileName;
                        }
                    }

                    _db.Posts.Add(post);
                    _db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Some Error Occured!");

                }
                ViewData["isBanUserValid"] = 1;
                BannedUser banUser = _db.BannedUsers.Find(HttpContext.Session.GetInt32("UserID"));
                if (banUser != null)
                {
                    ViewData["isBanUserValid"] = 0;
                }
                return View(post);
            }
            return RedirectToAction("Index", "Home");

        }

        // GET
        public ActionResult Update(int? id)
        {
            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }
                var post = _db.Posts.Find(id);

                if (post == null)
                {
                    return NotFound();
                }

                ViewData["isBanUserValid"] = 1;
                BannedUser banUser = _db.BannedUsers.Find(HttpContext.Session.GetInt32("UserID"));
                if (banUser != null)
                {
                    ViewData["isBanUserValid"] = 0;
                }

                return View(post);
            }
            return View("Index", "Home");
        }


        // POST
        [HttpPost]
        public ActionResult Update(Post post)
        {
            if (HttpContext.Session.GetInt32("UserID") != null)
            {

                if (ModelState.IsValid)
                {
                    string uniqueFileName = UploadedFile(post);
                    if (uniqueFileName != null)
                    {
                        string[] name = uniqueFileName.Split(".");

                        if (name[1] == "mp4")
                        {
                            post.PostVideoPath = uniqueFileName;
                        }
                        else if (name[1] == "pdf")
                        {
                            post.PostPdfPath = uniqueFileName;
                        }
                        else
                        {
                            post.PostImagePath = uniqueFileName;
                        }
                    }

                    _db.Posts.Update(post);
                    _db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Some error occured!");
                }
                return View(post);
            }
            return View("Index", "Home");
        }

        public ActionResult Delete(int? id)
        {
            var post = _db.Posts.Find(id);
            if (post == null)
            {
                return NotFound();
            }

            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "postUploads");

            if (post.PostImagePath != null && System.IO.File.Exists(Path.Combine(uploadsFolder, post.PostImagePath)))
            {
                System.IO.File.Delete(Path.Combine(uploadsFolder, post.PostImagePath));
            }

            if (post.PostVideoPath != null && System.IO.File.Exists(Path.Combine(uploadsFolder, post.PostVideoPath)))
            {
                System.IO.File.Delete(Path.Combine(uploadsFolder, post.PostVideoPath));
            }

            if (post.PostPdfPath != null && System.IO.File.Exists(Path.Combine(uploadsFolder, post.PostPdfPath)))
            {
                System.IO.File.Delete(Path.Combine(uploadsFolder, post.PostPdfPath));
            }

            _db.Posts.Remove(post);
            _db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        private string UploadedFile(Post post)
        {
            string uniqueFileName = null;

            if (post.PostUpload != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "postUploads");


                if (post.PostImagePath != null && System.IO.File.Exists(Path.Combine(uploadsFolder, post.PostImagePath)))
                {
                    System.IO.File.Delete(Path.Combine(uploadsFolder, post.PostImagePath));
                    post.PostImagePath = null;
                }

                if (post.PostVideoPath != null && System.IO.File.Exists(Path.Combine(uploadsFolder, post.PostVideoPath)))
                {
                    System.IO.File.Delete(Path.Combine(uploadsFolder, post.PostVideoPath));
                    post.PostVideoPath = null;
                }

                if (post.PostPdfPath != null && System.IO.File.Exists(Path.Combine(uploadsFolder, post.PostPdfPath)))
                {
                    System.IO.File.Delete(Path.Combine(uploadsFolder, post.PostPdfPath));
                    post.PostPdfPath = null;
                }

                uniqueFileName = Guid.NewGuid().ToString() + "_" + post.PostUpload.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    post.PostUpload.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        public IActionResult ViewPost(int id)
        {
            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                if (ModelState.IsValid)
                {
                    ViewData["isBanUserValid"] = 1;
                    BannedUser banUser = _db.BannedUsers.Find(HttpContext.Session.GetInt32("UserID"));
                    if (banUser != null)
                    {
                        ViewData["isBanUserValid"] = 0;
                    }
                    var post = _db.Posts.Find(id);
                    ViewData["post"] = post;
                    ViewData["SessionUserId"] = HttpContext.Session.GetInt32("UserID");
                    return View("ViewPost");
                }
                else
                {
                    ModelState.AddModelError("", "Some error occured!");
                }
                return View();
            }
            return View("Index", "Home");
        }

    }

}