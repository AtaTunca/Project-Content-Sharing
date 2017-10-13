using Project_Content_Sharing.Models;
using Project_Content_Sharing.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_Content_Sharing.Controllers
{
    public class HomeController : Controller
    {
        public class LoginVM
        {
            [Required]
            public string EmailAddress { get; set; }

            [Required]
            public string Password { get; set; }

            public bool RememberMe { get; set; }

        }

        public class RegisterVM
        {
            [Required]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            public string EmailAddress { get; set; }

            [Required]
            [MinLength(8)]
            [MaxLength(12)]
            public string Password { get; set; }

            [Required]
            [System.ComponentModel.DataAnnotations.Compare("Password")]
            public string PasswordAgain { get; set; }

            
            public bool Terms { get; set; }

        }





        public ActionResult Register()
        {


            return View();
        }


        public ActionResult Main()
        {

            return View();
        }

        // GET: Home
        public ActionResult Index()
        {


            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {

            if (ModelState.IsValid)
            {
                UserTable t = new UserTable();

                t.EmailAddress = model.EmailAddress;
                t.UserName = model.UserName;
                t.Password = model.Password;
                t.IsEnabled = false;
                t.ActivationCode = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);



                using (UserAccountDBEntities db = new UserAccountDBEntities())
                {
                    db.UserTable.Add(t);
                    db.SaveChanges();

                    JsonMessageResult j1 = new JsonMessageResult();
                    j1.IsSuccess = true;
                    j1.Message = "Kullanıcı Kaydınız Yapıldı";
                    j1.RedirectUrl = "/account/login";

                    var url = Path.Combine("http://localhost:1266/account/activate/", t.ActivationCode);

                    MailService s = new MailService();
                    s.SendMessage(new MailTemplate { Subject = "Üyelik", To = model.EmailAddress, Message = "<a href=" + url + ">Üyeliği Aktif Et</a>" });

                    return Json(j1);
                }
            }

            JsonMessageResult j = new JsonMessageResult();
            j.IsSuccess = false;
            j.Message = "Kullanıcı Kaydınız Yapılamadı.Lütfen tekrar Deneyiniz";
            j.RedirectUrl = null;

            return Json(j);
        }
        public class JsonMessageResult
        {
            public string Message { get; set; }
            public bool IsSuccess { get; set; }
            public string RedirectUrl { get; set; }

        }
    }
}