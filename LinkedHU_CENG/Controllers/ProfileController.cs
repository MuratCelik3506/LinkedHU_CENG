using LinkedHU_CENG.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace LinkedHU_CENG.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProfileController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            this.db = context;
            this.webHostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {


            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                var user = db.Users.FirstOrDefault(
                    u => u.UserId.Equals(HttpContext.Session.GetInt32("UserID")) && u.Email.Equals(HttpContext.Session.GetString("Email")));
                ViewData["User"] = user;
                ViewData["changePassword"] = TempData["changePassword"];
                ViewData["stateMerge"] = TempData["stateMerge"];

                ViewData["stateDeleteAccount"] = TempData["stateDeleteAccount"];

                ViewData["isBanUserValid"] = 1;
                BannedUser banUser = db.BannedUsers.Find(HttpContext.Session.GetInt32("UserID"));

                if (banUser != null)
                {
                    ViewData["isBanUserValid"] = 0;
                }
                return View();
            }

            return RedirectToAction("Index", "Home");

        }

        public ActionResult Edit()
        {
            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                var user = db.Users.FirstOrDefault(u => u.UserId.Equals(HttpContext.Session.GetInt32("UserID")) && u.Email.Equals(HttpContext.Session.GetString("Email")));

                if (user == null)
                {
                    return NotFound();
                }

                ViewData["Date"] = DateTime.Now.AddYears(-18).ToString("yyyy-MM-dd");

                return View(user);

            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Edit(User user)
        {
            if (HttpContext.Session.GetInt32("UserID") != null)
            {

                if (ModelState.IsValid)
                {
                    var oldUser = db.Users.AsNoTracking().Where(x => x.UserId.Equals(user.UserId)).ToList()[0];
                    int sameMail = 0;
                    if (user.SecondEmail != null)
                    {
                        sameMail = db.Users.AsNoTracking().Where(x => (x.Email == user.SecondEmail) || (x.SecondEmail == user.SecondEmail)).ToList().Count;
                    }


                    string uniqueFileName = UploadedFile(user);
                    if (uniqueFileName != null)
                    {
                        user.ProfilePicturePath = uniqueFileName;
                    }

                    var posts = db.Posts.Where(x => x.UserId == user.UserId).ToList();
                    foreach (Post post in posts)
                    {
                        post.UserProfilePicture = user.ProfilePicturePath;
                    }

                    var announcements = db.Announcements.Where(x => x.UserId == user.UserId).ToList();
                    foreach (Announcement announcement in announcements)
                    {
                        announcement.UserProfilePicture = user.ProfilePicturePath;
                    }

                    if (user.SecondEmail != null)
                    {
                        var oneUserMoreRequest = db.MergeEmailRequests.AsNoTracking().Where(x => x.UserId == user.UserId).ToList();
                        if (oneUserMoreRequest.Count > 0)
                        {
                            TempData["stateMerge"] = -2;
                        }
                        else if (sameMail == 0)
                        {

                            MergeEmailRequest newRequest = new MergeEmailRequest();
                            newRequest.UserId = user.UserId;
                            newRequest.Email = user.Email;
                            newRequest.Name = user.Name;
                            newRequest.Surname = user.Surname;
                            newRequest.SecondEmail = user.SecondEmail;
                            db.MergeEmailRequests.Add(newRequest);
                            TempData["stateMerge"] = 1;
                        }
                        else
                        {
                            TempData["stateMerge"] = -3;
                        }
                    }
                    else if((oldUser.SecondEmail != null) && user.SecondEmail == null)
                    {
                        TempData["stateMerge"] = -1;
                    }
                    user.SecondEmail = oldUser.SecondEmail;

                    db.Users.Update(user);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Profile");
                }
                else
                {
                    ModelState.AddModelError("", "Some error occured!");
                }


                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult ChangePassword()
        {
            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                var user = db.Users.FirstOrDefault(u => u.UserId.Equals(HttpContext.Session.GetInt32("UserID")) && u.Email.Equals(HttpContext.Session.GetString("Email")));

                if (user == null)
                {
                    return NotFound();
                }
                string oldPassword = "";
                string newPassword = "";

                ViewData["User"] = user;

                return View();
            }

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public ActionResult ChangePasswordRequest()
        {
            string oldPassword = HttpContext.Request.Form["oldPassword"];
            string newPassword = HttpContext.Request.Form["newPassword"];
            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                var user = db.Users.FirstOrDefault(u => u.UserId.Equals(HttpContext.Session.GetInt32("UserID")) && u.Email.Equals(HttpContext.Session.GetString("Email")));
                if (ModelState.IsValid)
                {
                    if (Encrypt(oldPassword) == user.Password)
                    {
                        user.Password = Encrypt(newPassword);
                        db.SaveChanges();
                        TempData["changePassword"] = 1;
                        return RedirectToAction("Logout", "User");
                    }
                    else
                    {
                        TempData["changePassword"] = -1;
                        return RedirectToAction("Index", "Profile");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Some error occured!");
                }
                return View();
            }
            return RedirectToAction("Index", "Home");
        }


        private string UploadedFile(User user)
        {
            string uniqueFileName = null;

            if (user.ProfilePicture != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "profilePictures");

                if (user.ProfilePicturePath != null && System.IO.File.Exists(Path.Combine(uploadsFolder, user.ProfilePicturePath)))
                {
                    System.IO.File.Delete(Path.Combine(uploadsFolder, user.ProfilePicturePath));
                }

                uniqueFileName = Guid.NewGuid().ToString() + "_" + user.ProfilePicture.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    user.ProfilePicture.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        private string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
    }
}
