using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Jinher.AMP.BTP.UI.Filters
{
    public class CheckUserIdAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Guid userId = Guid.Empty;

            string strUserId = filterContext.HttpContext.Request.QueryString["userId"];
            Guid.TryParse(strUserId, out userId);

            string sessionid = filterContext.HttpContext.Request.QueryString["sessionId"];

            if (userId == Guid.Empty || string.IsNullOrWhiteSpace(sessionid))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "SetMobile", action = "Index" }));
            }
        }
    }
}