using Microsoft.AspNetCore.Mvc;
using LinkedHU_CENG.Models;
using System.Text;
using System.Security.Cryptography;

namespace LinkedHU_CENG.Controllers
{
    public class UnregisteredUserController : Controller
    {
        private readonly ApplicationDbContext db;

        public UnregisteredUserController(ApplicationDbContext context)
        {
            this.db = context;
        }


        public IActionResult Index()
        {
            return View(db.UnregisteredUsers.ToList());
        }

        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("UserID") == null)
            {
                ViewData["Date"] = DateTime.Now.AddYears(-18).ToString("yyyy-MM-dd");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult Register(UnregisteredUser usr)
        {
            try
            {
                if (ModelState.IsValid)
                {
                
                var user1 = db.Users.FirstOrDefault(u => u.Email.Equals(usr.Email));
                var user2 = db.Users.FirstOrDefault(u => u.Email.Equals(usr.SecondEmail));
                var user3 = db.Users.FirstOrDefault(u => u.PhoneNum.Equals(usr.PhoneNum));

                //if user did not register with the same email and phone number before
                if (user1 == null && user2 == null && user3 == null)
                {
                    usr.Password = Encrypt(usr.Password);
                    db.UnregisteredUsers.Add(usr);
                    db.SaveChanges();
                    TempData["RegisterState"] = 1;
                }
                else
                {
                    TempData["RegisterState"] = 2;
                }
                //ELSE we need to give WARNING
                return RedirectToAction("Login", "User");
            }
            
            }
            catch
            {
                TempData["RegisterState"] = 3;
            }
            return RedirectToAction("Login", "User");
        }

        public ActionResult Delete(int? id)
        {
            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {
                if (ModelState.IsValid)
                {
                    var enRegistered = db.UnregisteredUsers.Find(id);
                    if (enRegistered == null)
                    {
                        return NotFound();
                    }
                    TempData["stateRevert"] = 1;
                    db.UnregisteredUsers.Remove(enRegistered);
                    db.SaveChanges();

                    return RedirectToAction("VerifyAccounts", "Administrator");
                }
                else
                {
                    TempData["stateRevert"] = -1;
                    ModelState.AddModelError("", "Some Error Occured!");
                    return RedirectToAction("Index", "Administrator");

                }
            }
            else
            {
                return RedirectToAction("Index", "Administrator");
            }
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
