using Project_Content_Sharing.Models;
using Project_Content_Sharing.Service;
using System;
using System.Collections.Generic;
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