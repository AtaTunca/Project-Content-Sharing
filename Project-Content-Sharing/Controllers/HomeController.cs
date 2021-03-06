﻿using Project_Content_Sharing.Models;
using Project_Content_Sharing.Service;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Project_Content_Sharing.Controllers
{
    public class HomeController : Controller
    {
        public class LoginVM
        {

            [Required(ErrorMessage = "Please enter E-Mail Adress.")]
            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            public string EmailAddress { get; set; }
            [Required(ErrorMessage = "Please enter Password.")]
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


            public bool TermsAndConditions { get; set; }

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
        public ActionResult PasswordReset()
        {
            return View();
        }
        public class CaptchaResponse
        {

            public bool Success { get; set; }

            public List<string> ErrorCodes { get; set; }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(PasswordResetVM model)
        {

            var response = Request["g-recaptcha-response"];
            const string secret = "6LcjsTQUAAAAALUtX2jw6R8y7oEkK8zHtBolrLli";
            //Kendi Secret keyinizle değiştirin.

            //webclient recaptcha test
            var client = new WebClient();
            var reply =
                client.DownloadString(
                    string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));

            var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);

            if (ModelState.IsValid)
            {
                ContentSharingEntities1 db = new ContentSharingEntities1();
                var usr = db.UserTable.FirstOrDefault(x => x.ActivationCode == model.ActivationCode);

                if (usr != null)
                {
                    if (!captchaResponse.Success)
                    {
                        ViewBag.Message = "Lütfen güvenliği doğrulayınız.";

                    }
                    else
                    {
                        ViewBag.Message = "Güvenlik başarıyla doğrulanmıştır.";
                        usr.Password = model.Password;
                        db.SaveChanges();
                    }
                }
            }

            return View();
        }
        public ActionResult ResetPassword(string code)
        {
            ContentSharingEntities1 db = new ContentSharingEntities1();
            var usr = db.UserTable.FirstOrDefault(x => x.ActivationCode == code);

            if (usr != null)
            {
                ViewBag.Code = usr.ActivationCode;
                return View();
            }

            return Redirect("/account/login");
        }
        public class PasswordResetVM
        {
            [Required]
            [MinLength(8)]
            [MaxLength(12)]
            public string Password { get; set; }

            [System.ComponentModel.DataAnnotations.Compare("Password")]
            public string PasswordAgain { get; set; }

            public string ActivationCode { get; set; }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PasswordReset(string Email)
        {
            ContentSharingEntities1 db = new ContentSharingEntities1();
            var usr = db.UserTable.FirstOrDefault(x => x.EmailAddress == Email && x.IsEnabled == true);

            var url = Path.Combine("http://localhost:62423/Home/resetpassword/", usr.ActivationCode);

            MailTemplate t = new MailTemplate();
            t.To = Email;
            t.Message = "<a href=" + url + ">Parola Resetle</a>";
            t.Subject = "Parola Değişikliği";

            MailService service = new MailService();
            service.SendMessage(t);

            ViewBag.Message = "Lütfen eposta hesabınızı kontrol ediniz";

            return View();
        }

        public ActionResult Activate(string code)
        {

            using (ContentSharingEntities1 db = new ContentSharingEntities1())
            {
                var user = db.UserTable.FirstOrDefault(x => x.ActivationCode == code);

                if (user != null)
                {
                    user.IsEnabled = true;
                    db.SaveChanges();
                    return Redirect("/Home/Main");
                }

            }

            return Redirect("/Home/Register");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                using (ContentSharingEntities1 db = new ContentSharingEntities1())
                {
                    bool control = db.UserTable.Any(x => x.EmailAddress == model.EmailAddress && x.Password == model.Password && x.IsEnabled == true);

                    if (control == true)
                    {
                        //oturum aç
                        FormsAuthentication.SetAuthCookie(model.EmailAddress, true);

                        return Redirect("/Account/Hot");
                    }
                }

                JsonMessageResult j = new JsonMessageResult();
                j.IsSuccess = true;
                j.Message = "Kullanıcı adı veya parola hatalı!";
                j.RedirectUrl = "/Home/Main";

                return Json(j);
            }


            return Json("Böyle bir Kullanıcı veya parola bulunamadı!");
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



                using (ContentSharingEntities1 db = new ContentSharingEntities1())
                {

                    var a = (from k in db.UserTable where t.EmailAddress == k.EmailAddress select k).Any();

                    if (a == false)
                    {
                        db.UserTable.Add(t);
                        db.SaveChanges();

                        JsonMessageResult j1 = new JsonMessageResult();
                        j1.IsSuccess = true;
                        j1.Message = "Kullanıcı Kaydınız Yapıldı.E-Mail Adresinize Doğrulama Linki Gönderildi!";
                        j1.RedirectUrl = "/account/login";

                        var url = Path.Combine("http://localhost:62423/Home/activate/", t.ActivationCode);

                        MailService s = new MailService();
                        s.SendMessage(new MailTemplate { Subject = "Üyelik", To = model.EmailAddress, Message = "<a href=" + url + ">Üyeliği Aktif Et</a>" });

                        return Json(j1);
                    }
                    else
                    {
                        JsonMessageResult j2 = new JsonMessageResult();
                        j2.IsSuccess = false;
                        j2.Message = "Email Adresi zaten mevcut";
                        j2.RedirectUrl = null;

                        return Json(j2);
                    }
                   



                }
            }

            JsonMessageResult j = new JsonMessageResult();
            j.IsSuccess = false;
            j.Message = "Kullanıcı Kaydınız Yapılamadı.Lütfen tekrar Deneyiniz";
            j.RedirectUrl = null;

            return Json(j);
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
        public class JsonMessageResult
        {
            public string Message { get; set; }
            public bool IsSuccess { get; set; }
            public string RedirectUrl { get; set; }

        }
    }
}