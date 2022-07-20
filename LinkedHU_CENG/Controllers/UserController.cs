using LinkedHU_CENG.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace LinkedHU_CENG.Controllers
{

    public class UserController : Controller
    {
        private readonly ApplicationDbContext db;

        public UserController(ApplicationDbContext context)
        {
            this.db = context;
        }

        public IActionResult Index()
        {
            return View(db.Users.ToList());
        }



        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                ViewData["RegisterState"] = TempData["RegisterState"];
                ViewData["LoginState"] = TempData["LoginState"];
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        
        public IActionResult Login(User usr)
        {
            try
            {
                var info = db.Users.FirstOrDefault(u => (u.Email.Equals(usr.Email) || u.SecondEmail.Equals(usr.Email)) && u.Password.Equals(Encrypt(usr.Password)));
                if (info != null)
                {
                    HttpContext.Session.SetInt32("UserID", info.UserId);
                    HttpContext.Session.SetString("Email", info.Email);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["LoginState"] = 2;
                    return RedirectToAction("Login", "User");
                }
            }
            catch
            {
                TempData["LoginState"] = 3;
            }
            return RedirectToAction("Login", "User");
        }


        public IActionResult Logout()
        {
            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                //HttpContext.Session.Clear();
                HttpContext.Session.Remove("UserID");
                HttpContext.Session.Remove("Email");
                TempData["changePasswordToHome"] = TempData["changePassword"];

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                var oneUserMoreRequest = db.DeleteRequests.AsNoTracking().Where(x => x.UserId == id).ToList();
                if (oneUserMoreRequest.Count > 0)
                {
                    TempData["stateDeleteAccount"] = -1;
                }
                else
                {
                    var deleteUser = db.Users.Find(id);
                    DeleteRequest request = new DeleteRequest();
                    request.UserId = id;
                    request.Name = deleteUser.Name;
                    request.Email = deleteUser.Email;
                    request.Surname = deleteUser.Surname;
                    request.Role = deleteUser.Role;
                    db.DeleteRequests.Add(request);
                    db.SaveChanges();
                    TempData["stateDeleteAccount"] = 1;

                }


                return RedirectToAction("Index", "Profile");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult ForgetPassword()
        {
            return View("ForgetPassword");
        }

        [HttpPost]
        public IActionResult ForgetPassword(ForgetPassword forgetUser)
        {
            if (ModelState.IsValid)
            {
                try { 
                User user = db.Users.Where(m => (m.Email.Equals(forgetUser.Email) || m.SecondEmail.Equals(forgetUser.Email))).FirstOrDefault();
                if (user != null)
                {
                    forgetUser.Name = user.Name;
                    forgetUser.Surname = user.Surname;
                    db.ForgetPasswords.Add(forgetUser);
                    db.SaveChanges();
                    TempData["ForgetPassword"] = 10;
                }
                else
                {
                    TempData["ForgetPassword"] = 15;
                }
                return RedirectToAction("Index", "Home");
                }
                catch
                {
                    TempData["ForgetPassword"] = -10;
                    ModelState.AddModelError("", "Some Error Occured!");
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["ForgetPassword"] = -10;
                ModelState.AddModelError("", "Some Error Occured!");
                return RedirectToAction("Index", "Home");
            }
        }



        public IActionResult Search()
        {
            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                string word = HttpContext.Request.Form["word"];

                System.Diagnostics.Debug.WriteLine(word);
                char[] charsToTrim = { '*', ' ', '\'' };
                string[] wordSearch = word.Trim(charsToTrim).Split(" ");

                string nameSurnameCommand = $"SELECT * FROM \"Users\" WHERE \"Name\" ILIKE '%{word}%'";
                var nameSurnameList = db.Users.FromSqlRaw(nameSurnameCommand).ToList();

                string nameCommand = $"SELECT * FROM \"Users\" WHERE \"Name\" ILIKE '%{wordSearch[0]}%'";
                var nameList = db.Users.FromSqlRaw(nameCommand).ToList();
                var surnameList = new List<User>();

                if (wordSearch.Length > 1)
                {
                    string surnameCommand = $"SELECT * FROM \"Users\" WHERE \"Surname\" ILIKE '%{wordSearch[1]}%'";
                    surnameList = db.Users.FromSqlRaw(surnameCommand).ToList();
                }

                foreach (var usrNameSurname in nameSurnameList)
                {
                    nameList.Remove(usrNameSurname);
                    surnameList.Remove(usrNameSurname);
                }
                foreach (var usrNameSurname in nameList)
                {
                    surnameList.Remove(usrNameSurname);
                }

                ViewData["NameSurname"] = nameSurnameList;
                ViewData["Name"] = nameList;
                ViewData["Surname"] = surnameList;
                if((nameSurnameList.Count > 0) || (nameList.Count > 0) && (surnameList.Count > 0))
                {
                    ViewData["atLeastOneSearch"] = 1;
                }
                else
                {
                    ViewData["atLeastOneSearch"] = -1;
                }
                return View();
            }
            return RedirectToAction("Index", "Home");
        }


        public IActionResult ViewProfile(int id)
        {
            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                var user = db.Users.Find(id);
                List<Post> posts = db.Posts.OrderByDescending(x => x.CreatedAt).Where(x => x.UserId == id).ToList();
                List<Resource> resources = db.Resources.OrderByDescending(x => x.CreatedAt).Where(x => x.UserId == id).ToList();
                ViewData["SessionUserId"] = HttpContext.Session.GetInt32("UserID");
                ViewData["User"] = user;
                ViewData["Posts"] = posts;
                ViewData["Resources"] = resources;
                var followState = db.Follows.Where(x => (x.FollowerId == HttpContext.Session.GetInt32("UserID")) && (x.FollowingId == id)).ToList();
                if (followState.Count > 0) { ViewData["IsFollow"] = 1; }
                else { ViewData["IsFollow"] = 0; }
                return View("Profile");
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ViewProfileToFollow()
        {
            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                int id = (int)TempData["FollowingId"];
                var user = db.Users.Find(id);
                List<Post> posts = db.Posts.OrderByDescending(x => x.CreatedAt).Where(x => x.UserId == id).ToList();
                ViewData["SessionUserId"] = HttpContext.Session.GetInt32("UserID");
                ViewData["User"] = user;
                ViewData["Posts"] = posts;
                ViewData["IsFollow"] = TempData["IsFollow"];
                return View("Profile");
            }
            return RedirectToAction("Index", "Home");
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

        public static string GetAllColumns()
        {
            return "UserId,Name,Surname,Email,PhoneNum,Role,Birthdate,About Me,Location";
        }
        private string Decrypt(string cipherText) // bu fonksiyonu nerede kullanıcaz bilmiyorum ama kaybetmemek adına buraya ekledim
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }


    }
}
