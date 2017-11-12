using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Project_Content_Sharing
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("activation", "Home/activate/{code}", new { controller = "Home", action = "Activate", code = UrlParameter.Optional });

            routes.MapRoute("CommentPage", "Account/Comment/{code}", new { controller = "Account", action = "Comment", code = UrlParameter.Optional });

            routes.MapRoute("CommentHome", "Home/Comment/{code}", new { controller = "Home", action = "Comment", code = UrlParameter.Optional });

            routes.MapRoute("resetpassword", "Home/ResetPassword/{code}", new { controller = "Home", action = "ResetPassword", code = UrlParameter.Optional });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Hot", id = UrlParameter.Optional }
            );
        }
    }
}
