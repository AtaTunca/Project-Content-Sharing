using Project_Content_Sharing.Models;
using Project_Content_Sharing.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Project_Content_Sharing.Controllers
{
    [Authorize]

    public class AccountController : Controller
    {
        public class PasswordRe
        {
            [Required]
            public string OldPassword { get; set; }
            [Required]
            [MinLength(8)]
            [MaxLength(12)]
            public string NewPassword { get; set; }
            [Required]
            [MinLength(8)]
            [MaxLength(12)]
            [System.ComponentModel.DataAnnotations.Compare("NewPassword")]
            public string PasswordAgain { get; set; }

        }
        public class SettingsRe
        {
            [Required]
            public string UserName { get; set; }
            [Required]
            [EmailAddress(ErrorMessage ="Invalid Email")]
            public string EmailAddress { get; set; }
           

        }

        // GET: Account
        public ActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                //cookie deki bilgiyi name ile çektik
                ViewBag.Email = HttpContext.User.Identity.Name;
                
            }

            ContentSharingEntities ImgDB = new ContentSharingEntities();
            var model = ImgDB.ImgDB.ToList();

            return View(model);
        }
        public ActionResult Hot()
        {

            return View();
        }

        public ActionResult Password()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Password(PasswordRe PW)
        {
            using (ContentSharingEntities DB = new ContentSharingEntities())
            {

                bool Pass = DB.UserTable.Any(x => x.Password == PW.OldPassword);

                var user = DB.UserTable.FirstOrDefault(x => x.EmailAddress == HttpContext.User.Identity.Name);

                //var Pass = (from a in DB.UserTable where a.EmailAddress == HttpContext.User.Identity.Name select a.Password).Single<string>();

                if (Pass)
                {
                    user.Password = PW.NewPassword;
                    DB.SaveChanges();

                }

            }


            return View();
        }
        
        public ActionResult Settings()
        {

            using (ContentSharingEntities DB = new ContentSharingEntities())
            {


                var user = DB.UserTable.FirstOrDefault(x => x.EmailAddress == HttpContext.User.Identity.Name);

                ViewBag.EmailAdress = user.EmailAddress;
                ViewBag.UserName = user.UserName;


            }

            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Settings(SettingsRe Setting)
        {
            using (ContentSharingEntities DB = new ContentSharingEntities())
            {

                var user = DB.UserTable.FirstOrDefault(x => x.EmailAddress == HttpContext.User.Identity.Name);


                user.UserName = Setting.UserName;
                user.EmailAddress = Setting.EmailAddress;
                DB.SaveChanges();


                return Redirect("/Account/Settings");
            }
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            using (ContentSharingEntities DB = new ContentSharingEntities())
            {
                var ID = (from a in DB.UserTable where a.EmailAddress == HttpContext.User.Identity.Name select a.Id).First();
                try
                {
                    file.SaveAs(Server.MapPath("~/Images/" + file.FileName));
                    string Route = "/Images/" + file.FileName;

                    ImgDB r = new ImgDB();
                    r.UserID = ID;
                    r.ImgVote = 2;
                    r.Name = file.FileName;
                    r.Route = Route;
                    

                    ContentSharingEntities db = new ContentSharingEntities();
                    db.ImgDB.Add(r);
                    db.SaveChanges();
                }
                catch (Exception)
                {

                    throw;
                }
            }


           



            return Redirect("index");
        }



        public ActionResult Logout()
        {

            FormsAuthentication.SignOut();


            return Redirect("/Home/Main");
        }




    }
}