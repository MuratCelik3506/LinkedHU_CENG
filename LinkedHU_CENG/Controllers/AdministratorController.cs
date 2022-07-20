using Microsoft.AspNetCore.Mvc;
using LinkedHU_CENG.Models;
using System.Net.Mail;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace LinkedHU_CENG.Controllers
{

    public class AdministratorController : Controller
    {

        private readonly ApplicationDbContext db;

        public AdministratorController(ApplicationDbContext context)
        {
            this.db = context;
        }


        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Admin_UserName") == null)
            {
                return RedirectToAction("Login", "Administrator");
            }
            else
            {
                List<User> users = db.Users.ToList();
                List<UnregisteredUser> unregisteredUsers = db.UnregisteredUsers.ToList();
                List<Report> reports = db.Reports.ToList();
                List<DeleteRequest> deleteRequests = db.DeleteRequests.ToList();
                List<ForgetPassword> passwordRequests = db.ForgetPasswords.ToList();
                List<Post> posts = db.Posts.ToList();
                List<Announcement> announcements = db.Announcements.ToList();
                List<MergeEmailRequest> mergeRequest = db.MergeEmailRequests.ToList();
                List<Advertisement> advertisements = db.Advertisements.ToList();
                //List<MergeRequest> mergeRequests = db.MergeRequest.ToList();
                ViewData["User"] = users;
                ViewData["UnregisteredUser"] = unregisteredUsers;
                ViewData["Report"] = reports;
                ViewData["DeleteRequest"] = deleteRequests;
                ViewData["PasswordRequest"] = passwordRequests;
                ViewData["Post"] = posts;
                ViewData["Announcement"] = announcements;
                ViewData["MergeEmailRequest"] = mergeRequest;
                ViewData["Advertisements"] = advertisements;
                ViewData["SystemVersion"] = "V02052022_release_1";
                //  ViewData["MergeReports"] = mergeRequests;
                return View();
            }
            
        }




        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("Admin_UserName") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Administrator");
            }
        }

        public IActionResult Login(Administrator administrator)
        {

            var info = db.Administrators.FirstOrDefault(u => u.UserName.Equals(administrator.UserName) && u.Password.Equals(administrator.Password));
            if (info != null)
            {
                HttpContext.Session.SetString("Admin_UserName", info.UserName);

                return RedirectToAction("Index", "Administrator");
            }
            return View();
        }





        public IActionResult Logout()
        {
            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {
                //HttpContext.Session.Clear();
                HttpContext.Session.Remove("Admin_UserName");
                return RedirectToAction("Index", "Administrator");
            }
            else
            {
                return RedirectToAction("Index", "Administrator");
            }
        }




        public IActionResult VerifyAccounts()
        {
            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {
                List<UnregisteredUser> unregisteredUsers = db.UnregisteredUsers.ToList();
                ViewData["UnregisteredUsers"] = unregisteredUsers;
                ViewData["stateAccept"] = TempData["stateAccept"];
                ViewData["stateRevert"] = TempData["stateRevert"];

                return View();
            }
            return RedirectToAction("Index", "Administrator");
        }

        public IActionResult VerifyAnAccount(int id)
        {
            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {
                if (ModelState.IsValid)
                {
                    var unregisteredUser = db.UnregisteredUsers.Find(id);
                    User user = new User() { Name = unregisteredUser.Name, Surname = unregisteredUser.Surname, Password = unregisteredUser.Password, Email = unregisteredUser.Email, Role = unregisteredUser.Role, BirthDate = unregisteredUser.BirthDate, PhoneNum = unregisteredUser.PhoneNum };

                    db.UnregisteredUsers.Remove(unregisteredUser);
                    db.Users.Add(user);
                    db.SaveChanges();
                    TempData["stateAccept"] = 1;
                    return RedirectToAction("VerifyAccounts", "Administrator");
                }
                else
                {
                    TempData["stateAccept"] = -1;
                    ModelState.AddModelError("", "Some Error Occured!");
                    return RedirectToAction("Index", "Administrator");

                }
            }
            else
            {
                return RedirectToAction("Index", "Administrator");
            }
        }




        [HttpGet]
        public IActionResult ReportedUser()
        {

            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {
                List<Report> reports = db.Reports.ToList();
                ViewData["Report"] = reports;
                return View();
            }
            return RedirectToAction("Index", "Administrator");

        }






        public List<User> Search(string word, string role) {
            char[] charsToTrim = { '*', ' ', '\'' };
            string[] wordSearch = word.Trim(charsToTrim).Split(" ");
            string nameSurnameCommand = $"SELECT * FROM \"Users\" WHERE \"Name\" ILIKE '%{word}%'";
            if (role == "student")
            {
                nameSurnameCommand = $"SELECT * FROM \"Users\" WHERE (\"Name\" ILIKE '%{word}%') and (\"Role\" = '{role}')";
            }
            var nameSurnameList = db.Users.FromSqlRaw(nameSurnameCommand).ToList();

            string nameCommand = $"SELECT * FROM \"Users\" WHERE \"Name\" ILIKE '%{wordSearch[0]}%'";
            if (role == "student")
            {
                nameCommand = $"SELECT * FROM \"Users\" WHERE (\"Name\" ILIKE '%{wordSearch[0]}%') and (\"Role\" = '{role}')";
            }
            var nameList = db.Users.FromSqlRaw(nameCommand).ToList();
            var surnameList = new List<User>();

            if (wordSearch.Length > 1)
            {
                string surnameCommand = $"SELECT * FROM \"Users\" WHERE \"Surname\" ILIKE '%{wordSearch[1]}%'";
                if (role == "student")
                {
                    surnameCommand = $"SELECT * FROM \"Users\" WHERE (\"Surname\" ILIKE '%{wordSearch[1]}%') and (\"Role\" = '{role}')";
                }
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
            foreach (var usrNameSurname in nameList) { nameSurnameList.Add(usrNameSurname); }
            foreach (var usrNameSurname in surnameList) { nameSurnameList.Add(usrNameSurname); }

            return nameSurnameList;
        }

        [HttpGet]
        public IActionResult BannedUser()
        {

            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {
                List<BannedUser> bannedUsers = db.BannedUsers.ToList();
                ViewData["bannedUsers"] = bannedUsers;
                List<User> users = new List<User>();
                ViewData["users"] = users;
                ViewData["stateBan"] = TempData["stateBan"];
                ViewData["stateUnBan"] = TempData["stateUnBan"];
                return View();
            }
            return RedirectToAction("Index", "Administrator");

        }

        [HttpPost]
        public IActionResult BannedUserSearch()
        {
            string name = HttpContext.Request.Form["SearchName"];
            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {
                List<BannedUser> bannedUsers = db.BannedUsers.ToList();
                List<User> users = Search(name, "all");
                
                ViewData["users"] = users;
                ViewData["bannedUsers"] = bannedUsers;

                return View("BannedUser");
            }
            return RedirectToAction("Index", "Administrator");
        }

        public IActionResult BannedUserAccept(int id)
        {
            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {
                if (ModelState.IsValid)
                {
                    var user = db.Users.Find(id);
                    BannedUser bannedUser = new BannedUser();
                    bannedUser.UserId = user.UserId;
                    bannedUser.Name = user.Name;
                    bannedUser.Surname = user.Surname;
                    bannedUser.Email = user.Email;
                    db.BannedUsers.Add(bannedUser);

                    user.IsBannedBefore = true;
                    db.Users.Update(user);
                    db.SaveChanges();
                    TempData["stateBan"] = 1;
                    return RedirectToAction("BannedUser", "Administrator");
                }
                else
                {
                    TempData["stateBan"] = -1;
                    ModelState.AddModelError("", "Some Error Occured!");
                    return RedirectToAction("Index", "Administrator");
                }
            }
            else
            {
                return RedirectToAction("Index", "Administrator");
            }
        }

        public IActionResult BannedUserRevert(int id)
        {
            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {
                if (ModelState.IsValid)
                {
                    var user = db.BannedUsers.Find(id);
                    db.BannedUsers.Remove(user);
                    db.SaveChanges();
                    TempData["stateUnBan"] = 1;

                    return RedirectToAction("BannedUser", "Administrator");
                }
                else
                {
                    TempData["stateUnBan"] = 1;
                    ModelState.AddModelError("", "Some Error Occured!");
                    return RedirectToAction("Index", "Administrator");
                }
            }
            else
            {
                return RedirectToAction("Index", "Administrator");
            }
        }



        [HttpGet]
        public IActionResult StudentRepresentative()
        {

            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {
                List<User> representative = db.Users.Where(x=>x.Role == "studentRepresentative").ToList();
                ViewData["studentRepresentatives"] = representative;
                List<User> users = new List<User>();
                ViewData["users"] = users;
                ViewData["stateselect"] = TempData["stateselect"];
                ViewData["stateUnselect"] = TempData["stateUnselect"];
                return View();
            }
            return RedirectToAction("Index", "Administrator");

        }

        [HttpPost]
        public IActionResult StudentRepresentativeSearch()
        {
            string name = HttpContext.Request.Form["SearchName"];
            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {
                List<User> studentRepresentatives = db.Users.Where(x=> x.Role == "studentRepresentative").ToList();
                List<User> students = Search(name, "student");
                ViewData["users"] = students;
                ViewData["studentRepresentatives"] = studentRepresentatives;

                return View("StudentRepresentative");
            }
            return RedirectToAction("Index", "Administrator");
        }
        public IActionResult StudentRepresentativeAccept(int id)
        {
            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {
                if (ModelState.IsValid)
                {
                    User user = db.Users.Find(id);
                    user.Role = "studentRepresentative";
                    db.Users.Update(user);
                    db.SaveChanges();
                    TempData["stateselect"] = 1;
                    return RedirectToAction("StudentRepresentative", "Administrator");
                }
                else
                {
                    TempData["stateselect"] = 1;
                    ModelState.AddModelError("", "Some Error Occured!");
                    return RedirectToAction("Index", "Administrator");
                }
            }
            else
            {
                return RedirectToAction("Index", "Administrator");
            }
        }

        public IActionResult StudentRepresentativeRevert(int id)
        {
            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {
                if (ModelState.IsValid)
                {
                    User user = db.Users.Find(id);
                    user.Role = "student";
                    db.Users.Update(user);
                    db.SaveChanges();
                    TempData["stateUnselect"] = 1;

                    return RedirectToAction("StudentRepresentative", "Administrator");
                }
                else
                {
                    TempData["stateUnselect"] = -1;
                    ModelState.AddModelError("", "Some Error Occured!");
                    return RedirectToAction("Index", "Administrator");
                }
            }
            else
            {
                return RedirectToAction("Index", "Administrator");
            }
        }


        public IActionResult DeleteUser()
        {
            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {

                List<DeleteRequest> requests = db.DeleteRequests.ToList();
                ViewData["DeleteRequest"] = requests;
                ViewData["stateAccept"] = TempData["stateAccept"];
                ViewData["stateRevert"] = TempData["stateRevert"];
                return View();
            }
            return RedirectToAction("Index", "Administrator");
        }

        public IActionResult DeleteUserAccept(int id) // kullanıcının post ve announcementlarını da siliyor
        {
            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {
                if (ModelState.IsValid)
                {
                    List<Post> posts = db.Posts.Where(m => m.UserId == id).ToList<Post>();
                    List<Announcement> announcements = db.Announcements.Where(m => m.UserId == id).ToList<Announcement>();
                    List<Comment> comments = db.Comments.Where(m => m.UserId == id).ToList<Comment>();
                    List<Resource> resources = db.Resources.Where(m => m.UserId == id).ToList<Resource>();
                    List<Advertisement> advertisements = db.Advertisements.Where(m => m.UserId == id).ToList<Advertisement>();
                    List<Application> applications = db.Applications.Where(m => m.UserId == id).ToList<Application>();
                    foreach (var post in posts)
                    {
                        db.Posts.Remove(post);
                    }
                    foreach (var announcement in announcements)
                    {
                        db.Announcements.Remove(announcement);
                    }
                    foreach (var comment in comments)
                    {
                        db.Comments.Remove(comment);
                    }
                    foreach (var resource in resources)
                    {
                        db.Resources.Remove(resource);
                    }
                    foreach (var advertisement in advertisements)
                    {
                        db.Advertisements.Remove(advertisement);
                    }
                    foreach (var application in applications)
                    {
                        db.Applications.Remove(application);
                    }

                    var deleteRequest = db.DeleteRequests.Find(id);
                    db.DeleteRequests.Remove(deleteRequest);
                    var deleteUser = db.Users.Find(id);
                    db.Users.Remove(deleteUser);

                    if (HttpContext.Session.GetInt32("UserID") == id)
                    {
                        HttpContext.Session.Remove("UserID");
                        HttpContext.Session.Remove("Email");
                    }

                    db.SaveChanges();
                    TempData["stateAccept"] = 1;

                    return RedirectToAction("DeleteUser", "Administrator");
                }
                else
                {
                    TempData["stateAccept"] = -1;
                    ModelState.AddModelError("", "Some Error Occured!");
                    return RedirectToAction("Index", "Administrator");
                }
            }
            else
            {
                return RedirectToAction("Index", "Administrator");
            }
        }

        public IActionResult DeleteUserRequest(int id)
        {
            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {
                if (ModelState.IsValid)
                {
                    var deleteRequest = db.DeleteRequests.Find(id);
                    db.DeleteRequests.Remove(deleteRequest);
                    db.SaveChanges();
                    TempData["stateRevert"] = 1;

                    return RedirectToAction("DeleteUser", "Administrator");
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



        
        public IActionResult ForgetPassword()
        {
            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {
                List<ForgetPassword> requests = db.ForgetPasswords.ToList();
                ViewData["ForgetPassword"] = requests;
                ViewData["stateAccept"] = TempData["stateAccept"];
                ViewData["stateRevert"] = TempData["stateRevert"];
                return View();
            }
            return RedirectToAction("Index", "Administrator");
        }

        public IActionResult DeleteForgetPasswordRequest(int id)
        {
            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {
                if (ModelState.IsValid)
                {
                    var request = db.ForgetPasswords.Where(m=>m.ID.Equals(id)).FirstOrDefault();
                    db.ForgetPasswords.Remove(request);
                    db.SaveChanges();
                    TempData["stateRevert"] = 1;
                    return RedirectToAction("ForgetPassword", "Administrator");
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

        public IActionResult ForgetPasswordAccept(int id)
        {
            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {
                if (ModelState.IsValid)
                {
                    var request = db.ForgetPasswords.Where(m => m.ID.Equals(id)).FirstOrDefault();
                    var user = db.Users.Where(m => m.Email.Equals(request.Email)).FirstOrDefault();
                    RandomNumberGenerator generator = new RandomNumberGenerator();
                    string pass = generator.RandomPassword();  // kullanıcıya gönderilecek parolayı yaratıyor, aşağıda fonksiyonu mevcut, ortak bir dosyaya kaydedilse daha iyi olur

                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                    mail.To.Add("mobilmurat4@gmail.com"); // buraya maili göndereceğimiz adres user.Email gelecek
                    mail.From = new MailAddress("explodinggradient2022@gmail.com", "Email head", System.Text.Encoding.UTF8);
                    mail.Subject = "Password Request for" + user.Name;
                    mail.SubjectEncoding = System.Text.Encoding.UTF8;
                    mail.Body = "\nYour new password is : " + pass; // şuraya güzel bir yazı yazılabilir
                    mail.BodyEncoding = System.Text.Encoding.UTF8;
                    mail.IsBodyHtml = true;
                    mail.Priority = MailPriority.High;
                    SmtpClient client = new SmtpClient();
                    client.Credentials = new System.Net.NetworkCredential("explodinggradient2022@gmail.com", "Bbm3842022");
                    client.Port = 587;
                    client.Host = "smtp.gmail.com";
                    client.EnableSsl = true;
                    try
                    {
                        client.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        TempData["stateAccept"] = -1;
                        RedirectToAction("Index", "Administrator");
                    }

                    user.Password = Encrypt(pass); // parolayı veritabanında da güncelliyor
                    db.Users.Update(user);

                    db.ForgetPasswords.Remove(request);
                    db.SaveChanges();
                    TempData["stateAccept"] = 1;
                    return RedirectToAction("ForgetPassword", "Administrator");
                }
                else
                {
                    TempData["stateAccept"] = -1;
                    ModelState.AddModelError("", "Some Error Occured!");
                    return RedirectToAction("Index", "Administrator");
                }
            }
            else
            {
                return RedirectToAction("Index", "Administrator");
            }
        }

        [HttpGet]
        public IActionResult ExportUserDetails()
        {

            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {
                List<User> users = db.Users.ToList();
                ViewData["Users"] = users;
                return View();
            }
            return RedirectToAction("Index", "Administrator");

        }

        public IActionResult ExportUserDetailsASCSV()
        {

            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {
                List<User> users = db.Users.ToList();
                string csv = string.Empty;
                string columnsOfUsers = UserController.GetAllColumns();
                csv += columnsOfUsers + ',';
                csv += "\r\n";
                foreach (User user in users)
                {
                    csv += user.ToString() + "\r\n";
                    System.Diagnostics.Debug.WriteLine(user.ToString());
                }

                return File(Encoding.UTF32.GetBytes(csv.ToString()), "text/csv", "UsersDetails.csv");
            }
            return RedirectToAction("Index", "Administrator");

        }




        [HttpGet]
        public IActionResult MergeEmailRequest()
        {
            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {
                List<MergeEmailRequest> requests = db.MergeEmailRequests.ToList();
                ViewData["MergeEmailRequest"] = requests;
                ViewData["stateAccept"] = TempData["stateAccept"];
                ViewData["stateRevert"] = TempData["stateRevert"];
                return View();
            }
            return RedirectToAction("Index", "Administrator");
        }

        public IActionResult MergeEmailRequestAccept(int id)
        {
            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {
                if (ModelState.IsValid)
                {
                    var request = db.MergeEmailRequests.Find(id);
                    var user = db.Users.Find(id);
                    user.SecondEmail = request.SecondEmail;
                    db.Users.Update(user);
                    db.MergeEmailRequests.Remove(request);
                    db.SaveChanges();
                    TempData["stateAccept"] = 1;
                    return RedirectToAction("MergeEmailRequest", "Administrator");
                }
                else
                {
                    TempData["stateAccept"] = 1;
                    ModelState.AddModelError("", "Some Error Occured!");
                    return RedirectToAction("Index", "Administrator");
                }
            }
            else
            {
                return RedirectToAction("Index", "Administrator");
            }
        }

        public IActionResult MergeEmailRequestRevert(int id)
        {
            if (HttpContext.Session.GetString("Admin_UserName") != null)
            {
                if (ModelState.IsValid)
                {
                    var request = db.MergeEmailRequests.Find(id);
                    db.MergeEmailRequests.Remove(request);
                    db.SaveChanges();
                    TempData["stateRevert"] = 1;
                    return RedirectToAction("MergeEmailRequest", "Administrator");
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


    public class RandomNumberGenerator   // bu sınıfı bi düzgünlestirmek lazım
    {
        // Generate a random number between two numbers    
        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        // Generate a random string with a given size and case.   
        // If second parameter is true, the return string is lowercase  
        public string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        // Generate a random password of a given length (optional)  
        public string RandomPassword(int size = 0)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(4, true));
            builder.Append(RandomNumber(1000, 9999));
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }
    }


}
