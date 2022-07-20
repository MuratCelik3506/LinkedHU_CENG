using LinkedHU_CENG.Models;
using Microsoft.AspNetCore.Mvc;

namespace LinkedHU_CENG.Controllers
{
    public class ResourceController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ResourceController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            this.webHostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return PartialView(_db.Resources.ToList());
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                return View();
            }
            return RedirectToAction("Index", "Profile");

        }

        [HttpPost]
        public IActionResult Create(Resource resource)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetInt32("UserID");
                resource.UserId = userId;

                if (resource.UploadedFile != null)
                {
                    string uniqueFileName = UploadedFile(resource);
                    string[] name = uniqueFileName.Split(".");

                    if (name[1] == "mp4")
                    {
                        resource.ResourceVideoName = uniqueFileName;
                    }
                    else if (name[1] == "pdf")
                    {
                        resource.ResourcePdfName = uniqueFileName;
                    }
                    else if (name[1] == "xls" & name[1] == "xlsx")
                    {
                        resource.ResourceExelName = uniqueFileName;
                    }
                    else if (name[1] == "doc" & name[1] == "docx")
                    {
                        resource.ResourceWordName = uniqueFileName;
                    }
                    else if (name[1] == "ppt" & name[1] == "pptx")
                    {
                        resource.ResourcePointName = uniqueFileName;
                    }
                    else
                    {
                        resource.ResourceImageName = uniqueFileName;
                    }
                }

                _db.Resources.Add(resource);
                _db.SaveChanges();
                return RedirectToAction("Index", "Profile");
            }
            else
            {
                ModelState.AddModelError("", "Some Error Occured!");
            }
            return View(resource);

        }

        // GET
        public ActionResult Update(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var resource = _db.Resources.Find(id);

            if (resource == null)
            {
                return NotFound();
            }

            ViewData["isBanUserValid"] = 1;
            BannedUser banUser = _db.BannedUsers.Find(HttpContext.Session.GetInt32("UserID"));
            if (banUser != null)
            {
                ViewData["isBanUserValid"] = 0;
            }

            return View(resource);
        }

        // POST
        [HttpPost]
        public ActionResult Update(Resource resource)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFile(resource);
                if (uniqueFileName != null)
                {
                    string[] name = uniqueFileName.Split(".");

                    if (name[1] == "mp4")
                    {
                        resource.ResourceVideoName = uniqueFileName;
                    }
                    else if (name[1] == "pdf")
                    {
                        resource.ResourcePdfName = uniqueFileName;
                    }
                    else if (name[1] == "xls" & name[1] == "xlsx")
                    {
                        resource.ResourceExelName = uniqueFileName;
                    }
                    else if (name[1] == "doc" & name[1] == "docx")
                    {
                        resource.ResourceWordName = uniqueFileName;
                    }
                    else if (name[1] == "ppt" & name[1] == "pptx")
                    {
                        resource.ResourcePointName = uniqueFileName;
                    }
                    else
                    {
                        resource.ResourceImageName = uniqueFileName;
                    }
                }

                _db.Resources.Update(resource);
                _db.SaveChanges();
                return RedirectToAction("Index", "Profile");
            }
            else
            {
                ModelState.AddModelError("", "Some error occured!");
            }
            return View(resource);
        }

        public ActionResult Delete(int? id)
        {
            var resource = _db.Resources.Find(id);
            if (resource == null)
            {
                return NotFound();
            }

            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "resourceUploads");

            if (resource.ResourceImageName != null && System.IO.File.Exists(Path.Combine(uploadsFolder, resource.ResourceImageName)))
            {
                System.IO.File.Delete(Path.Combine(uploadsFolder, resource.ResourceImageName));
            }

            if (resource.ResourceVideoName != null && System.IO.File.Exists(Path.Combine(uploadsFolder, resource.ResourceVideoName)))
            {
                System.IO.File.Delete(Path.Combine(uploadsFolder, resource.ResourceVideoName));
            }

            if (resource.ResourcePdfName != null && System.IO.File.Exists(Path.Combine(uploadsFolder, resource.ResourcePdfName)))
            {
                System.IO.File.Delete(Path.Combine(uploadsFolder, resource.ResourcePdfName));
            }

            if (resource.ResourceWordName != null && System.IO.File.Exists(Path.Combine(uploadsFolder, resource.ResourceWordName)))
            {
                System.IO.File.Delete(Path.Combine(uploadsFolder, resource.ResourceWordName));
            }

            if (resource.ResourceExelName != null && System.IO.File.Exists(Path.Combine(uploadsFolder, resource.ResourceExelName)))
            {
                System.IO.File.Delete(Path.Combine(uploadsFolder, resource.ResourceExelName));
            }


            _db.Resources.Remove(resource);
            _db.SaveChanges();

            return RedirectToAction("Index", "Profile");
        }

        private string UploadedFile(Resource resource)
        {
            string uniqueFileName = null;

            if (resource.UploadedFile != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "resourceUploads");


                if (resource.ResourceImageName != null && System.IO.File.Exists(Path.Combine(uploadsFolder, resource.ResourceImageName)))
                {
                    System.IO.File.Delete(Path.Combine(uploadsFolder, resource.ResourceImageName));
                    resource.ResourceImageName = null;
                }

                if (resource.ResourceVideoName != null && System.IO.File.Exists(Path.Combine(uploadsFolder, resource.ResourceVideoName)))
                {
                    System.IO.File.Delete(Path.Combine(uploadsFolder, resource.ResourceVideoName));
                    resource.ResourceVideoName = null;
                }

                if (resource.ResourcePdfName != null && System.IO.File.Exists(Path.Combine(uploadsFolder, resource.ResourcePdfName)))
                {
                    System.IO.File.Delete(Path.Combine(uploadsFolder, resource.ResourcePdfName));
                    resource.ResourcePdfName = null;
                }

                if (resource.ResourceWordName != null && System.IO.File.Exists(Path.Combine(uploadsFolder, resource.ResourceWordName)))
                {
                    System.IO.File.Delete(Path.Combine(uploadsFolder, resource.ResourceWordName));
                    resource.ResourceWordName = null;
                }

                if (resource.ResourceExelName != null && System.IO.File.Exists(Path.Combine(uploadsFolder, resource.ResourceExelName)))
                {
                    System.IO.File.Delete(Path.Combine(uploadsFolder, resource.ResourceExelName));
                    resource.ResourceExelName = null;
                }

                uniqueFileName = Guid.NewGuid().ToString() + "_" + resource.UploadedFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    resource.UploadedFile.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
