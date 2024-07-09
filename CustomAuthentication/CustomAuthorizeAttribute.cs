using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OperationsManagementSystem.CustomAuthentication
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected virtual CustomPrincipal CurrentUser
        {   
            get { return HttpContext.Current.User as CustomPrincipal; }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.User.Identity.IsAuthenticated ? true: false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            RedirectToRouteResult routeData = null;

                routeData = new RedirectToRouteResult
                (new System.Web.Routing.RouteValueDictionary
                 (new
                 {
                     controller = "Home",
                     action = "UnAuthorize"
                 }
                 ));

            filterContext.Result = routeData;
        }

    }
}