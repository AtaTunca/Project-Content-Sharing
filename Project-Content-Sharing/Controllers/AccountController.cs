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
            [EmailAddress(ErrorMessage = "Invalid Email")]
            public string EmailAddress { get; set; }


        }

        // GET: Account
        public ActionResult Upload()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                //cookie deki bilgiyi name ile çektik
                ViewBag.Email = HttpContext.User.Identity.Name;

            }

            ContentSharingEntities1 ImgDB = new ContentSharingEntities1();
            var model = ImgDB.ImgDB.ToList();

            return View(model);
        }
        public ActionResult Hot()
        {
            ContentSharingEntities1 db = new ContentSharingEntities1();


            var model2 = db.ImgDB.ToList();


            List<ImageDBList> model = new List<ImageDBList>(model2.Count);

            for (int i = 0; i < model2.Count; i++)
            {

                ImageDBList List = new ImageDBList()
                {
                    ImgID = model2[i].ImgID,
                    UserID = model2[i].UserID,
                    Route = model2[i].Route,
                    Description = model2[i].Description,
                    CommentNumber = model2[i].Comment.Count(),
                    VoteNumber = model2[i].ImgVote.Count
                };

                if (List.VoteNumber > 7)
                {
                    model.Add(List);
                }


            }



            return View(model);





            //var model2 = (from I in db.ImgDB
            //              join C in db.Comment
            //              on I.ImgID equals C.ImageID
            //              into groupofleft
            //              group I by I.ImgID into grup
            //              select new
            //              {
            //                  ImgID = grup.Key,
            //                  Count = grup.Count()
            //              }).ToList();


            //var model3 = (from I in db.ImgDB
            //              join C in db.Comment on I.ImgID equals C.ImageID into j1
            //              from j2 in j1.DefaultIfEmpty()
            //              group j2 by 
            //              new
            //              {
            //                  I.ImgID,
            //                  I.Route,
            //                  I.UserID,
            //                  I.Description
            //              }
            //              into grouped
            //              select new
            //              {
            //                  ParentId = grouped.Key,
            //                  Count = grouped.Count(x => x.ImageID != null)


            //              }).ToList();

            //var model4 = (from C in db.Comment
            //              join I in db.ImgDB on C.ImageID equals I.ImgID into j1
            //              from j2 in j1.DefaultIfEmpty()
            //              group j2 by C.ImageID into grouped
            //              select new
            //              {

            //                  ParentId = grouped.Key,
            //                  Route = grouped.Select(x=> x.Route),
            //                  Count = grouped.Count(x => x.ImgID != null)


            //              }).ToList();


            //var model = from I in db.ImgDB group I by I.ImgID;

            //var model = sql("Select ImgDB.ImgID,ImgDB.ImgVote,ImgDB.Route,ImgDB.UserID,ImgDB.Description,Count(Comment.ImageID) as Sum_Comment from ImgDB left JOIN Comment on(ImgDB.ImgID = Comment.ImageID) group by ImgDB.ImgID,ImgDB.ImgVote,ImgDB.Route,ImgDB.UserID,ImgDB.Description").ToList();



        }
        public ActionResult Trending()
        {
            ContentSharingEntities1 db = new ContentSharingEntities1();


            var model2 = db.ImgDB.ToList();


            List<ImageDBList> model = new List<ImageDBList>(model2.Count);

            for (int i = 0; i < model2.Count; i++)
            {

                ImageDBList List = new ImageDBList()
                {
                    ImgID = model2[i].ImgID,
                    UserID = model2[i].UserID,
                    Route = model2[i].Route,
                    Description = model2[i].Description,
                    CommentNumber = model2[i].Comment.Count(),
                    VoteNumber = model2[i].ImgVote.Count
                };

                if (List.VoteNumber > 5)
                {
                    model.Add(List);
                }


            }



            return View(model);
        }
        public ActionResult Fresh()
        {
            ContentSharingEntities1 db = new ContentSharingEntities1();


            var model2 = db.ImgDB.ToList();


            List<ImageDBList> model = new List<ImageDBList>(model2.Count);

            for (int i = 0; i < model2.Count; i++)
            {

                ImageDBList List = new ImageDBList()
                {
                    ImgID = model2[i].ImgID,
                    UserID = model2[i].UserID,
                    Route = model2[i].Route,
                    Description = model2[i].Description,
                    CommentNumber = model2[i].Comment.Count(),
                    VoteNumber = model2[i].ImgVote.Count
                };

                if (List.VoteNumber < 5)
                {
                    model.Add(List);
                }


            }



            return View(model);
        }

        public ActionResult Password()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Password(PasswordRe PW)
        {
            using (ContentSharingEntities1 DB = new ContentSharingEntities1())
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

        public ActionResult Comment(string id)
        {
            int newid = Convert.ToInt32(id);

            ContentSharingEntities1 DB = new ContentSharingEntities1();

            List<Comments> Comment = new List<Comments>();


            var data = (from d in DB.ImgDB where d.ImgID == newid select d).First();

            ViewBag.ID = data.ImgID;
            ViewBag.Route = data.Route;
            ViewBag.Description = data.Description;
            ViewBag.Point = data.ImgVote.Count;
            ViewBag.Comment = data.Comment.Count;



            var model = (from a in DB.Comment where a.ImageID == newid select a).ToList();

            for (int i = 0; i < model.Count; i++)
            {
                Comments CList = new Comments()
                {
                    CommentText = model[i].CommentText,
                    VoteComment = model[i].VoteComment,
                    ImgID = model[i].ImageID,
                    CommentUsrID = model[i].UserID,
                    UserName = model[i].UserTable.UserName
                };

                Comment.Add(CList);
                


            }



            return View(Comment);
        }

        

        public ActionResult Settings()
        {

            using (ContentSharingEntities1 DB = new ContentSharingEntities1())
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
            using (ContentSharingEntities1 DB = new ContentSharingEntities1())
            {

                var user = DB.UserTable.FirstOrDefault(x => x.EmailAddress == HttpContext.User.Identity.Name);


                user.UserName = Setting.UserName;
                user.EmailAddress = Setting.EmailAddress;
                DB.SaveChanges();


                return Redirect("/Account/Settings");
            }
        }
        public ActionResult PostComment(string Comment,int ImageID)
        {

            ContentSharingEntities1 DB = new ContentSharingEntities1();

            var UserID = (from a in DB.UserTable where a.EmailAddress == HttpContext.User.Identity.Name select a.Id).First();

            Comment comment = new Comment();

            comment.CommentText = Comment;
            comment.ImageID = ImageID;
            comment.UserID = UserID;
            comment.VoteComment = 0;

            DB.Comment.Add(comment);
            DB.SaveChanges();


            return Redirect("/Account/Comment?id="+ImageID);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file, string description)
        {
            using (ContentSharingEntities1 DB = new ContentSharingEntities1())
            {
                var ID = (from a in DB.UserTable where a.EmailAddress == HttpContext.User.Identity.Name select a.Id).First();

                var allowedExtensions = new[] { ".jpg", ".gif", ".mp4", ".png" };

                var checkextension = Path.GetExtension(file.FileName).ToLower();

                bool extension = false;

                foreach (var item in allowedExtensions)
                {
                    if (item.Contains(checkextension))
                    {
                        extension = true;

                    }

                }


                if (extension)
                {
                    try
                    {
                        file.SaveAs(Server.MapPath("~/Images/" + file.FileName));
                        string Route = "/Images/" + file.FileName;

                        ImgDB r = new ImgDB();

                        r.UserID = ID;
                        r.Description = description;
                        r.Route = Route;


                        ContentSharingEntities1 db = new ContentSharingEntities1();
                        db.ImgDB.Add(r);
                        db.SaveChanges();
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }






            return Redirect("/Account/Hot");
        }
        [HttpPost]
        public JsonResult UpVote(string id)
        {
            var newid = Int32.Parse(id);



            ContentSharingEntities1 DB = new ContentSharingEntities1();

            ImgVote Img = new ImgVote();

            var userid = (from a in DB.UserTable where a.EmailAddress == HttpContext.User.Identity.Name select a.Id).First();

            bool exist = (from b in DB.ImgVote where b.UserID == userid && b.ImgID == newid select b).Any();

            if (!exist)
            {

                Img.ImgID = newid;
                Img.UserID = userid;

                DB.ImgVote.Add(Img);
                DB.SaveChanges();

                return Json("1", JsonRequestBehavior.AllowGet);
            }


            return Json("0", JsonRequestBehavior.AllowGet);
        }

        public JsonResult DownVote(string id)
        {
            var newid = Int32.Parse(id);



            ContentSharingEntities1 DB = new ContentSharingEntities1();


            var userid = (from a in DB.UserTable where a.EmailAddress == HttpContext.User.Identity.Name select a.Id).First();


            bool exist = (from b in DB.ImgVote where b.UserID == userid && b.ImgID == newid select b).Any();

            if (exist)
            {

                var imgid = (from c in DB.ImgVote where c.UserID == userid && c.ImgID == newid select c.ImgVoteID).First();


                int newID = Convert.ToInt32(imgid);

                ImgVote Img = DB.ImgVote.Find(newID);

                DB.ImgVote.Remove(Img);
                DB.SaveChanges();

                return Json("1", JsonRequestBehavior.AllowGet);
            }


            return Json("0", JsonRequestBehavior.AllowGet);
        }


        public ActionResult Logout()
        {

            FormsAuthentication.SignOut();


            return Redirect("/Home/Hot");
        }




    }
}