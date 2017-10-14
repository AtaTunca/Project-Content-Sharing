using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

            return View();
        }
    }
}